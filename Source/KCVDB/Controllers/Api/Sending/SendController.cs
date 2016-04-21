using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using KCVDB.Services;
using Newtonsoft.Json;

namespace KCVDB.Controllers.Api.Sending
{
	[RoutePrefix("api/send")]
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

		[Route("gzip")]
		[HttpPost]
		public async Task<IHttpActionResult> PostGzipAsync()
		{
			if (!Request.Content.IsMimeMultipartContent()) {
				return UnsupportedMediaType();
			}

			var provider = await Request.Content.ReadAsMultipartAsync();
			var metadataPart = provider.Contents.FirstOrDefault(x => x.Headers.ContentDisposition.Name == "metadata");
			var bodyPart = provider.Contents.FirstOrDefault(x => x.Headers.ContentDisposition.Name == "body");

			if (metadataPart == null || bodyPart == null) {
				return BadRequest();
			}

			var metadataJson = await metadataPart.ReadAsStringAsync();
			var bodyJson = await DecompressToStringAsync(await bodyPart.ReadAsByteArrayAsync());

			var metadata = JsonConvert.DeserializeObject<PostGzipMetadata>(metadataJson);
			var apiDataArray = JsonConvert.DeserializeObject<ApiData[]>(bodyJson);
			await ApiDataWriter.WriteAsync(
				metadata.AgentId,
				metadata.SessionId,
				apiDataArray);

			return NoContent();
		}

        //public void PostTest(MultiPostParameter model)
        //{
        //    int i = 0;
        //    i++;
        //    var apiDataArray = JsonConvert.DeserializeObject<ApiData[]>(model.JsonArrayData);
        //    ApiDataWriter.WriteAsync(
        //        model.AgentId,
        //        model.SessionId,
        //        apiDataArray);
        //}


        async Task<string> DecompressToStringAsync(byte[] compressedBuffer) {
			using (var compressedMemoryStream = new MemoryStream(compressedBuffer))
			using (var gzipStream = new GZipStream(compressedMemoryStream, CompressionMode.Decompress))
			using (var decompressedMemoryStream = new MemoryStream()) {
				await gzipStream.CopyToAsync(decompressedMemoryStream);
				return Encoding.UTF8.GetString(decompressedMemoryStream.ToArray());
			}
		}

		class PostGzipMetadata
		{
			public string SessionId { get; set; }
			
			public string AgentId { get; set; }
		}
	}
}
