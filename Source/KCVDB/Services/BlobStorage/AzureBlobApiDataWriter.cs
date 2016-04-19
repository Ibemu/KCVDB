using KCVDB.Utils;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            DateTime today = DateTime.Now.AddHours(Constants.BlobStorage.OffsetTime);
            // 前回アクセスした日付
            DateTime lastAccessDate = today.AddDays(-1);
            bool isUseToday = false;
            // TableStorageから取得
            TableOperation retrieveOperation = TableOperation.Retrieve<SessinEntity>(sessionId,sessionId);
            TableResult retrievedResult = TableContainer.Execute(retrieveOperation);
            if(retrievedResult.Result != null){
                lastAccessDate = ((SessinEntity)retrievedResult.Result).BeforeAccessTime;
            }
            else{
                isUseToday = true;
            }

            // 日付が一致した場合は当日のデータとして扱う
            if(today.Date.Equals(lastAccessDate.Date)){
                isUseToday = true;
            }

			// Append Blob への参照を作成（現在日付）
            var todayAppendBlob = GetAppendBlob(today, sessionId);

            // Append Blob への参照を作成（前回アクセスした日付）
            var lastAccessDateAppendBlob = GetAppendBlob(lastAccessDate, sessionId);

            CloudAppendBlob appendBlob = null;
            bool isCreate = false;

            // 前回アクセス日がない場合 or 前回アクセス日が現在日付と一致
            if(isUseToday){
                appendBlob = todayAppendBlob;
            }
            // 今日の日付のBlobがない
            else{
                // 昨日の日付のBlobが存在する
                if(await lastAccessDateAppendBlob.ExistsAsync()){
                    // api_portが含まれる要素のindexを取得
                    var index = FindApiElement(apiData, Constants.BlobStorage.ApiPortEqual);
                    if(index > -1){
                        var lastAccessDateData = apiData.Take(index);
                        var todayData = apiData.Skip(index -1);

                        await WriteBlob(todayAppendBlob, todayData, agentId, sessionId);
                        await WriteBlob(lastAccessDateAppendBlob, lastAccessDateData, agentId, sessionId);

                        // 両方書いたので終わり
                        isCreate = true;
                    }
                    else{
                        index = FindApiElement(apiData, Constants.BlobStorage.ApiStart2Equal);
                        if (index > 0){
                            // start2が含まれていたら当日扱いにする
                            appendBlob = todayAppendBlob;
                        }
                        else {
                            // それ以外の場合はportがくるまで、
                            // 前日データに関連している可能性があるので前日扱いとする
                            appendBlob = lastAccessDateAppendBlob;
                        }
                    }

                }
                else{
                    // 今日も昨日のBlobがない場合は今日のBlobを利用する
                    appendBlob = todayAppendBlob;
                }
            }

            // tableStorageに書き込み
            WriteTableStorage(retrievedResult, today);

            // もう作ってたら終わりー
            if(isCreate){
                return;
            }

            await WriteBlob(appendBlob, apiData, agentId, sessionId);
		}

        /// <summary>
        /// Blob書き込み
        /// </summary>
        /// <param name="appendBlob">Blobコンテナー</param>
        /// <param name="apiDatas">データリスト</param>
        /// <param name="agentId">agentId</param>
        /// <param name="sessionId">sessionId</param>
        /// <returns></returns>
        private async Task WriteBlob(CloudAppendBlob appendBlob, IEnumerable<ApiData> apiDatas, string agentId, string sessionId)
        {
            // データを文字列に変換
            var serializedTexts = apiDatas.Select(x => SerializeApiData(agentId, sessionId, x) + Constants.BlobStorage.ApiRawFileNewLine);
            var textToWrite = string.Concat(serializedTexts);

            // Blobが生成されていなければ作成
            if (!await appendBlob.ExistsAsync()){
                await appendBlob.CreateOrReplaceAsync();
            }

            // ....〆(･ω･｀ )ｶｷｶｷ
            await appendBlob.AppendTextAsync(textToWrite);
        }

        /// <summary>
        /// AppendBlob取得
        /// </summary>
        /// <param name="date">AppendBlob作成日付</param>
        /// <param name="sessionId">sessionId</param>
        /// <returns></returns>
        private CloudAppendBlob GetAppendBlob(DateTime date, string sessionId)
        {
            var blobName = string.Format(
                                Constants.BlobStorage.BlobFileNameFormat,
                                date.ToString(Constants.BlobStorage.BlobFileNameDateTimeToStringFormat),
                                sessionId.ToLower());
            return BlobContainer.GetAppendBlobReference(blobName);
        }

        /// <summary>
        /// TableStorageに書き込み（Insert or Replace）
        /// </summary>
        /// <param name="tableResult"></param>
        /// <param name="data"></param>
        private void WriteTableStorage(TableResult tableResult, DateTime data)
        {
            var updateEntry = (SessinEntity)tableResult.Result;

            if (updateEntry != null){
                updateEntry.BeforeAccessTime = data;
                TableOperation insertupdateOpe = TableOperation.InsertOrReplace(updateEntry);

                TableContainer.Execute(insertupdateOpe);
            }
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