using SpeedWagon.Runtime.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace SpeedWagon.Runtime.Services.Files
{
    public class FileSystemFileProvider : IFileProvider
    {
        public string Combine(string path1, string path2)
        {
            return Path.Combine(path1, path2);
        }

        public string Combine(string path1, string path2, string path3)
        {
            return Path.Combine(path1, path2, path3);

        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public void Delete(string path)
        {
            File.Delete(path);
        }

        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public Task<Stream> GetStream(string path)
        {
            return Task.FromResult<Stream>(new FileStream(path, FileMode.Create));
        }

        public Task<string[]> List(string path, string pattern, bool recursive)
        {
           if(recursive)
            {
                return Task.FromResult(Directory.GetFiles(path, pattern, SearchOption.AllDirectories));
            }

            return Task.FromResult(Directory.GetFiles(path));
        }

        public async Task<string> ReadAllText(string path)
        {
            return await File.ReadAllTextAsync(path);
        }

        public async Task WriteAllText(string path, string contents)
        {
            await File.WriteAllTextAsync(path, contents);
        }
    }
}
