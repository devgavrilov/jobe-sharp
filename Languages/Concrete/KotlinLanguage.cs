﻿using System;
using System.Text.RegularExpressions;
using JobeSharp.Languages.Abstract;
using JobeSharp.Languages.Versions;
using JobeSharp.Sandbox;

namespace JobeSharp.Languages.Concrete
{
    internal class KotlinLanguage : CompiledLanguage
    {
        public override string Name => "kotlin";
        
        protected override IVersionProvider VersionProvider => 
            new CommandRegexVersionProvider("kotlin -version", new Regex("version ?(([0-9._-]|release)*)"));

        protected override CompileExecutionResult Compile(ExecutionTask executionTask)
        {
            executionTask.ExecuteOptions.NumberOfProcesses = Math.Max(executionTask.ExecuteOptions.NumberOfProcesses, 512);
            executionTask.ExecuteOptions.TotalMemoryKb = 0;
            executionTask.ExecuteOptions.CpuTimeSeconds = 20;
            executionTask.ExecuteOptions.TimeSeconds = 30;
            
            var compileCommand = $"kotlinc {GetSourceFilePath(executionTask)} {executionTask.GetCompileArguments("-include-runtime")} -d {GetSourceFilePath(executionTask)}.jar";
            
            return new CompileExecutionResult(
                SandboxExecutor.Execute(compileCommand, executionTask.ExecuteOptions));
        }

        protected override RunExecutionResult Run(ExecutionTask executionTask)
        {
            executionTask.ExecuteOptions.StdIn = executionTask.Input;
            executionTask.ExecuteOptions.NumberOfProcesses = Math.Max(executionTask.ExecuteOptions.NumberOfProcesses, 256);
            executionTask.ExecuteOptions.TotalMemoryKb = 0;
            
            return new RunExecutionResult(
                SandboxExecutor.Execute($"java -jar {executionTask.GetExecuteArguments("-Xrs -Xss8m -Xmx200m")} {GetSourceFilePath(executionTask)}.jar", executionTask.ExecuteOptions));
        }
    }
}