using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SystemConsole = System.Console;

namespace SharpSyslogServer.Console
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var source = new CancellationTokenSource();
            var sysLog = new UdpSyslogServer(new ConsoleSyslogMessageHandler());

            try
            {
                var sysLogTask = Task.Run(() => sysLog.Start(source.Token), source.Token);

                SystemConsole.WriteLine("Press ESC to exit");

                do
                {
                    while (true)
                    {
                        if (sysLogTask.IsFaulted)
                            sysLogTask.Wait(); // throw syslogServer errors

                        if (!SystemConsole.KeyAvailable)
                            break;

                        Task.Delay(1000).Wait();
                    }

                } while (SystemConsole.ReadKey(true).Key != ConsoleKey.Escape);

                return 0;
            }
            catch (AggregateException aggEx)
            {
                if (aggEx.InnerExceptions.Any(p => p is OperationCanceledException))
                    return 0;
                throw;
            }
        }

    }
}
