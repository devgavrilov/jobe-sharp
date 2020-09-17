using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace JobeSharp.Sandbox
{
    internal static class SandboxExecutor
    {
        public static Process Execute(string command, RunOptions runOptions = null)
        {
            if (string.IsNullOrEmpty(command))
            {
                throw new ArgumentException(nameof(command));
            }
            
            var parsedCommandData = command.Split(" ").ToList();
            var commandFilePath = TryGetFileFromPath(parsedCommandData.First());
            var arguments = string.Join(" ", parsedCommandData.Skip(1));
            
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = $"{Environment.CurrentDirectory}/Sandbox/RunGuard/runguard",
                Arguments = $"{runOptions?.ToArgumentsString()} {commandFilePath} {arguments}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            });
            
            process?.WaitForExit();

            return process;
        }

        private static string TryGetFileFromPath(string fileName)
        {
            if (File.Exists(fileName))
            {
                return Path.GetFullPath(fileName);
            }
            
            var environmentPath = Environment.GetEnvironmentVariable("PATH");
            if (string.IsNullOrEmpty(environmentPath))
            {
                return Path.GetFullPath(fileName);
            }
            
            var paths = environmentPath.Split(Path.PathSeparator);
            
            var exePath = paths
                .Select(x => Path.Combine(x, fileName))
                .FirstOrDefault(File.Exists);
            
            return exePath ?? Path.GetFullPath(fileName);
        }
    }
}