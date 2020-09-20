using System;
using System.Text;

namespace JobeSharp.Sandbox
{
    public class ExecuteOptions
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
        public string StdIn { get; set; }
        public int StreamSizeKb { get; set; }
        public string OutExitFile { get; set; }
        public string OutTimeFile { get; set; }
        public string WorkingDirectory { get; set; }
        
        public string ToArgumentsString()
        {
            var arguments = new StringBuilder();

            ExecuteIfNotDefault(Root, () => arguments.Append($" --root={Root}"));
            ExecuteIfNotDefault(User, () => arguments.Append($" --user={User}"));
            ExecuteIfNotDefault(Group, () => arguments.Append($" --group={Group}"));
            ExecuteIfNotDefault(TimeSeconds, () => arguments.Append($" --time={TimeSeconds}"));
            ExecuteIfNotDefault(CpuTimeSeconds, () => arguments.Append($" --cputime={CpuTimeSeconds}"));
            ExecuteIfNotDefault(TotalMemoryKb, () => arguments.Append($" --memsize={TotalMemoryKb}"));
            ExecuteIfNotDefault(FileSizeKb, () => arguments.Append($" --filesize={FileSizeKb}"));
            ExecuteIfNotDefault(NumberOfProcesses, () => arguments.Append($" --nproc={NumberOfProcesses}"));
            ExecuteIfNotDefault(CpuSet, () => arguments.Append($" --cpuset={CpuSet}"));
            ExecuteIfNotDefault(NoCoreDumps, () => arguments.Append(" --no-core"));
            ExecuteIfNotDefault(StdOutFile, () => arguments.Append($" --stdout={StdOutFile}"));
            ExecuteIfNotDefault(StdErrFile, () => arguments.Append($" --stderr={StdErrFile}"));
            ExecuteIfNotDefault(StreamSizeKb, () => arguments.Append($" --streamsize={StreamSizeKb}"));
            ExecuteIfNotDefault(OutExitFile, () => arguments.Append($" --outexit={OutExitFile}"));
            ExecuteIfNotDefault(OutTimeFile, () => arguments.Append($" --outtime={OutTimeFile}"));

            return arguments.ToString();
        }

        public void MergeIntoCurrent(ExecuteOptions other)
        {
            ExecuteIfNotDefault(other.Root, () => Root = other.Root);
            ExecuteIfNotDefault(other.User, () => User = other.User);
            ExecuteIfNotDefault(other.Group, () => Group = other.Group);
            ExecuteIfNotDefault(other.TimeSeconds, () => TimeSeconds = other.TimeSeconds);
            ExecuteIfNotDefault(other.CpuTimeSeconds, () => CpuTimeSeconds = other.CpuTimeSeconds);
            ExecuteIfNotDefault(other.TotalMemoryKb, () => TotalMemoryKb = other.TotalMemoryKb);
            ExecuteIfNotDefault(other.FileSizeKb, () => FileSizeKb = other.FileSizeKb);
            ExecuteIfNotDefault(other.NumberOfProcesses, () => NumberOfProcesses = other.NumberOfProcesses);
            ExecuteIfNotDefault(other.CpuSet, () => CpuSet = other.CpuSet);
            ExecuteIfNotDefault(other.NoCoreDumps, () => NoCoreDumps = other.NoCoreDumps);
            ExecuteIfNotDefault(other.StdOutFile, () => StdOutFile = other.StdOutFile);
            ExecuteIfNotDefault(other.StdErrFile, () => StdErrFile = other.StdErrFile);
            ExecuteIfNotDefault(other.StreamSizeKb, () => StreamSizeKb = other.StreamSizeKb);
            ExecuteIfNotDefault(other.OutExitFile, () => OutExitFile = other.OutExitFile);
            ExecuteIfNotDefault(other.OutTimeFile, () => OutTimeFile = other.OutTimeFile);
        }

        private void ExecuteIfNotDefault(object checkValue, Action action)
        {
            switch (checkValue)
            {
                case string stringValue when !string.IsNullOrEmpty(stringValue):
                case int intValue when intValue > 0:
                case double doubleValue when Math.Abs(doubleValue) > 0.0001:
                case bool boolValue when boolValue:
                    action();
                    return;
            }
        }
    }
}