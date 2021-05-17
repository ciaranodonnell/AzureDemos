
using Common;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundNumberGenerator
{
	class Program
	{
		static async Task Main(string[] args)
		{
			Console.WriteLine("Hello World!");

			MessagingService s = new MessagingService(new MessagingConfig
			{
				ConnectionString = "Endpoint=sb://ciaransyoutubedemos.servicebus.windows.net/;SharedAccessKeyName=SendDemo;SharedAccessKey=YTx4cOCZrec/H+kalXfwuW6H4jkBpjPMfUAk9LrIC0c=",
				ReplyTopic = "replyTopic",
				RequestTopic = "requestTopic",
				RequestSubscription = "requestSubscription"
			});

			Console.CancelKeyPress += Console_CancelKeyPress;

			await Task.Delay(Convert.ToInt32(TimeSpan.FromHours(1).TotalMilliseconds), cancel.Token);


		}
		static CancellationTokenSource cancel = new CancellationTokenSource();
		private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
		{
			cancel.Cancel();
		}
	}
}
