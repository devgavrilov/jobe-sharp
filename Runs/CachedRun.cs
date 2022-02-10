using JobeSharp.DTO;
using JobeSharp.Model;

namespace JobeSharp.Runs
{
	public class CachedRun
	{
		public RunState State { get; set; } = RunState.InQueue;
		public ResultDto Result { get; set; }
	}
}