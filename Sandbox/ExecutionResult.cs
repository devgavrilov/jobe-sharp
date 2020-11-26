namespace JobeSharp.Sandbox
{
    public class ExecutionResult
    {
        public int ExitCode { get; }
        public string Output { get; }
        public string Error { get; }
        
        public ExecutionResult(int exitCode, string output, string error)
        {
            ExitCode = exitCode;
            Output = output;
            Error = error;
        }
    }
}