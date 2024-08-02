
namespace ASI.Lib.Process
{
	public interface IProcess
	{
		/// 
		/// <param name="pLabel"></param>
		/// <param name="pBody"></param>
		int ProcEvent(string pLabel, string pBody);

		/// 
		/// <param name="pMessage"></param>
		int ProcTimerEvent(string pMessage);

		/// 
		/// <param name="pProcName"></param>
		int StartTask(string pComputer, string pProcName);

		void StopTask();
		void Run();
	}
}
