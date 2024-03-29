﻿using JobeSharp.Languages.Abstract;
using JobeSharp.Languages.Concrete;

namespace JobeSharp.Languages
{
    public class LanguageRegistry
    {
        public ILanguage[] Languages { get; } = {
            new JavaLanguage(),
            new CLanguage(),
            new CppLanguage(),
            new TestLibLanguage(),
            new Python3Language(),
            new KotlinLanguage(),
            new PascalLanguage(),
            new NodeJsLanguage()
        };
    }
}