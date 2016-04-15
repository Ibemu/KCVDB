using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace KCVDB.Controllers.Api
{
	public class HttpResponseResult : IHttpActionResult
	{
		public HttpResponseMessage Message { get; }

		public HttpResponseResult(HttpResponseMessage message)
		{
			if (message == null) { throw new ArgumentNullException("message"); }
			Message = message;
		}

		public Task<HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken)
		{
			return Task.FromResult(Message);
		}
	}
}