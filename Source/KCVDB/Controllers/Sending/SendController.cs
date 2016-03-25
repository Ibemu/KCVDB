using System.Threading.Tasks;
using System.Web.Http;
using KCVDB.Services;

namespace KCVDB.Controllers.Sending
{
	[RoutePrefix("send")]
    public class SendController : ApiController
	{
		private AzureBlobService _blobService;

		public SendController()
		{
			_blobService = new AzureBlobService();
		}
		
		[HttpPost]
		public async Task Post([FromBody]KancolleApiSendParameter model)
		{
			await _blobService.LogApiDataAsync(
				model.LoginSessionId,
				model.Path,
				model.RequestValue,
				model.ResponseValue,
				model.AgentId,
				model.StatusCode,
				model.HttpDate,
				model.LocalTime);
		}
	}
}
