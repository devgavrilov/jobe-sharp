using System;
using System.Collections.Generic;
using System.Linq;
using JobeSharp.DTO;
using JobeSharp.Languages;
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
                Language = LanguageRegistry.Languages.Single(l => l.Name == runDto.RunSpec.LanguageName),
                SourceCode = runDto.RunSpec.SourceCode,
                SourceFileName = runDto.RunSpec.SourceFileName,
                CachedFilesIdPath = runDto.RunSpec.FileList?.ToDictionary(a => a[0], a => a[1]) ?? new Dictionary<string, string>(),
                Debug = runDto.RunSpec.Debug
            };
            
            using var executor = new LanguageExecutor(task, FileCache);
            var result = executor.Execute();

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