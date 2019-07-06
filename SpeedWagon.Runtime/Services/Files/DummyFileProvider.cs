using System.Threading.Tasks;
using SpeedWagon.Runtime.Interfaces;

namespace SpeedWagon.Runtime.Services.Files
{
    public class DummyFileProvider : IFileProvider
    {
        public string Combine(string path1, string path2)
        {
            return string.Empty;
        }

        public string Combine(string path1, string path2, string path3)
        {
            return string.Empty;
        }

        public void CreateDirectory(string path)
        {
           
        }

        public void Delete(string path)
        {
            
        }

        public bool Exists(string path)
        {
            return false;
        }

        public Task<string[]> List(string path, string pattern, bool recursive)
        {
            return Task.FromResult(new string[] { });
        }

        public Task<string> ReadAllText(string path)
        {
            return Task.FromResult(string.Empty);
        }

        public Task WriteAllText(string path, string contents)
        {
            return Task.CompletedTask;
        }
    }
}
