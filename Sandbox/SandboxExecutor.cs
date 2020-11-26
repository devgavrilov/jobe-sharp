using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace JobeSharp.Sandbox
{
    internal static class SandboxExecutor
    {
        public static ExecutionResult Execute(string command, ExecuteOptions executeOptions = null)
        {
            if (string.IsNullOrEmpty(command))
            {
                throw new ArgumentException(nameof(command));
            }

            var workingDirectory = executeOptions?.WorkingDirectory ?? Environment.CurrentDirectory;
            
            var parsedCommandData = command.Split(" ").ToList();
            var commandFilePath = TryGetFileFromPath(workingDirectory, parsedCommandData.First());
            var arguments = string.Join(" ", parsedCommandData.Skip(1));
            
            File.WriteAllText(Path.Join(workingDirectory, "prog.cmd"), $"{commandFilePath} {arguments}");
            
            File.WriteAllText(Path.Join(workingDirectory, "sandbox.cmd"), $"{Environment.CurrentDirectory}/Sandbox/RunGuard/runguard {executeOptions?.ToArgumentsString()} {TryGetFileFromPath(workingDirectory, "bash")} {Path.Join(workingDirectory, "prog.cmd")} >{Path.Join(workingDirectory, "prog.out")} 2>{Path.Join(workingDirectory, "prog.err")} <{Path.Join(workingDirectory, "prog.in")}");
            
            File.WriteAllText(Path.Join(workingDirectory, "prog.in"), executeOptions?.StdIn ?? "");
            
            using var process = Process.Start(new ProcessStartInfo
            {
                FileName = TryGetFileFromPath(workingDirectory, "bash"),
                Arguments = Path.Join(workingDirectory, "sandbox.cmd"),
                WorkingDirectory = workingDirectory
            });
            
            process?.WaitForExit();

            var result = new ExecutionResult(
                exitCode: process?.ExitCode ?? 0, 
                output: File.ReadAllText(Path.Join(workingDirectory, "prog.out")),
                error: File.ReadAllText(Path.Join(workingDirectory, "prog.err"))
            );
            
            process?.Close();

            return result;
        }

        private static string TryGetFileFromPath(string workingDirectory, string fileName)
        {
            if (File.Exists(Path.Combine(workingDirectory, fileName)))
            {
                return Path.Combine(workingDirectory, fileName);
            }
            
            var environmentPath = Environment.GetEnvironmentVariable("PATH");
            if (string.IsNullOrEmpty(environmentPath))
            {
                return Path.Combine(workingDirectory, fileName);
            }
            
            var paths = environmentPath.Split(Path.PathSeparator);
            
            var exePath = paths
                .Select(x => Path.Combine(x, fileName))
                .FirstOrDefault(File.Exists);
            
            return exePath ?? Path.Combine(workingDirectory, fileName);
        }
    }
}