using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;

namespace KancolleVV.Logics.BlobStrage
{
    public class AzuleBlobService
    {
        private CloudStorageAccount _storageAccount = null;

        private CloudBlobClient _blobClient = null;

        public AzuleBlobService()
        {
            // とりあえず接続文字列は直で
            // なんかうまくWebConfigから読まなかった
            _storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["KCVDBStorageConnectionString"].ConnectionString);

            _blobClient = _storageAccount.CreateCloudBlobClient();
        }

        public void Add(string LoginSessinId, string path, string RequestValue, string ResponseValue, string AgentId, int? StatusCode, string HttpDate, string PCDate)
        {
            // コンテナーはログインセッションIDで作成
            CloudBlobContainer container = _blobClient.GetContainerReference("kancolleapidataraw");

			// Create the container if it doesn't already exist.
			container.CreateIfNotExists();

            // 追加Blobの参照生成
            string DateNow = DateTime.Now.ToString(@"yyyy\\MM\\dd");
            CloudAppendBlob appendBlob = container.GetAppendBlobReference(string.Format(@"{0}.log", DateNow));
            
            // Blob作成されていないなら作る
            if(!appendBlob.Exists())
            {
                appendBlob.CreateOrReplace();
            }

            // 改行削除
            path = path.Replace("\r\n", "").Replace("\n", "");
            RequestValue = RequestValue.Replace("\r\n", "").Replace("\n", "");
            ResponseValue = ResponseValue.Replace("\r\n", "").Replace("\n", "");

            appendBlob.AppendText(AgentId ?? "");
			appendBlob.AppendText("\t" + LoginSessinId ?? "");
			appendBlob.AppendText("\t" + path ?? "");
            appendBlob.AppendText("\t" + StatusCode.ToString() ?? "");
            appendBlob.AppendText("\t" + HttpDate ?? "");
            appendBlob.AppendText("\t" + PCDate ?? "");
            appendBlob.AppendText("\t" + RequestValue ?? "");
            appendBlob.AppendText("\t" + ResponseValue ?? "");
			appendBlob.AppendText("\r\n");

        }
    }
}