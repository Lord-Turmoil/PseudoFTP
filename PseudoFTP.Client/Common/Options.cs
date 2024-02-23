using CommandLine;

namespace PseudoFTP.Client.Common;

[Verb("transfer", HelpText = "Transfer files to the server.")]
class TransferOptions
{
    [Option('v', "verbose", Default = false, HelpText = "Prints all messages to standard output.")]
    public bool Verbose { get; set; }

    /// <summary>
    ///     Local source directory or file.
    /// </summary>
    [Option('s', "source", Required = true, HelpText = "Local source directory or file.")]
    public string Source { get; set; } = null!;

    /// <summary>
    ///     If specified, will use profile settings.
    /// </summary>
    [Option('p', "profile", Required = false, HelpText = "If specified, will use profile settings.")]
    public string? Profile { get; set; }

    /// <summary>
    ///     Remote destination directory.
    /// </summary>
    [Option('d', "destination", Required = false, HelpText = "Remote destination directory.")]
    public string? Destination { get; set; }

    /// <summary>
    ///     Optional transfer message.
    /// </summary>
    [Option('m', "message", Default = "", HelpText = "Transfer message.")]
    public string? Message { get; set; }

    /// <summary>
    ///     Whether to overwrite the existing files.
    ///     This will leave deleted files in the destination.
    /// </summary>
    [Option('o', "overwrite", Default = false, HelpText = "Whether to overwrite the existing files.")]
    public bool Overwrite { get; set; }

    /// <summary>
    ///     Whether to purge all the previous files.
    /// </summary>
    [Option('f', "force", Default = false, HelpText = "Whether to purge all the previous files.")]
    public bool PurgePrevious { get; set; }

    /// <summary>
    ///     Whether to keep the original .zip archive.
    /// </summary>
    [Option('k', "keep", Default = false, HelpText = "Whether to keep the original .zip archive.")]
    public bool KeepOriginal { get; set; }
}

[Verb("status", HelpText = "Get the status of the server.")]
class StatusOptions
{
}

[Verb("profile", HelpText = "Manage the profiles.")]
class ProfileOptions
{
    [Option('l', "list", HelpText = "List all the profiles: profile --list")]
    public bool List { get; set; }

    [Option('a', "add", Required = false, Min = 2, Max = 2, HelpText = "Add a new profile: profile --add {name} {path}")]
    public IEnumerable<string>? Add { get; set; }

    [Option('r', "remove", Required = false, HelpText = "Remove a profile: profile --remove {name}")]
    public string? Remove { get; set; }
}