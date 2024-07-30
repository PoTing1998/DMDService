using System;
using System.Collections.Generic;
using System.Text;
using ASI.Lib.Process;
using ASI.Lib.Config;

namespace ASI.Wanda.DMD.TaskKernel
{
	 class  Program
	{
		static void Main(string[] args)
		{
			//if (Environment.UserInteractive)
			//{

				string qname = "TaskKernel";
				if (args.Length == 1) qname = args[0];

				IProcess TheProc = new ProcKernel();
				//if (TheProc.StartTask(System.Environment.MachineName, qname) >= 0)
				if (TheProc.StartTask(ConfigApp.Instance.HostName, qname) >= 0)
				{
                    Console.WriteLine("Service on");
                    Console.ReadLine();
                    TheProc.Run();
				}

				TheProc.StopTask();
           // }
        }
	}
}
