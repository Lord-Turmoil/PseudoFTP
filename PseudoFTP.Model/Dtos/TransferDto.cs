using Microsoft.AspNetCore.Http;

namespace PseudoFTP.Model.Dtos;

public class TransferDto
{
    /// <summary>
    ///     Whether to use profile instead of explicit destination.
    /// </summary>
    public string? Profile { get; init; }

    /// <summary>
    ///     Destination path.
    /// </summary>
    public string? Destination { get; init; }

    /// <summary>
    ///     Optional message.
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    ///     Path to .ftpignore file.
    /// </summary>
    public string? FtpIgnore { get; init; }

    /// <summary>
    ///     A .zip archive.
    /// </summary>
    public IFormFile Archive { get; set; } = null!;

    public bool Overwrite { get; init; }
    public bool PurgePrevious { get; init; }
    public bool KeepOriginal { get; init; }

    public bool IsProfile => !string.IsNullOrWhiteSpace(Profile);
}