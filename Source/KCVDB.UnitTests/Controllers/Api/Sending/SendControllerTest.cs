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

        #region テスト・テスト
        //[TestMethod]
        //public void apiPortで切り替えチェック日付変わる前その1()
        //{
        //    var agentId = "ほっぽっぽっぽっっぽ";
        //    var sessionId = "sessionId固定その1";
        //    var apiDataArray = new ApiData[] {
        //        new ApiData {
        //            HttpDate = "2016/4/17 HttpDate",
        //            LocalTime = "2016/4/17 LocalTime",
        //            RequestBody = "艦これ艦これ艦これ艦これ艦これ艦こけ",
        //            ResponseBody = "なし",
        //            RequestUri = "http://125.6.187.205/kcsapi/api_get_member/useitem",
        //            StatusCode = 0000,
        //        },
        //        new ApiData {
        //            HttpDate = "2016/4/17 HttpDate",
        //            LocalTime = "2016/4/17 LocalTime",
        //            RequestBody = "艦これ艦これ艦これ艦これ艦これ艦こけ",
        //            ResponseBody = "なし",
        //            RequestUri = "http://125.6.187.205/kcsapi/api_get_member/kdock",
        //            StatusCode = 0000,
        //        },
        //    };

        //    var apiDataWriterMock = new Mock<IApiDataWriter>();
        //    apiDataWriterMock
        //        .Setup(x => x.WriteAsync(
        //            It.Is<string>(value => value == agentId),
        //            It.Is<string>(value => value == sessionId),
        //            It.Is<ApiData[]>(value => Enumerable.SequenceEqual(value, apiDataArray, new ApiDataEqualityComparer())))
        //        );

        //    var parameter = new MultiPostParameter
        //    {
        //        AgentId = agentId,
        //        SessionId = sessionId,
        //        JsonArrayData = JsonConvert.SerializeObject(apiDataArray)
        //    };

        //    var controller = new SendController(apiDataWriterMock.Object)
        //    {
        //        Request = new System.Net.Http.HttpRequestMessage()
        //    };

        //    controller.PostTest(parameter);
        //}

        //[TestMethod]
        //public async Task apiPortで切り替えチェック日付変わる前その2()
        //{
        //    var agentId = "ほっぽっぽっぽっっぽ";
        //    var sessionId = "sessionId固定その2";
        //    var apiDataArray = new ApiData[] {
        //        new ApiData {
        //            HttpDate = "2016/4/17 HttpDate",
        //            LocalTime = "2016/4/17 LocalTime",
        //            RequestBody = "艦これ艦これ艦これ艦これ艦これ艦こけ",
        //            ResponseBody = "なし",
        //            RequestUri = "http://125.6.187.205/kcsapi/api_get_member/useitem",
        //            StatusCode = 0000,
        //        },
        //        new ApiData {
        //            HttpDate = "2016/4/17 HttpDate",
        //            LocalTime = "2016/4/17 LocalTime",
        //            RequestBody = "艦これ艦これ艦これ艦これ艦これ艦こけ",
        //            ResponseBody = "なし",
        //            RequestUri = "http://125.6.187.205/kcsapi/api_get_member/kdock",
        //            StatusCode = 0000,
        //        },
        //    };

        //    var apiDataWriterMock = new Mock<IApiDataWriter>();
        //    apiDataWriterMock
        //        .Setup(x => x.WriteAsync(
        //            It.Is<string>(value => value == agentId),
        //            It.Is<string>(value => value == sessionId),
        //            It.Is<ApiData[]>(value => Enumerable.SequenceEqual(value, apiDataArray, new ApiDataEqualityComparer())))
        //        )
        //        .Returns(Task.Delay(0));

        //    var parameter = new MultiPostParameter
        //    {
        //        AgentId = agentId,
        //        SessionId = sessionId,
        //        JsonArrayData = JsonConvert.SerializeObject(apiDataArray)
        //    };

        //    var controller = new SendController(apiDataWriterMock.Object)
        //    {
        //        Request = new System.Net.Http.HttpRequestMessage()
        //    };
        //    var result = await controller.MultiPostAsync(parameter);
        //    var ret = await result.ExecuteAsync(CancellationToken.None);
        //    Assert.AreEqual(HttpStatusCode.NoContent, ret.StatusCode);
        //}

        //[TestMethod]
        //public async Task apiPortで切り替えチェック日付変わった後その1()
        //{
        //    var agentId = "ほっぽっぽっぽっっぽ";
        //    var sessionId = "sessionId固定その1";
        //    var apiDataArray = new ApiData[] {
        //        new ApiData {
        //            HttpDate = "2016/4/17 HttpDate",
        //            LocalTime = "2016/4/17 LocalTime",
        //            RequestBody = "艦これ艦これ艦これ艦これ艦これ艦こけ",
        //            ResponseBody = "なし",
        //            RequestUri = "http://125.6.187.205/kcsapi/api_get_member/unsetslot",
        //            StatusCode = 0000,
        //        },
        //        new ApiData {
        //            HttpDate = "2016/4/18 HttpDate",
        //            LocalTime = "2016/4/18 LocalTime",
        //            RequestBody = "艦これ艦これ艦これ艦これ艦これ艦こけ",
        //            ResponseBody = "切り替わるぞ",
        //            RequestUri = "http://125.6.187.205/kcsapi/api_port/port",
        //            StatusCode = 0000,
        //        },
        //        new ApiData {
        //            HttpDate = "2016/4/18 HttpDate",
        //            LocalTime = "2016/4/18 LocalTime",
        //            RequestBody = "艦これ艦これ艦これ艦これ艦これ艦こけ",
        //            ResponseBody = "変わったにょ",
        //            RequestUri = "http://125.6.187.205/kcsapi/api_req_mission/result",
        //            StatusCode = 0000,
        //        },
        //        new ApiData {
        //            HttpDate = "2016/4/18 HttpDate",
        //            LocalTime = "2016/4/18 LocalTime",
        //            RequestBody = "艦これ艦これ艦これ艦これ艦これ艦こけ",
        //            ResponseBody = "変わったにょ",
        //            RequestUri = "http://125.6.187.205/kcsapi/api_port/port",
        //            StatusCode = 0000,
        //        },
        //    };

        //    var apiDataWriterMock = new Mock<IApiDataWriter>();
        //    apiDataWriterMock
        //        .Setup(x => x.WriteAsync(
        //            It.Is<string>(value => value == agentId),
        //            It.Is<string>(value => value == sessionId),
        //            It.Is<ApiData[]>(value => Enumerable.SequenceEqual(value, apiDataArray, new ApiDataEqualityComparer())))
        //        )
        //        .Returns(Task.Delay(0));

        //    var parameter = new MultiPostParameter
        //    {
        //        AgentId = agentId,
        //        SessionId = sessionId,
        //        JsonArrayData = JsonConvert.SerializeObject(apiDataArray)
        //    };

        //    var controller = new SendController(apiDataWriterMock.Object)
        //    {
        //        Request = new System.Net.Http.HttpRequestMessage()
        //    };
        //    var result = await controller.MultiPostAsync(parameter);
        //    var ret = await result.ExecuteAsync(CancellationToken.None);
        //    Assert.AreEqual(HttpStatusCode.NoContent, ret.StatusCode);
        //}

        //[TestMethod]
        //public async Task apiPortで切り替えチェック日付変わった後その2()
        //{
        //    var agentId = "ほっぽっぽっぽっっぽ";
        //    var sessionId = "sessionId固定その1";
        //    var apiDataArray = new ApiData[] {
        //        new ApiData {
        //            HttpDate = "2016/4/18 HttpDate",
        //            LocalTime = "2016/4/18 LocalTime",
        //            RequestBody = "艦これ艦これ艦これ艦これ艦これ艦こけ",
        //            ResponseBody = "切り替わるぞ",
        //            RequestUri = "http://125.6.187.205/kcsapi/api_port/port",
        //            StatusCode = 0000,
        //        },
        //    };

        //    var apiDataWriterMock = new Mock<IApiDataWriter>();
        //    apiDataWriterMock
        //        .Setup(x => x.WriteAsync(
        //            It.Is<string>(value => value == agentId),
        //            It.Is<string>(value => value == sessionId),
        //            It.Is<ApiData[]>(value => Enumerable.SequenceEqual(value, apiDataArray, new ApiDataEqualityComparer())))
        //        )
        //        .Returns(Task.Delay(0));

        //    var parameter = new MultiPostParameter
        //    {
        //        AgentId = agentId,
        //        SessionId = sessionId,
        //        JsonArrayData = JsonConvert.SerializeObject(apiDataArray)
        //    };

        //    var controller = new SendController(apiDataWriterMock.Object)
        //    {
        //        Request = new System.Net.Http.HttpRequestMessage()
        //    };
        //    var result = await controller.MultiPostAsync(parameter);
        //    var ret = await result.ExecuteAsync(CancellationToken.None);
        //    Assert.AreEqual(HttpStatusCode.NoContent, ret.StatusCode);
        //}
        #endregion

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
