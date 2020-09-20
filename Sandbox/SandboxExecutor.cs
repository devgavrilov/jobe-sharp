using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace JobeSharp.Sandbox
{
    internal static class SandboxExecutor
    {
        public static Process Execute(string command, ExecuteOptions executeOptions = null)
        {
            if (string.IsNullOrEmpty(command))
            {
                throw new ArgumentException(nameof(command));
            }

            var workingDirectory = executeOptions?.WorkingDirectory ?? Environment.CurrentDirectory;
            
            var parsedCommandData = command.Split(" ").ToList();
            var commandFilePath = TryGetFileFromPath(workingDirectory, parsedCommandData.First());
            var arguments = string.Join(" ", parsedCommandData.Skip(1));
            
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = $"{Environment.CurrentDirectory}/Sandbox/RunGuard/runguard",
                Arguments = $"{executeOptions?.ToArgumentsString()} {commandFilePath} {arguments}",
                WorkingDirectory = workingDirectory,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                CreateNoWindow = true
            });

            if (!string.IsNullOrEmpty(executeOptions?.StdIn))
            {
                process.StandardInput.Write(executeOptions.StdIn);
                process.StandardInput.Flush();
            }
            
            process?.WaitForExit();

            return process;
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