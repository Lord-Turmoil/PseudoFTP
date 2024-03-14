using CommandLine;

namespace PseudoFTP.Client.Common;

/// <summary>
///     Base options provides overrides to config.json.
/// </summary>
class BaseOptions
{
    [Option('u', "username", Required = false, HelpText = "Username")]
    public string? Username { get; set; }

    [Option('p', "password", Required = false, HelpText = "Password")]
    public string? Password { get; set; }

    [Option('r', "remote", Required = false, HelpText = "Remote server ip with port")]
    public string? Server { get; set; }

    [Option('v', "verbose", Default = false, HelpText = "Prints all messages to standard output")]
    public bool Verbose { get; set; }
}

[Verb("transfer", HelpText = "Transfer files to the server.")]
class TransferOptions : BaseOptions
{
    /// <summary>
    ///     Local source directory or file.
    /// </summary>
    [Option('s', "source", Required = false, HelpText = "Local source directory or file")]
    public string? Source { get; set; }

    /// <summary>
    ///     If specified, will use profile settings.
    /// </summary>
    [Option('l', "profile", Required = false, HelpText = "If specified, will use profile settings")]
    public string? Profile { get; set; }

    /// <summary>
    ///     Remote destination directory.
    /// </summary>
    [Option('d', "destination", Required = false, HelpText = "Remote destination directory")]
    public string? Destination { get; set; }

    /// <summary>
    ///     Optional transfer message.
    /// </summary>
    [Option('m', "message", Default = "", HelpText = "Transfer message")]
    public string? Message { get; set; }

    /// <summary>
    ///     Path to .ftpignore file.
    /// </summary>
    [Option('i', "ignore", Required = false, HelpText = "Path to .ftpignore file")]
    public string? FtpIgnore { get; set; }

    /// <summary>
    ///     Whether to overwrite the existing files.
    ///     This will leave deleted files in the destination.
    /// </summary>
    [Option('o', "overwrite", Default = false, HelpText = "Whether to overwrite the existing files")]
    public bool Overwrite { get; set; }

    /// <summary>
    ///     Whether to purge all the previous files.
    /// </summary>
    [Option('f', "force", Default = false, HelpText = "Whether to purge all the previous files")]
    public bool PurgePrevious { get; set; }

    /// <summary>
    ///     Whether to keep the original .zip archive.
    /// </summary>
    [Option('k', "keep", Default = false, HelpText = "Whether to keep the original .zip archive")]
    public bool KeepOriginal { get; set; }

    [Option("histories", Default = false, HelpText = "Get latest 10 transfer histories")]
    public bool Histories { get; set; }
}

[Verb("status", HelpText = "Get the status of the server.")]
class StatusOptions
{
}

[Verb("profile", HelpText = "Manage the profiles.")]
class ProfileOptions : BaseOptions
{
    [Option('l', "list", HelpText = "List all the profiles: profile --list")]
    public bool List { get; set; }

    [Option('a', "add", Required = false, Min = 2, Max = 2,
        HelpText = "Add a new profile: profile --add {name} {path}")]
    public IEnumerable<string>? Add { get; set; }

    [Option('d', "delete", Required = false, HelpText = "Delete a profile: profile --remove {name}")]
    public string? Remove { get; set; }
}