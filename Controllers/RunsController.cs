using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using JobeSharp.DTO;
using JobeSharp.Languages;
using JobeSharp.Model;
using JobeSharp.Sandbox;
using JobeSharp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ExecutionResult = JobeSharp.Languages.ExecutionResult;

namespace JobeSharp.Controllers
{
    [Route("/jobe/index.php/restapi/")]
    [ApiController]
    public class RunsController : ControllerBase
    {
        private LanguageRegistry LanguageRegistry { get; }
        private FileCache FileCache { get; }
        private ApplicationDbContext ApplicationDbContext { get; }
        
        public RunsController(LanguageRegistry languageRegistry, FileCache fileCache, ApplicationDbContext applicationDbContext)
        {
            LanguageRegistry = languageRegistry;
            FileCache = fileCache;
            ApplicationDbContext = applicationDbContext;
        }

        [HttpGet("runresults/{jobId}")]
        public async Task<ActionResult> GetRunResult(string jobId)
        {
            var run = await ApplicationDbContext.Runs.SingleAsync(r => r.JobId == jobId);

            if (run.State != RunState.Completed)
            {
                return NoContent();
            }
            
            return Ok(JsonConvert.DeserializeObject<ResultDto>(run.SerializedExecutionResult));
        }

        [HttpPost("runs")]
        public async Task<ActionResult> Run(RunDto runDto)
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

            using (var executor = new LanguageExecutor(task, FileCache))
            {
                if (!executor.CheckCachedFileExistence())
                {
                    return NotFound();
                }
            }
            
            var run = new Run
            {
                LanguageName = runDto.RunSpec.LanguageName,
                SerializedTask = JsonConvert.SerializeObject(task)
            };
            await ApplicationDbContext.Runs.AddAsync(run);
            await ApplicationDbContext.SaveChangesAsync();

            var jobId = BackgroundJob.Enqueue(() => ProcessTask(run.Id));
            run.JobId = jobId;
            await ApplicationDbContext.SaveChangesAsync();
            
            return Accepted(new { RunId = jobId });
        }

        public async Task ProcessTask(int runId)
        {
            var run = await ApplicationDbContext.Runs.FindAsync(runId);
            run.State = RunState.Processing;
            await ApplicationDbContext.SaveChangesAsync();
        
            var language = LanguageRegistry.Languages.Single(l => l.Name == run.LanguageName);
            var task = JsonConvert.DeserializeObject<ExecutionTask>(run.SerializedTask);
        
            using var executor = new LanguageExecutor(task, FileCache);
            var result = executor.Execute(language) switch
            {
                RunExecutionResult rer => new ResultDto
                {
                    CmpInfo = "",
                    StdErr = rer.Error,
                    StdOut = rer.Output,
                    RunId = null,
                    Outcome = GetOutcomeByExecutionResult(rer),
                },
                CompileExecutionResult cer => new ResultDto
                {
                    CmpInfo = cer.Error,
                    StdErr = "",
                    StdOut = cer.Output,
                    RunId = null,
                    Outcome = GetOutcomeByExecutionResult(cer),
                },
                _ => throw new NotImplementedException()
            };

            run.SerializedExecutionResult = JsonConvert.SerializeObject(result);
            run.State = RunState.Completed;
            await ApplicationDbContext.SaveChangesAsync();
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