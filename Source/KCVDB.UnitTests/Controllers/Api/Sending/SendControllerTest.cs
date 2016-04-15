using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KCVDB.Controllers.Api.Sending;
using KCVDB.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;

namespace KCVDB.UnitTests.Controllers.Api.Sending
{
	[TestClass]
	public class SendControllerTest
	{
		[TestMethod]
		public async Task MultiPostTest()
		{
			var agentId = Guid.NewGuid().ToString();
			var sessionId = Guid.NewGuid().ToString();
			var apiDataArray = new ApiData[] {
				new ApiData {
					HttpDate = "unko",
					LocalTime = "chinko\nmanko",
					RequestBody = "chinchin \r\n manman",
					ResponseBody = "chimpoko",
					RequestUri = "manko",
					StatusCode = 0721,
				},
				new ApiData {
					HttpDate = "unko",
					LocalTime = "chinko\nmanko",
					RequestBody = "chinchin \r\n manman",
					ResponseBody = "chimpoko",
					RequestUri = "manko",
					StatusCode = 0721,
				},
			};

			var apiDataWriterMock = new Mock<IApiDataWriter>();
			apiDataWriterMock
				.Setup(x => x.WriteAsync(
					It.Is<string>(value => value == agentId),
					It.Is<string>(value => value == sessionId),
					It.Is<ApiData[]>( value => Enumerable.SequenceEqual(value, apiDataArray, new ApiDataEqualityComparer())))
				)
				.Returns(Task.Delay(0));

			var parameter = new MultiPostParameter {
				AgentId = agentId,
				SessionId = sessionId,
				JsonArrayData = JsonConvert.SerializeObject(apiDataArray)
			};
			
			var controller = new SendController(apiDataWriterMock.Object) {
				Request = new System.Net.Http.HttpRequestMessage()
			};
			var result = await controller.MultiPostAsync(parameter);
			var ret = await result.ExecuteAsync(CancellationToken.None);
			Assert.AreEqual(HttpStatusCode.NoContent, ret.StatusCode);
		}

		class ApiDataEqualityComparer : IEqualityComparer<ApiData>
		{
			public bool Equals(ApiData x, ApiData y)
			{
				if (x == null && y == null) {
					return true;
				}
				else if (x == null || y == null) {
					return false;
				}
				else {
					return (
						x.RequestUri == y.RequestUri &&
						x.RequestBody == y.RequestBody &&
						x.ResponseBody == y.ResponseBody &&
						x.HttpDate == y.HttpDate &&
						x.LocalTime == y.LocalTime &&
						x.StatusCode == y.StatusCode);
				}
			}

			public int GetHashCode(ApiData obj)
			{
				return (
					obj.RequestUri?.GetHashCode() ?? 0 ^
					obj.RequestBody?.GetHashCode() ?? 0 ^
					obj.ResponseBody?.GetHashCode() ?? 0 ^
					obj.HttpDate?.GetHashCode() ?? 0 ^
					obj.LocalTime?.GetHashCode() ?? 0 ^
					obj.StatusCode?.GetHashCode() ?? 0);


			}
		}
	}
}
