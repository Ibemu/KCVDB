using System;
using KCVDB.Services;
using KCVDB.Services.BlobStorage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage.Blob;
using Moq;

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
			var po = new PrivateObject(typeof(AzureBlobApiDataWriter), containerMock.Object);
			var actual = (string)po.Invoke("SerializeApiData", agentId, sessionId, apiData);

			Assert.AreEqual(expected, actual);
		}
	}
}
