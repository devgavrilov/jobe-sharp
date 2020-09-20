namespace JobeSharp.Languages.Abstract
{
    internal abstract class CompiledLanguage : LanguageBase
    {
        public override ExecutionResult Execute(ExecutionTask executionTask)
        {
            base.Execute(executionTask);
            
            var compilationResult = Compile(executionTask);
            if (!compilationResult.IsSuccess)
            {
                return compilationResult;
            }

            return Run(executionTask);
        }

        protected abstract CompileExecutionResult Compile(ExecutionTask executionTask);

        protected abstract RunExecutionResult Run(ExecutionTask executionTask);
    }
}