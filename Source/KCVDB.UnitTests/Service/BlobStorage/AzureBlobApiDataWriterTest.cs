using System;
using KCVDB.Services;
using KCVDB.Services.BlobStorage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage.Blob;
using Moq;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;

namespace KCVDB.UnitTests.Service.BlobStorage
{
	[TestClass]
	public class AzureBlobApiDataWriterTest
	{
		[TestMethod]
		public void SerializeApiTest()
		{
			var agentId = Guid.NewGuid().ToString();
			var sessionId = Guid.NewGuid().ToString();
			var apiData = new ApiData {
				HttpDate = "unko",
				LocalTime = "chinko\nmanko",
				RequestBody = "chinchin \r\n manman",
				ResponseBody = "chimpoko",
				RequestUri = "manko",
				StatusCode = 0721,
			};

			var expected = string.Join(
				"\t",
				agentId,
				sessionId,
				"manko",	// RequestUri
				"721",		// StatusCode
				"unko",		// HttpDate
				"chinkomanko",		// LocalTime
				"chinchin  manman",	// RequestBody
				"chimpoko"			// ResponseBody
			);

			var containerMock = new Mock<CloudBlobContainer>(new Uri("http://kcvdb.jp"));
            var tableMock = new Mock<CloudTable>(new Uri("http://kcvdb.jp"));
			var po = new PrivateObject(typeof(AzureBlobApiDataWriter), containerMock.Object, tableMock.Object);
			var actual = (string)po.Invoke("SerializeApiData", agentId, sessionId, apiData);

			Assert.AreEqual(expected, actual);
		}

        [TestMethod]
        public void 送信テスト()
        {
            var agentId = Guid.NewGuid().ToString();
            var sessionId = "sessionIdsessionId";
            var apiData = new ApiData
            {
                HttpDate = "unko",
                LocalTime = "chinkomanko",
                RequestBody = "chinchinmanman",
                ResponseBody = "chimpoko",
                RequestUri = "manko",
                StatusCode = 0000,
            };

            var storageAccount = CloudStorageAccount.Parse("");
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(Constants.BlobStorage.ApiDataBlobContainerName);

            var tableClient = storageAccount.CreateCloudTableClient();
            var tableContainer = tableClient.GetTableReference(Constants.BlobStorage.ApiDataTableContainerName);

            var po = new PrivateObject(typeof(AzureBlobApiDataWriter), blobContainer, tableContainer);
            po.Invoke("WriteAsync", agentId, sessionId, new ApiData[] { apiData });
            //var service = new AzureBlobApiDataWriter(blobContainer, tableContainer);
            //service.WriteAsyncaaa(agentId, sessionId, new ApiData[] { apiData });
        }

        [TestMethod]
        public void 一覧取得()
        {
            var storageAccount = CloudStorageAccount.Parse("");
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(Constants.BlobStorage.ApiDataBlobContainerName);

            var tableClient = storageAccount.CreateCloudTableClient();
            var tableContainer = tableClient.GetTableReference(Constants.BlobStorage.ApiDataTableContainerName);

            var service = new AzureBlobApiDataWriter(blobContainer, tableContainer);
            var list = service.ReadTableStorage();
        }

        [TestMethod]
        public void 単一取得()
        {
            var storageAccount = CloudStorageAccount.Parse("");
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(Constants.BlobStorage.ApiDataBlobContainerName);

            var tableClient = storageAccount.CreateCloudTableClient();
            var tableContainer = tableClient.GetTableReference(Constants.BlobStorage.ApiDataTableContainerName);

            var service = new AzureBlobApiDataWriter(blobContainer, tableContainer);
            var str = service.ReadTableStorageOnly();

            
            Assert.AreEqual(str, "2016\\04\\23\\送信セッション.log");

            //Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void テーブルかきかき()
        {
            var storageAccount = CloudStorageAccount.Parse("");
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(Constants.BlobStorage.ApiDataBlobContainerName);

            var tableClient = storageAccount.CreateCloudTableClient();
            var tableContainer = tableClient.GetTableReference(Constants.BlobStorage.ApiDataTableContainerName);

            var service = new AzureBlobApiDataWriter(blobContainer, tableContainer);
            service.WriteTableStorage();

            //Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void テーブルインサートリプレイス()
        {
            var storageAccount = CloudStorageAccount.Parse("");
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(Constants.BlobStorage.ApiDataBlobContainerName);

            var tableClient = storageAccount.CreateCloudTableClient();
            var tableContainer = tableClient.GetTableReference(Constants.BlobStorage.ApiDataTableContainerName);

            var service = new AzureBlobApiDataWriter(blobContainer, tableContainer);
            service.InsertorReplaceTableStorage();

            //Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void findindextest1()
        {
            var apiDataArray = new ApiData[] {
                    new ApiData {
                        HttpDate = "2016/4/17 HttpDate",
                        LocalTime = "2016/4/17 LocalTime",
                        RequestBody = "艦これ艦これ艦これ艦これ艦これ艦こけ",
                        ResponseBody = "なし",
                        RequestUri = "http://125.6.187.205/kcsapi/api_get_member/unsetslot",
                        StatusCode = 0000,
                    },
                    new ApiData {
                        HttpDate = "2016/4/18 HttpDate",
                        LocalTime = "2016/4/18 LocalTime",
                        RequestBody = "艦これ艦これ艦これ艦これ艦これ艦こけ",
                        ResponseBody = "切り替わるぞ",
                        RequestUri = "http://125.6.187.205/kcsapi/api_port/port",
                        StatusCode = 0000,
                    },
                    new ApiData {
                        HttpDate = "2016/4/18 HttpDate",
                        LocalTime = "2016/4/18 LocalTime",
                        RequestBody = "艦これ艦これ艦これ艦これ艦これ艦こけ",
                        ResponseBody = "変わったにょ",
                        RequestUri = "http://125.6.187.205/kcsapi/api_req_mission/result",
                        StatusCode = 0000,
                    },
                    new ApiData {
                        HttpDate = "2016/4/18 HttpDate",
                        LocalTime = "2016/4/18 LocalTime",
                        RequestBody = "艦これ艦これ艦これ艦これ艦これ艦こけ",
                        ResponseBody = "変わったにょ",
                        RequestUri = "http://125.6.187.205/kcsapi/api_port/port",
                        StatusCode = 0000,
                    },
                };

            var storageAccount = CloudStorageAccount.Parse("");
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(Constants.BlobStorage.ApiDataBlobContainerName);

            var tableClient = storageAccount.CreateCloudTableClient();
            var tableContainer = tableClient.GetTableReference(Constants.BlobStorage.ApiDataTableContainerName);

            var service = new AzureBlobApiDataWriter(blobContainer, tableContainer);

            //int count = service.FindFirstApiIndexOf(apiDataArray, "/kcsapi/api_port/port");

            //Assert.AreEqual(1, count);

            //Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void findindextest2()
        {
            var apiDataArray = new ApiData[] {
                    new ApiData {
                        HttpDate = "2016/4/18 HttpDate",
                        LocalTime = "2016/4/18 LocalTime",
                        RequestBody = "艦これ艦これ艦これ艦これ艦これ艦こけ",
                        ResponseBody = "切り替わるぞ",
                        RequestUri = "http://125.6.187.205/kcsapi/api_port/port",
                        StatusCode = 0000,
                    },
                    new ApiData {
                        HttpDate = "2016/4/18 HttpDate",
                        LocalTime = "2016/4/18 LocalTime",
                        RequestBody = "艦これ艦これ艦これ艦これ艦これ艦こけ",
                        ResponseBody = "変わったにょ",
                        RequestUri = "http://125.6.187.205/kcsapi/api_req_mission/result",
                        StatusCode = 0000,
                    },
                    new ApiData {
                        HttpDate = "2016/4/18 HttpDate",
                        LocalTime = "2016/4/18 LocalTime",
                        RequestBody = "艦これ艦これ艦これ艦これ艦これ艦こけ",
                        ResponseBody = "変わったにょ",
                        RequestUri = "http://125.6.187.205/kcsapi/api_port/port",
                        StatusCode = 0000,
                    },
                };

            var storageAccount = CloudStorageAccount.Parse("");
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(Constants.BlobStorage.ApiDataBlobContainerName);

            var tableClient = storageAccount.CreateCloudTableClient();
            var tableContainer = tableClient.GetTableReference(Constants.BlobStorage.ApiDataTableContainerName);

            var service = new AzureBlobApiDataWriter(blobContainer, tableContainer);

            //int count = service.FindFirstApiIndexOf(apiDataArray, "/kcsapi/api_port/port");

            //Assert.AreEqual(0, count);

            //Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void findindextest3()
        {
            var apiDataArray = new ApiData[] {
                    new ApiData {
                        HttpDate = "2016/4/18 HttpDate",
                        LocalTime = "2016/4/18 LocalTime",
                        RequestBody = "艦これ艦これ艦これ艦これ艦これ艦こけ",
                        ResponseBody = "変わったにょ",
                        RequestUri = "http://125.6.187.205/kcsapi/api_req_mission/result",
                        StatusCode = 0000,
                    },
                    new ApiData {
                        HttpDate = "2016/4/18 HttpDate",
                        LocalTime = "2016/4/18 LocalTime",
                        RequestBody = "艦これ艦これ艦これ艦これ艦これ艦こけ",
                        ResponseBody = "変わったにょ",
                        RequestUri = "http://125.6.187.205/kcsapi/api_req_mission/result",
                        StatusCode = 0000,
                    },
                };

            var storageAccount = CloudStorageAccount.Parse("");
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(Constants.BlobStorage.ApiDataBlobContainerName);

            var tableClient = storageAccount.CreateCloudTableClient();
            var tableContainer = tableClient.GetTableReference(Constants.BlobStorage.ApiDataTableContainerName);

            var service = new AzureBlobApiDataWriter(blobContainer, tableContainer);

            //int count = service.FindFirstApiIndexOf(apiDataArray, "/kcsapi/api_port/port");

            //Assert.AreEqual(-1, count);

            //Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void findindextest4()
        {
            var apiDataArray = new ApiData[] {
                    new ApiData {
                        HttpDate = "2016/4/18 HttpDate",
                        LocalTime = "2016/4/18 LocalTime",
                        RequestBody = "艦これ艦これ艦これ艦これ艦これ艦こけ",
                        ResponseBody = "切り替わるぞ",
                        RequestUri = "http://125.6.187.205/kcsapi/api_port/port",
                        StatusCode = 0000,
                    },
                };

            var storageAccount = CloudStorageAccount.Parse("");
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(Constants.BlobStorage.ApiDataBlobContainerName);

            var tableClient = storageAccount.CreateCloudTableClient();
            var tableContainer = tableClient.GetTableReference(Constants.BlobStorage.ApiDataTableContainerName);

            var service = new AzureBlobApiDataWriter(blobContainer, tableContainer);

            //int count = service.FindFirstApiIndexOf(apiDataArray, "/kcsapi/api_port/port");

            //Assert.AreEqual(0, count);

            //Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void 日付オフセット()
        {
            DateTime now = DateTime.UtcNow.Add(Constants.BlobStorage.OffsetGMT);

            var date = now.Date.Add(Constants.BlobStorage.OffsetTime);
        }
    }
}
