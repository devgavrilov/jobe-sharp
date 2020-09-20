using System;
using System.IO;
using JobeSharp.Languages.Abstract;
using JobeSharp.Services;

namespace JobeSharp.Languages
{
    internal class LanguageExecutor : IDisposable
    {
        private string WorkTempDirectory { get; } =
            Path.Combine(Path.GetTempPath(), "jobe", "executions", Guid.NewGuid().ToString());
        
        private ExecutionTask ExecutionTask { get; }
        private FileCache FileCache { get; }

        public LanguageExecutor(ExecutionTask executionTask, FileCache fileCache)
        {
            Directory.CreateDirectory(WorkTempDirectory);

            ExecutionTask = executionTask;
            FileCache = fileCache;
            
            ExecutionTask.WorkTempDirectory = WorkTempDirectory;
            ExecutionTask.ExecuteOptions.WorkingDirectory = WorkTempDirectory;
        }

        public ExecutionResult Execute(ILanguage language)
        {
            LoadFiles();

            return language.Execute(ExecutionTask);
        }

        private void LoadFiles()
        {
            foreach (var (id, path) in ExecutionTask.CachedFilesIdPath)
            {
                File.WriteAllBytes(Path.Combine(WorkTempDirectory, path), FileCache.ReadBytes(id));
            }
        }

        public void Dispose()
        {
            if (ExecutionTask.Debug)
            {
                return;
            }
            
            Directory.Delete(WorkTempDirectory, recursive: true);
        }
    }
}