﻿using System;
using System.Text.RegularExpressions;
using JobeSharp.Sandbox;

namespace JobeSharp.Languages.Versions
{
    internal class CommandRegexVersionProvider : IVersionProvider
    {
        private string Command { get; }
        private Regex VersionParser { get; }
        
        public CommandRegexVersionProvider(string command, Regex versionParser)
        {
            Command = command;
            VersionParser = versionParser;
        }

        public string GetVersion()
        {
            var executionResult = SandboxExecutor.Execute(Command);
            if (executionResult.ExitCode != 0)
            {
                throw new InvalidOperationException("Can't use not installed language.");
            }
            
            var match = VersionParser.Match(executionResult.Output + executionResult.Error);
            if (!match.Success)
            {
                throw new InvalidOperationException("Can't find version by using regex.");
            }

            return match.Groups[1].Value;
        }

        public bool CheckAnyVersionExistence()
        {
            return SandboxExecutor.Execute(Command).ExitCode == 0;
        }
    }
}