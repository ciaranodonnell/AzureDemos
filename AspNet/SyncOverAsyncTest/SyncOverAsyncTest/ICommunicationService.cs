using System;
using System.Linq;
using System.Threading.Tasks;

namespace SyncOverAsyncTest
{
	public interface ICommunicationService
	{
		Task<GetNumberResult> DoAsyncWork(Guid Id);
	}
}