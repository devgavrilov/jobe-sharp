using JobeSharp.Languages.Abstract;
using JobeSharp.Languages.Concrete;

namespace JobeSharp.Languages
{
    public class LanguageRegistry
    {
        public ILanguage[] Languages { get; } = {
            new JavaLanguage(),
            new CLanguage(),
            new NodeJsLanguage()
        };
    }
}