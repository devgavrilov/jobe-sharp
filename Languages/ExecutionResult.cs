using System.Diagnostics;

namespace JobeSharp.Languages
{
    internal abstract class ExecutionResult
    {
        public bool IsSuccess => ExitCode == 0;
        public int ExitCode { get; }
        public string Output { get; }
        public string Error { get; }

        protected ExecutionResult(Process process)
            : this(process.ExitCode, process.StandardOutput.ReadToEnd(), process.StandardError.ReadToEnd())
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
        public RunExecutionResult(Process process) : base(process)
        {
        }

        public RunExecutionResult(int exitCode, string output, string error) : base(exitCode, output, error)
        {
        }
    }

    internal class CompileExecutionResult : ExecutionResult
    {
        public CompileExecutionResult(Process process) : base(process)
        {
        }

        public CompileExecutionResult(int exitCode, string output, string error) : base(exitCode, output, error)
        {
        }
    }
}