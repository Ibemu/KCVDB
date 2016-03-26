using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using KCVDB.Utils;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace KCVDB.Services
{
	internal class AzureBlobService
	{
		private CloudStorageAccount _storageAccount;
		private CloudBlobClient _blobClient;

		public AzureBlobService()
		{
			_storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings[Constants.Storage.ApiDataStorageKey].ConnectionString);
			_blobClient = _storageAccount.CreateCloudBlobClient();
		}


		public async Task LogApiDataAsync(
			string sessionId,
			string path,
			string requestValue,
			string responseValue,
			string agentId,
			int? statusCode,
			string httpDate,
			string localTime)
		{
			if (sessionId == null) { throw new ArgumentNullException(nameof(sessionId)); }
			if (path == null) { throw new ArgumentNullException(nameof(path)); }
			if (requestValue == null) { throw new ArgumentNullException(nameof(requestValue)); }
			if (responseValue == null) { throw new ArgumentNullException(nameof(responseValue)); }

			// コンテナーはログインセッションIDで作成
			var container = _blobClient.GetContainerReference(Constants.Storage.ApiDataBlobContainerName);

			// コンテナなかったら作る
			container.CreateIfNotExists();

			// 追加Blobの参照生成
			string DateNow = DateTime.Now.ToString(Constants.Storage.BlobFileNameDateTimeToStringFormat);
			var appendBlob = container.GetAppendBlobReference(string.Format(Constants.Storage.BlobFileNameFormat, DateNow));
			
			// 書き込む文字列を生成
			var columns = new string[]{
				agentId,
				sessionId,
				path,
				statusCode?.ToString(),
				httpDate,
				localTime,
				requestValue,
				responseValue,
			};
			var text = string.Join("\t", columns.Select(x => x?.RemoveNewLiens() ?? "")) + Constants.Storage.ApiRawFileNewLine;
			
			// Blob作成されていないなら作る
			if (!(await appendBlob.ExistsAsync())) {
				await appendBlob.CreateOrReplaceAsync();
			}

			// ....〆(･ω･｀ )ｶｷｶｷ
			await appendBlob.AppendTextAsync(text);

		}
	}
}
