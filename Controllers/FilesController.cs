using System;
using JobeSharp.DTO;
using JobeSharp.Services;
using Microsoft.AspNetCore.Mvc;

namespace JobeSharp.Controllers
{
    [Route("/jobe/index.php/restapi/files")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private FileCache FileCache { get; }
        
        public FilesController(FileCache fileCache)
        {
            FileCache = fileCache;
        }
        
        [HttpHead("{key}")]
        public ActionResult CheckFile(string key)
        {
            if (FileCache.IsKeyExists(key))
            {
                return NoContent();
            }

            return NotFound();
        }

        [HttpPut("{key}")]
        public ActionResult SaveFile(string key, FileDto fileDto)
        {
            FileCache.Write(key, Convert.FromBase64String(fileDto.FileContents));
            return NoContent();
        }
    }
}