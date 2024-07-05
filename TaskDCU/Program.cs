using ASI.Lib.Config;
using ASI.Lib.Process;
using ASI.Wanda.DMD.TaskDCU;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Wanda.DMD.TaskDCU
{
    class Program
    {
        static void Main(string[] args)
        {
            string qname = "TaskDCU";
            if (args.Length == 1) qname = args[0];

            IProcess TheProc = new ProcTaskDCU();
            //if (TheProc.StartTask(System.Environment.MachineName, qname) >= 0)
            if (TheProc.StartTask(ConfigApp.Instance.HostName, qname) >= 0)
            {
                TheProc.Run();
            }
            TheProc.StopTask();
        }
    }
}
