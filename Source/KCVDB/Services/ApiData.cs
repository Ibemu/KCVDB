namespace KCVDB.Services
{
	public class ApiData
	{
		public string RequestUri { get; set; }
		public string RequestBody { get; set; }
		public string ResponseBody { get; set; }
		public int? StatusCode { get; set; }
		public string HttpDate { get; set; }
		public string LocalTime { get; set; }
	}
}