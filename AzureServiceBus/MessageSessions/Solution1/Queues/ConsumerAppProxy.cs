using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CompetingConsumersQueues
{
	class ConsumerAppProxy
	{
		public static IDisposable StartConsumerApp(string connectionString, string queueName, object id)
		{
			ProcessStartInfo psi = new ProcessStartInfo("ConsumerApp.exe", string.Join(' ', connectionString, queueName, id.ToString()))
			{
				CreateNoWindow = false,
				WindowStyle = ProcessWindowStyle.Normal,
				UseShellExecute = true,
			};
			var process = Process.Start(psi);


			
		
			return new ProcessKiller(process);
		}

		class ProcessKiller : IDisposable
		{
			private Process p;

			public ProcessKiller(Process p) { this.p = p; }

			public void Dispose()
			{
				p.Kill(true);
			}
		
		}

	
	
		public void IgnoreThisMethod()
		{
			//ignore this variable
			var type = typeof(ConsumerApp.Program);
		}
	}
}
