using System.ComponentModel.DataAnnotations;

namespace KCVDB.Controllers.Api.Sending
{
	public class MultiPostParameter
	{
		[Required(AllowEmptyStrings = false)]
		public string AgentId { get; set; }

		[Required(AllowEmptyStrings = false)]
		public string SessionId { get; set; }

		[Required(AllowEmptyStrings = false)]
		public string JsonArrayData { get; set; }
	}
}