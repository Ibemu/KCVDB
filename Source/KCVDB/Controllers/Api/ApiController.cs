using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace KCVDB.Controllers.Api
{
    public class ApiControllerBase : ApiController
	{		
		protected HttpResponseResult HttpResponseResult(HttpResponseMessage message)
		{
			return new HttpResponseResult(message);
		}

		protected HttpResponseResult Forbidden(string message)
		{
			return new HttpResponseResult(Request.CreateErrorResponse(HttpStatusCode.Forbidden, message));
		}

		protected HttpResponseResult NoContent()
		{
			return new HttpResponseResult(Request.CreateResponse(HttpStatusCode.NoContent));
		}

		protected HttpResponseResult UnsupportedMediaType()
		{
			return new HttpResponseResult(Request.CreateResponse(HttpStatusCode.UnsupportedMediaType));
		}
	}
}
