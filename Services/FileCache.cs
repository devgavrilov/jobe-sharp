﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace JobeSharp.Services
{
    public class FileCache
    {
        private static string TempDirectory { get; } = Path.Combine(Path.GetTempPath(), "jobe");
        private static TimeSpan TTL { get; } = TimeSpan.FromDays(1);

        static FileCache()
        {
            if (!Directory.Exists(TempDirectory))
            {
                Directory.CreateDirectory(TempDirectory);
            }
            Console.WriteLine(TempDirectory);
        }

        public bool IsKeyExists(string key)
        {
            return File.Exists(GetFilePathByKey(key));
        }

        public string ReadString(string key)
        {
            return File.ReadAllText(GetFilePathByKey(key));
        }
        
        public void Write(string key, string value)
        {
            foreach (var file in Directory.EnumerateFiles(TempDirectory))
            {
                if (DateTime.UtcNow.Subtract(File.GetLastWriteTimeUtc(file)) > TTL)
                {
                    File.Delete(file);
                }
            }

            File.WriteAllText(GetFilePathByKey(key), value);
        }

        private string GetFilePathByKey(string key)
        {
            return Path.Combine(TempDirectory, GetMd5StringOfKey(key));
        }

        private string GetMd5StringOfKey(string key)
        {
            var bytes = MD5.Create().ComputeHash(Encoding.Default.GetBytes(key));
            
            var sb = new StringBuilder();
            foreach (var b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }
    }
}