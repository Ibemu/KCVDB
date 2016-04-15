using System;
using System.Threading.Tasks;
using System.Web.Http;
using KCVDB.Services;
using Newtonsoft.Json;

namespace KCVDB.Controllers.Api.Sending
{
	[RoutePrefix("send")]
	public class SendController : ApiControllerBase
	{
		IApiDataWriter ApiDataWriter { get; }

		public SendController(IApiDataWriter apiDataWriter)
		{
			if (apiDataWriter == null) { throw new ArgumentNullException(nameof(apiDataWriter)); }
			ApiDataWriter = apiDataWriter;
		}

		[HttpPost]
		public async Task<IHttpActionResult> PostAsync([FromBody]KancolleApiSendParameter model)
		{
			var agentId = model.AgentId;
			var sessionId = model.LoginSessionId;
			var apiData = new ApiData {
				RequestUri = model.Path,
				ResponseBody = model.ResponseValue,
				RequestBody = model.RequestValue,
				HttpDate = model.HttpDate,
				LocalTime = model.LocalTime,
				StatusCode = model.StatusCode
			};

			await ApiDataWriter.WriteAsync(agentId, sessionId, apiData);

			return NoContent();
		}

		[Route("multi")]
		[HttpPost]
		public async Task<IHttpActionResult> MultiPostAsync([FromBody]MultiPostParameter parameter)
		{
			var apiDataArray = JsonConvert.DeserializeObject<ApiData[]>(parameter.JsonArrayData);
			await ApiDataWriter.WriteAsync(
				parameter.AgentId,
				parameter.SessionId,
				apiDataArray);

			return NoContent();
		}
	}
}
