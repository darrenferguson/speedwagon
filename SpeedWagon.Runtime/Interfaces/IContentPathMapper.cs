namespace SpeedWagon.Interfaces
{
    public interface IContentPathMapper
    {
        string PathForUrl(string url, bool ensure);

        string RelativePath(string path);

        string GetContentFileName();

        string ContentRootFolder(string relativePath);

        string ContentFolder(string relativePath);

    }
}
