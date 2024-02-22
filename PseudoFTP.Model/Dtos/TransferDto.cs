using Microsoft.AspNetCore.Http;

namespace PseudoFTP.Model.Dtos;

public class TransferDto
{
    /// <summary>
    ///     Whether to use profile instead of explicit destination.
    /// </summary>
    public string? Profile { get; set; }

    /// <summary>
    ///     Destination path.
    /// </summary>
    public string? Destination { get; set; }

    /// <summary>
    ///     Optional message.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    ///     A .zip archive.
    /// </summary>
    public IFormFile Archive { get; set; } = null!;

    public bool Overwrite { get; set; }
    public bool PurgePrevious { get; set; }
    public bool KeepOriginal { get; set; }

    public bool IsProfile => !string.IsNullOrWhiteSpace(Profile);
}