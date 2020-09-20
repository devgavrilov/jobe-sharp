using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using JobeSharp.DTO;
using JobeSharp.Languages;
using JobeSharp.Sandbox;
using JobeSharp.Services;
using Microsoft.AspNetCore.Mvc;

namespace JobeSharp.Controllers
{
    [Route("/jobe/index.php/restapi/runs")]
    [ApiController]
    public class RunsController : ControllerBase
    {
        private LanguageRegistry LanguageRegistry { get; }
        private FileCache FileCache { get; }
        
        public RunsController(LanguageRegistry languageRegistry, FileCache fileCache)
        {
            LanguageRegistry = languageRegistry;
            FileCache = fileCache;
        }

        [HttpPost]
        public ActionResult Run(RunDto runDto)
        {
            _ = runDto ?? throw new ArgumentNullException(nameof(runDto));
            _ = runDto.RunSpec ?? throw new ArgumentNullException(nameof(runDto.RunSpec));
            
            var task = new ExecutionTask
            {
                SourceCode = runDto.RunSpec.SourceCode,
                SourceFileName = runDto.RunSpec.SourceFileName,
                Input = runDto.RunSpec.Input,
                CachedFilesIdPath = runDto.RunSpec.FileList?.ToDictionary(a => a[0], a => a[1]) ?? new Dictionary<string, string>(),
                Debug = runDto.RunSpec.Debug,
            };

            if (runDto.RunSpec.Parameters != null)
            {
                if (runDto.RunSpec.Parameters.ContainsKey("disklimit"))
                {
                    task.ExecuteOptions.MergeIntoCurrent(new ExecuteOptions { FileSizeKb = runDto.RunSpec.Parameters["disklimit"].GetInt32() });
                }

                if (runDto.RunSpec.Parameters.ContainsKey("streamsize"))
                {
                    task.ExecuteOptions.MergeIntoCurrent(new ExecuteOptions { StreamSizeKb = runDto.RunSpec.Parameters["streamsize"].GetInt32() });
                }

                if (runDto.RunSpec.Parameters.ContainsKey("cputime"))
                {
                    task.ExecuteOptions.MergeIntoCurrent(new ExecuteOptions { CpuTimeSeconds = runDto.RunSpec.Parameters["cputime"].GetDouble() });
                }

                if (runDto.RunSpec.Parameters.ContainsKey("memorylimit"))
                {
                    task.ExecuteOptions.MergeIntoCurrent(new ExecuteOptions { TotalMemoryKb = runDto.RunSpec.Parameters["memorylimit"].GetInt32() });
                }

                if (runDto.RunSpec.Parameters.ContainsKey("numprocs"))
                {
                    task.ExecuteOptions.MergeIntoCurrent(new ExecuteOptions { NumberOfProcesses = runDto.RunSpec.Parameters["numprocs"].GetInt32() });
                }

                if (runDto.RunSpec.Parameters.ContainsKey("compileargs"))
                {
                    task.CompileArguments = runDto.RunSpec.Parameters["compileargs"].EnumerateArray().Select(e => e.GetString()).ToArray();
                }

                if (runDto.RunSpec.Parameters.ContainsKey("linkargs"))
                {
                    task.LinkArguments = runDto.RunSpec.Parameters["linkargs"].EnumerateArray().Select(e => e.GetString()).ToArray();
                }

                if (runDto.RunSpec.Parameters.ContainsKey("interpreterargs"))
                {
                    task.ExecuteArguments = runDto.RunSpec.Parameters["interpreterargs"].EnumerateArray().Select(e => e.GetString()).ToArray();
                }
            }
            
            task.ExecuteOptions.MergeIntoCurrent(new ExecuteOptions{ TimeSeconds = task.ExecuteOptions.CpuTimeSeconds * 2 });

            var language = LanguageRegistry.Languages.Single(l => l.Name == runDto.RunSpec.LanguageName);
            
            using var executor = new LanguageExecutor(task, FileCache);
            var result = executor.Execute(language);

            return result switch
            {
                RunExecutionResult _ => Ok(new ResultDto
                {
                    CmpInfo = "",
                    StdErr = result.Error,
                    StdOut = result.Output,
                    RunId = null,
                    Outcome = GetOutcomeByExecutionResult(result),
                }),
                CompileExecutionResult _ => Ok(new ResultDto
                {
                    CmpInfo = result.Error,
                    StdErr = "",
                    StdOut = result.Output,
                    RunId = null,
                    Outcome = GetOutcomeByExecutionResult(result),
                }),
                _ => throw new NotImplementedException()
            };
        }

        private int GetOutcomeByExecutionResult(ExecutionResult executionResult)
        {
            if (executionResult.ExitCode == 0)
            {
                return 15;
            }
            
            if (executionResult is CompileExecutionResult)
            {
                return 11;
            }

            if (executionResult.Error.Contains("warning: timelimit exceeded"))
            {
                return 13;
            }

            return 12;
        }
    }
}