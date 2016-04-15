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

			// Append Blob への参照を作成
			var blobName = string.Format(
				Constants.BlobStorage.BlobFileNameFormat,
				DateTime.Now.ToString(Constants.BlobStorage.BlobFileNameDateTimeToStringFormat),
				sessionId.ToLower());
			var appendBlob = BlobContainer.GetAppendBlobReference(blobName);

			// データを文字列に変換
			var serializedTexts = apiData.Select(x => SerializeApiData(agentId, sessionId, x) + Constants.BlobStorage.ApiRawFileNewLine);
			var textToWrite = string.Concat(serializedTexts);

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
	}
}