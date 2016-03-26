using System.ComponentModel.DataAnnotations;

namespace KCVDB.Controllers.Sending
{
	public class KancolleApiSendParameter
	{
		public string LoginSessionId { get; set; }

		public string AgentId { get; set; }

		[Required(AllowEmptyStrings = false)]
		public string Path { get; set; }

		[Required(AllowEmptyStrings = true)]
		public string RequestValue { get; set; }

		[Required(AllowEmptyStrings = false)]
		public string ResponseValue { get; set; }

		public int? StatusCode { get; set; }

		public string HttpDate { get; set; }

		public string LocalTime { get; set; }
	}
}
