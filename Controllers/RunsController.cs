using System;
using System.Linq;
using JobeSharp.DTO;
using JobeSharp.Languages;
using Microsoft.AspNetCore.Mvc;

namespace JobeSharp.Controllers
{
    [Route("/jobe/index.php/restapi/runs")]
    [ApiController]
    public class RunsController : ControllerBase
    {
        private LanguageRegistry LanguageRegistry { get; }
        
        public RunsController(LanguageRegistry languageRegistry)
        {
            LanguageRegistry = languageRegistry;
        }

        [HttpPost]
        public ActionResult Run(RunDto runDto)
        {
            var task = new ExecutionTask
            {
                Language = LanguageRegistry.Languages.Single(l => l.Name == runDto.RunSpec.LanguageName),
                SourceCode = runDto.RunSpec.SourceCode,
                SourceFileName = runDto.RunSpec.SourceFileName
            };
            
            using var executor = new LanguageExecutor(task);
            var result = executor.Execute();

            return result switch
            {
                RunExecutionResult _ => Ok(new ResultDto
                {
                    CmpInfo = "",
                    StdErr = result.Error,
                    StdOut = result.Output,
                    RunId = "",
                    Outcome = GetOutcomeByExecutionResult(result),
                }),
                CompileExecutionResult _ => Ok(new ResultDto
                {
                    CmpInfo = result.Error,
                    StdErr = "",
                    StdOut = result.Output,
                    RunId = "",
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