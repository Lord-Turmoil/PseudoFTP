namespace PseudoFTP.Model;

public class TransferOption
{
    /// <summary>
    ///     Represents a .zip archive.
    /// </summary>
    public string Source { get; set; } = null!;

    /// <summary>
    ///     Represents the parent directory where the .zip archive will be extracted.
    /// </summary>
    public string Destination { get; set; } = null!;

    /// <summary>
    ///     Optional transfer message.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    ///     Whether to overwrite the existing files.
    ///     This will leave deleted files in the destination.
    /// </summary>
    public bool Overwrite { get; set; }

    /// <summary>
    ///     Whether to purge all the previous files.
    /// </summary>
    public bool PurgePrevious { get; set; }

    /// <summary>
    ///     Whether to keep the original .zip archive.
    /// </summary>
    public bool KeepOriginal { get; set; }
}