namespace JobeSharp.Languages.Abstract
{
    internal abstract class InterpretedLanguage : LanguageBase
    {
        public override ExecutionResult Execute(ExecutionTask executionTask)
        {
            base.Execute(executionTask);
            return Run(executionTask);
        }

        protected abstract RunExecutionResult Run(ExecutionTask executionTask);
    }
}