namespace JobeSharp.Languages
{
    public abstract class ExecutionResult
    {
        public bool IsSuccess => ExitCode == 0;
        public int ExitCode { get; }
        public string Output { get; }
        public string Error { get; }

        protected ExecutionResult(Sandbox.ExecutionResult executionResult)
            : this(executionResult.ExitCode, executionResult.Output, executionResult.Error)
        {
            
        }
        
        protected ExecutionResult(int exitCode, string output, string error)
        {
            ExitCode = exitCode;
            Output = output;
            Error = error;
        }
    }

    internal class RunExecutionResult : ExecutionResult
    {
        public RunExecutionResult(Sandbox.ExecutionResult executionResult) : base(executionResult)
        {
        }

        public RunExecutionResult(int exitCode, string output, string error) : base(exitCode, output, error)
        {
        }
    }

    internal class CompileExecutionResult : ExecutionResult
    {
        public CompileExecutionResult(Sandbox.ExecutionResult executionResult) : base(executionResult)
        {
        }

        public CompileExecutionResult(int exitCode, string output, string error) : base(exitCode, output, error)
        {
        }
    }
}