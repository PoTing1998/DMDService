using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;





namespace ASI.Wanda.DMD
{
    static class Program
    {

        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        static void Main()
        {
            //if (Environment.UserInteractive)
            //{
            //    KernelService s = new KernelService();
            //    s.Start(null);
            //    Console.WriteLine("Service on");
            //    Console.ReadLine();
            //   //  s.Stop();
            //  //  Console.WriteLine("Service off");
            //}
            //else
            //{
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new KernelService()
                };
                ServiceBase.Run(ServicesToRun);
            //}

           

        }


    }


}
