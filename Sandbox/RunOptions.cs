using System;
using System.Text;

namespace JobeSharp.Sandbox
{
    internal class RunOptions
    {
        public string Root { get; set; }
        public string User { get; set; }
        public string Group { get; set; }
        public double TimeSeconds { get; set; }
        public double CpuTimeSeconds { get; set; }
        public int TotalMemoryKb { get; set; }
        public int FileSizeKb { get; set; }
        public int NumberOfProcesses { get; set; }
        public string CpuSet { get; set; }
        public bool NoCoreDumps { get; set; }
        public string StdOutFile { get; set; }
        public string StdErrFile { get; set; }
        public int StreamSizeKb { get; set; }
        public string OutExitFile { get; set; }
        public string OutTimeFile { get; set; }
        public string WorkingDirectory { get; set; }
        
        public string ToArgumentsString()
        {
            var argumentsStringBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(Root))
            {
                argumentsStringBuilder.Append($" --root={Root}");
            }

            if (!string.IsNullOrEmpty(User))
            {
                argumentsStringBuilder.Append($" --user={User}");
            }

            if (!string.IsNullOrEmpty(Group))
            {
                argumentsStringBuilder.Append($" --group={Group}");
            }
            
            if (Math.Abs(TimeSeconds) > 0.0001)
            {
                argumentsStringBuilder.Append($" --time={TimeSeconds}");
            }
            
            if (Math.Abs(CpuTimeSeconds) > 0.0001)
            {
                argumentsStringBuilder.Append($" --cputime={CpuTimeSeconds}");
            }

            if (TotalMemoryKb > 0)
            {
                argumentsStringBuilder.Append($" --memsize={TotalMemoryKb}");
            }

            if (FileSizeKb > 0)
            {
                argumentsStringBuilder.Append($" --filesize={FileSizeKb}");
            }

            if (NumberOfProcesses > 0)
            {
                argumentsStringBuilder.Append($" --nproc={NumberOfProcesses}");
            }

            if (!string.IsNullOrEmpty(CpuSet))
            {
                argumentsStringBuilder.Append($" --cpuset={CpuSet}");
            }

            if (NoCoreDumps)
            {
                argumentsStringBuilder.Append(" --no-core");
            }

            if (!string.IsNullOrEmpty(StdOutFile))
            {
                argumentsStringBuilder.Append($" --stdout={StdOutFile}");
            }

            if (!string.IsNullOrEmpty(StdErrFile))
            {
                argumentsStringBuilder.Append($" --stderr={StdErrFile}");
            }

            if (StreamSizeKb > 0)
            {
                argumentsStringBuilder.Append($" --streamsize={StreamSizeKb}");
            }

            if (!string.IsNullOrEmpty(OutExitFile))
            {
                argumentsStringBuilder.Append($" --outexit={OutExitFile}");
            }

            if (!string.IsNullOrEmpty(OutTimeFile))
            {
                argumentsStringBuilder.Append($" --outtime={OutTimeFile}");
            }

            return argumentsStringBuilder.ToString();
        }
    }
}