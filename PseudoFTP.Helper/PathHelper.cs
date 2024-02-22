namespace PseudoFTP.Helper;

public static class PathHelper
{
    /// <summary>
    /// Determine whether the path is contained in the base path.
    /// </summary>
    /// <param name="basePath"></param>
    /// <param name="targetPath"></param>
    /// <returns></returns>
    public static bool IsContained(string basePath, string targetPath)
    {
        string fullBasePath = Path.GetFullPath(basePath);
        string fullTargetPath = Path.GetFullPath(targetPath);

        return fullTargetPath.StartsWith(fullBasePath, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determine whether two paths is related.
    /// </summary>
    /// <param name="path1"></param>
    /// <param name="path2"></param>
    /// <returns></returns>
    public static bool IsRelated(string path1, string path2)
    {
        return IsContained(path1, path2) || IsContained(path2, path1);
    }
}