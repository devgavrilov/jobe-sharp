using System.Linq;
using JobeSharp.Languages;
using Microsoft.AspNetCore.Mvc;

namespace JobeSharp.Controllers
{
    [Route("/jobe/index.php/restapi/languages")]
    [ApiController]
    public class LanguagesController : ControllerBase
    {
        private LanguageRegistry LanguageRegistry { get; }
        
        public LanguagesController(LanguageRegistry languageRegistry)
        {
            LanguageRegistry = languageRegistry;
        }
        
        [HttpGet]
        public ActionResult GetLanguages()
        {
            var installedLanguages = LanguageRegistry.Languages.Where(l => l.IsInstalled.Value);
            return Ok(installedLanguages.Select(l => new [] { l.Name, l.Version.Value }).ToArray());
        }
    }
}