using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KCVDB.Utils;
using Microsoft.WindowsAzure.Storage.Blob;

namespace KCVDB.Services.BlobStorage
{
	public class AzureBlobApiDataWriter : IApiDataWriter
	{
		public CloudBlobContainer BlobContainer { get; }

		public AzureBlobApiDataWriter(CloudBlobContainer blobContainer)
		{
			if (blobContainer == null) { throw new ArgumentNullException(nameof(blobContainer)); }
			BlobContainer = blobContainer;
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

            // 現在日付と前日の日付
            DateTime Today = DateTime.Now.AddHours(Constants.BlobStorage.OffsetTime);
            DateTime Yesterday = Today.AddDays(-1);

			// Append Blob への参照を作成（現在日付）
			var blobNameToday = string.Format(
				Constants.BlobStorage.BlobFileNameFormat,
				Today.ToString(Constants.BlobStorage.BlobFileNameDateTimeToStringFormat),
				sessionId.ToLower());
			var appendBlobToday = BlobContainer.GetAppendBlobReference(blobNameToday);

            // Append Blob への参照を作成（前日）
            var blobNameYesterday = string.Format(
                Constants.BlobStorage.BlobFileNameFormat,
                Yesterday.ToString(Constants.BlobStorage.BlobFileNameDateTimeToStringFormat),
                sessionId.ToLower());
            var appendBlobYesterday = BlobContainer.GetAppendBlobReference(blobNameYesterday);



            // データを文字列に変換
            var serializedTexts = apiData.Select(x => SerializeApiData(agentId, sessionId, x) + Constants.BlobStorage.ApiRawFileNewLine);
            var textToWrite = string.Concat(serializedTexts);

            CloudAppendBlob appendBlob = null;

            // 今日の日付のBlobが既に作成されている場合
            if (await appendBlobToday.ExistsAsync()){
                appendBlob = appendBlobToday;
            }
            // 今日の日付のBlobがない
            else{
                // 昨日の日付のBlobが存在する
                if(await appendBlobYesterday.ExistsAsync()){
                    // api_portが含まれる要素のindexを取得
                    var index = FindApiElement(apiData);
                    if(index > -1){
                        var dataOfYesterday = apiData.Take(index).Select(x => SerializeApiData(agentId, sessionId, x) + Constants.BlobStorage.ApiRawFileNewLine);
                        var dataOfToday = apiData.Skip(index -1).Select(x => SerializeApiData(agentId, sessionId, x) + Constants.BlobStorage.ApiRawFileNewLine);

                        // 今日のBlobは絶対存在しないのでここで作成
                        await appendBlobToday.CreateOrReplaceAsync();

                        // ....〆(･ω･｀ )ｶｷｶｷ
                        await appendBlobYesterday.AppendTextAsync(string.Concat(dataOfYesterday));
                        await appendBlobToday.AppendTextAsync(string.Concat(dataOfToday));

                        // 両方書いたので終わり
                        return;
                    }
                    else{
                        // 見つからないなら前日のファイルに書き込み
                        appendBlob = appendBlobYesterday;
                    }

                }
                else{
                    // 今日も昨日のBlobがない場合は今日のBlobを利用する
                    appendBlob = appendBlobToday;
                }
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
        int FindApiElement(IEnumerable<ApiData> apiDatas)
        {
            return (apiDatas.Select((x, i) => new { Data = x, Index = i })
                            .FirstOrDefault(x => x.Data.RequestUri.Contains(Constants.BlobStorage.ApiPortEqual))
                            ?.Index ?? 0) - 1;
        }
	}
}