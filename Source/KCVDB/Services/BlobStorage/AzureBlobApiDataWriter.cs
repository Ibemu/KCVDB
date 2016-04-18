using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KCVDB.Utils;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace KCVDB.Services.BlobStorage
{
	public class AzureBlobApiDataWriter : IApiDataWriter
	{
		public CloudBlobContainer BlobContainer { get; }

        public CloudTable TableContainer { get; }

		public AzureBlobApiDataWriter(CloudBlobContainer blobContainer, CloudTable tableContainer)
		{
			if (blobContainer == null) { throw new ArgumentNullException(nameof(blobContainer)); }
            if (tableContainer == null) { throw new ArgumentNullException(nameof(tableContainer)); }
            BlobContainer = blobContainer;
            TableContainer = tableContainer;
		}

		public Task WriteAsync(string agentId, string sessionId, ApiData apiData)
		{
			return WriteAsync(agentId, sessionId, new ApiData[] { apiData });
		}

		public async Task WriteAsync(string agentId, string sessionId, IEnumerable<ApiData> apiData)
		{
			if (apiData == null) { throw new ArgumentNullException(nameof(apiData)); }
			if (agentId == null) { throw new ArgumentNullException(nameof(agentId)); }
			if (sessionId == null) { throw new ArgumentNullException(nameof(sessionId)); }

			// コンテナなかったら作る
			await BlobContainer.CreateIfNotExistsAsync();

            // テーブルがなかったら作る
            await TableContainer.CreateIfNotExistsAsync();

            // 現在日付
            DateTime Today = DateTime.Now.AddHours(Constants.BlobStorage.OffsetTime);
            // 前回アクセスした日付
            DateTime BeforeAccessday = Today.AddDays(-1);
            // TableStorageから取得
            TableOperation RetrieveOperation = TableOperation.Retrieve<SessinEntity>(sessionId,sessionId);
            TableResult RetrievedResult = TableContainer.Execute(RetrieveOperation);
            if(RetrievedResult.Result != null){
                BeforeAccessday = ((SessinEntity)RetrievedResult.Result).BeforeAccessTime;
            }
            else{
                // 該当するものがなかったら、昨日の日付にしておく
            }

			// Append Blob への参照を作成（現在日付）
			var blobNameToday = string.Format(
				Constants.BlobStorage.BlobFileNameFormat,
				Today.ToString(Constants.BlobStorage.BlobFileNameDateTimeToStringFormat),
				sessionId.ToLower());
			var appendBlobToday = BlobContainer.GetAppendBlobReference(blobNameToday);

            // Append Blob への参照を作成（前回アクセスした日付）
            var blobNameBeforeAccessday = string.Format(
                Constants.BlobStorage.BlobFileNameFormat,
                BeforeAccessday.ToString(Constants.BlobStorage.BlobFileNameDateTimeToStringFormat),
                sessionId.ToLower());
            var appendBlobBeforeAccessday = BlobContainer.GetAppendBlobReference(blobNameBeforeAccessday);
            
            // データを文字列に変換
            var serializedTexts = apiData.Select(x => SerializeApiData(agentId, sessionId, x) + Constants.BlobStorage.ApiRawFileNewLine);
            var textToWrite = string.Concat(serializedTexts);

            CloudAppendBlob appendBlob = null;
            bool isCreate = false;

            // 今日の日付のBlobが既に作成されている場合
            if (await appendBlobToday.ExistsAsync()){
                appendBlob = appendBlobToday;
            }
            // 今日の日付のBlobがない
            else{
                // 昨日の日付のBlobが存在する
                if(await appendBlobBeforeAccessday.ExistsAsync()){
                    // api_portが含まれる要素のindexを取得
                    var index = FindApiElement(apiData, Constants.BlobStorage.ApiPortEqual);
                    if(index > -1){
                        var dataOfBeforeAccessday = apiData.Take(index).Select(x => SerializeApiData(agentId, sessionId, x) + Constants.BlobStorage.ApiRawFileNewLine);
                        var dataOfToday = apiData.Skip(index -1).Select(x => SerializeApiData(agentId, sessionId, x) + Constants.BlobStorage.ApiRawFileNewLine);

                        // 今日のBlobは絶対存在しないのでここで作成
                        await appendBlobToday.CreateOrReplaceAsync();

                        // ....〆(･ω･｀ )ｶｷｶｷ
                        await appendBlobBeforeAccessday.AppendTextAsync(string.Concat(dataOfBeforeAccessday));
                        await appendBlobToday.AppendTextAsync(string.Concat(dataOfToday));

                        // 両方書いたので終わり
                        isCreate = true;
                    }
                    else{
                        index = FindApiElement(apiData, Constants.BlobStorage.ApiStart2Equal);
                        if (index > 0){
                            // start2が含まれていたら当日扱いにする
                            appendBlob = appendBlobToday;
                        }
                        else {
                            // それ以外の場合はportがくるまで、
                            // 前日データに関連している可能性があるので前日扱いとする
                            appendBlob = appendBlobBeforeAccessday;
                        }
                    }

                }
                else{
                    // 今日も昨日のBlobがない場合は今日のBlobを利用する
                    appendBlob = appendBlobToday;
                }
            }

            var updateEntry = (SessinEntity)RetrievedResult.Result;

            if(updateEntry != null){
                updateEntry.BeforeAccessTime = Today;
                TableOperation insertupdateOpe = TableOperation.InsertOrReplace(updateEntry);

                TableContainer.Execute(insertupdateOpe);
            }

            // もう作ってたら終わりー
            if(isCreate){
                return;
            }

            // Blobが生成されていなければ作成
            if (!(await appendBlob.ExistsAsync())) {
				await appendBlob.CreateOrReplaceAsync();
			}
            
            // ....〆(･ω･｀ )ｶｷｶｷ
            await appendBlob.AppendTextAsync(textToWrite);
		}

		string SerializeApiData(string agentId, string sessionId, ApiData apiData)
		{
			var columns = new string[]{
				agentId,
				sessionId,
				apiData.RequestUri,
				apiData.StatusCode?.ToString() ?? "",
				apiData.HttpDate,
				apiData.LocalTime,
				apiData.RequestBody,
				apiData.ResponseBody
			};

			// TSVに変換
			return string.Join(
				"\t",
				columns.Select(x => x?.RemoveNewLiens() ?? ""));
		}

        /// <summary>
        /// 受信したデータからapi_portを含んだ要素を検索
        /// </summary>
        /// <param name="apiDatas">受信したデータ</param>
        /// <returns>一致した要素が見つかったindex</returns>
        int FindApiElement(IEnumerable<ApiData> apiDatas, string apiUrl)
        {
            return (apiDatas.Select((x, i) => new { Data = x, Index = i })
                            .FirstOrDefault(x => x.Data.RequestUri.Contains(apiUrl))
                            ?.Index ?? 0) - 1;
        }
	}
}