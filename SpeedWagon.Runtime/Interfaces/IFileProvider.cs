using System.Threading.Tasks;

namespace SpeedWagon.Runtime.Interfaces
{
    public interface IFileProvider
    {
        Task WriteAllText(string path, string contents);

        Task<string> ReadAllText(string path);

        Task<string[]> List(string path, string pattern, bool recursive);

        void Delete(string path);

        bool Exists(string path);

        void CreateDirectory(string path);

        string Combine(string path1, string path2);

        string Combine(string path1, string path2, string path3);

    }
}
