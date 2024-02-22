using PseudoFTP.Model;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;

namespace PseudoFTP.Helper;

/// <summary>
///     Help to extract files from a zip file to a directory.
/// </summary>
public static class TransferHelper
{
    public static void Transfer(TransferOption option)
    {
        TransferPreamble(option);

        BeforeTransfer(option);
        TransferImpl(option);
        PostTransfer(option);
    }

    private static void TransferPreamble(TransferOption option)
    {
        if (!File.Exists(option.Source))
        {
            throw new FileNotFoundException("The source file does not exist.", option.Source);
        }

        if (Path.GetExtension(option.Source) != ".zip")
        {
            throw new FileLoadException("The source file is not a .zip archive.");
        }

        if (!Directory.Exists(option.Destination))
        {
            throw new DirectoryNotFoundException("The destination directory does not exist.");
        }
    }

    private static void BeforeTransfer(TransferOption option)
    {
        if (option.PurgePrevious)
        {
            DirectoryInfo root = new(option.Destination);

            foreach (FileInfo file in root.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo dir in root.GetDirectories())
            {
                dir.Delete(true);
            }
        }
    }

    private static void PostTransfer(TransferOption option)
    {
        if (!option.KeepOriginal)
        {
            File.Delete(option.Source);
        }
    }

    private static void TransferImpl(TransferOption option)
    {
        using ZipArchive archive = ZipArchive.Open(option.Source);
        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            if (!entry.IsDirectory)
            {
                entry.WriteToDirectory(option.Destination, new ExtractionOptions {
                    ExtractFullPath = true,
                    Overwrite = option.Overwrite
                });
            }
        }
    }
}