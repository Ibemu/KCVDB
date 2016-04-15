using System.Collections.Generic;
using System.Threading.Tasks;

namespace KCVDB.Services
{
	public interface IApiDataWriter
	{
		Task WriteAsync(
			string agentId,
			string sessionId,
			ApiData apiData);

		Task WriteAsync(
			string agentId,
			string sessionId,
			IEnumerable<ApiData> apiData);
	}
}
