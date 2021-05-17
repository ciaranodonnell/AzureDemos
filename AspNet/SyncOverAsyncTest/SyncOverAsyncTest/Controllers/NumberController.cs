using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyncOverAsyncTest.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class NumberController : ControllerBase
	{
		private readonly ICommunicationService messagingService;
		private readonly ILogger<NumberController> _logger;

		public NumberController(ILogger<NumberController> logger,ICommunicationService messagingService)
		{
			this.messagingService = messagingService;
			_logger = logger;
		}

		[HttpGet]
		public async Task<int> Get()
		{
			var getNumberResult = await messagingService.DoAsyncWork(Guid.NewGuid());
			return getNumberResult.Number;
		}

		class TaskContext
		{
			public Task OriginalTask;
			public Action Continuation;
		}

	}
}
