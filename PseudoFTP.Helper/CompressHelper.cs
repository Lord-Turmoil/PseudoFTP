using MAB.DotIgnore;
using Microsoft.Extensions.Logging;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Writers;

namespace PseudoFTP.Client.Utils;

/// <summary>
///     A helper class for compressing files and directories.
/// </summary>
public static class CompressHelper
{
    /// <summary>
    ///     Compress files and directories into a zip archive.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="ftpIgnorePath"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    public static string CompressFiles(string source, string? ftpIgnorePath = null)
    {
        ILogger logger = LogHelper.GetLogger();
        var archive = ZipArchive.Create();
        if (File.Exists(source))
        {
            archive = ZipArchive.Create();
            archive.AddEntry(Path.GetFileName(source), source);
            logger.LogDebug("Compressing {file}...", source);
        }
        else if (Directory.Exists(source))
        {
            if (ftpIgnorePath is null)
            {
                // Check if .ftpignore exists in the source directory.
                string file = Path.Join(source, ".ftpignore");
                if (File.Exists(file))
                {
                    ftpIgnorePath = file;
                }
            }

            if (ftpIgnorePath is null)
            {
                archive.AddAllFromDirectory(source, "*");
                logger.LogDebug("Compressing all files and folders in {folder}...", source);
            }
            else
            {
                logger.LogDebug("Using .ftpignore file {file}", ftpIgnorePath);
                IEnumerable<string> files = GetAcceptedFiles(source, ftpIgnorePath);
                foreach (string file in files)
                {
                    var fileInfo = new FileInfo(file);
                    archive.AddEntry(
                        Path.GetRelativePath(source, file),
                        fileInfo.OpenRead(),
                        true,
                        fileInfo.Length,
                        fileInfo.LastWriteTime);
                    logger.LogDebug("\tCompressing {file}...", file);
                }
            }
        }
        else
        {
            throw new FileNotFoundException($"No such file or directory: {source}");
        }

        string archivePath = Path.GetTempFileName() + ".zip";
        archive.SaveTo(archivePath, new WriterOptions(CompressionType.Deflate));
        logger.LogDebug("Archive saved to {archive}", archivePath);

        return archivePath;
    }

    private static List<string> GetAcceptedFiles(string source, string ftpIgnorePath)
    {
        var directory = new DirectoryInfo(source);
        var ignores = new IgnoreList(ftpIgnorePath);

        var files = new List<string>();
        GetAcceptedFilesImpl(directory, ignores, files);
        return files;
    }

    private static void GetAcceptedFilesImpl(DirectoryInfo directory, IgnoreList ignores, ICollection<string> files)
    {
        foreach (DirectoryInfo dir in directory.GetDirectories().Where(d => !ignores.IsIgnored(d)))
        {
            GetAcceptedFilesImpl(dir, ignores, files);
        }

        foreach (FileInfo file in directory.GetFiles().Where(f => !ignores.IsIgnored(f)))
        {
            files.Add(file.FullName);
        }
    }
}