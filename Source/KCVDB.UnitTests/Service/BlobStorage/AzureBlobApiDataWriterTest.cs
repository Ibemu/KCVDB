﻿using System;
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
            var sessionId = "送信セッション";
            var apiData = new ApiData
            {
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
                "manko",    // RequestUri
                "721",      // StatusCode
                "unko",     // HttpDate
                "chinkomanko",      // LocalTime
                "chinchin  manman", // RequestBody
                "chimpoko"          // ResponseBody
            );

            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=kancollevdb;AccountKey=VwjCnYOYYL/5mF+CKYu4evctVmr6tMrjI4/d8gIPZt+JCGhUyDzhXqZB8rBj9U6xdOk+YlFrVuD7ZdmUkaWKIA==;BlobEndpoint=https://kancollevdb.blob.core.windows.net/;TableEndpoint=https://kancollevdb.table.core.windows.net/;QueueEndpoint=https://kancollevdb.queue.core.windows.net/;FileEndpoint=https://kancollevdb.file.core.windows.net/");
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(Constants.BlobStorage.ApiDataBlobContainerName);

            var tableClient = storageAccount.CreateCloudTableClient();
            var tableContainer = tableClient.GetTableReference(Constants.BlobStorage.ApiDataTableContainerName);

            //var po = new PrivateObject(typeof(AzureBlobApiDataWriter), blobContainer, tableContainer);
            //po.Invoke("WriteAsync", agentId, sessionId, apiData);
            var service = new AzureBlobApiDataWriter(blobContainer, tableContainer);
            service.WriteAsync(agentId, sessionId, apiData);

            //Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void 一覧取得()
        {
            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=kancollevdb;AccountKey=VwjCnYOYYL/5mF+CKYu4evctVmr6tMrjI4/d8gIPZt+JCGhUyDzhXqZB8rBj9U6xdOk+YlFrVuD7ZdmUkaWKIA==;BlobEndpoint=https://kancollevdb.blob.core.windows.net/;TableEndpoint=https://kancollevdb.table.core.windows.net/;QueueEndpoint=https://kancollevdb.queue.core.windows.net/;FileEndpoint=https://kancollevdb.file.core.windows.net/");
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
            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=kancollevdb;AccountKey=VwjCnYOYYL/5mF+CKYu4evctVmr6tMrjI4/d8gIPZt+JCGhUyDzhXqZB8rBj9U6xdOk+YlFrVuD7ZdmUkaWKIA==;BlobEndpoint=https://kancollevdb.blob.core.windows.net/;TableEndpoint=https://kancollevdb.table.core.windows.net/;QueueEndpoint=https://kancollevdb.queue.core.windows.net/;FileEndpoint=https://kancollevdb.file.core.windows.net/");
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
            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=kancollevdb;AccountKey=VwjCnYOYYL/5mF+CKYu4evctVmr6tMrjI4/d8gIPZt+JCGhUyDzhXqZB8rBj9U6xdOk+YlFrVuD7ZdmUkaWKIA==;BlobEndpoint=https://kancollevdb.blob.core.windows.net/;TableEndpoint=https://kancollevdb.table.core.windows.net/;QueueEndpoint=https://kancollevdb.queue.core.windows.net/;FileEndpoint=https://kancollevdb.file.core.windows.net/");
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
            var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=kancollevdb;AccountKey=VwjCnYOYYL/5mF+CKYu4evctVmr6tMrjI4/d8gIPZt+JCGhUyDzhXqZB8rBj9U6xdOk+YlFrVuD7ZdmUkaWKIA==;BlobEndpoint=https://kancollevdb.blob.core.windows.net/;TableEndpoint=https://kancollevdb.table.core.windows.net/;QueueEndpoint=https://kancollevdb.queue.core.windows.net/;FileEndpoint=https://kancollevdb.file.core.windows.net/");
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(Constants.BlobStorage.ApiDataBlobContainerName);

            var tableClient = storageAccount.CreateCloudTableClient();
            var tableContainer = tableClient.GetTableReference(Constants.BlobStorage.ApiDataTableContainerName);

            var service = new AzureBlobApiDataWriter(blobContainer, tableContainer);
            service.InsertorReplaceTableStorage();

            //Assert.AreEqual(expected, actual);
        }
    }
}
