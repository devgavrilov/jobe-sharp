using System;

namespace JobeSharp.Model
{
    public enum RunState
    {
        InQueue,
        Processing,
        Completed
    }
    
    public class Run
    {
        public int Id { get; set; }
        public RunState State { get; set; } = RunState.InQueue;
        public string JobId { get; set; }
        public string SerializedTask { get; set; }
        public string LanguageName { get; set; }
        public string SerializedExecutionResult { get; set; }
        public DateTime CreationDateTimeUtc { get; set; } = DateTime.UtcNow;
    }
}