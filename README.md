# PseudoFTP

> Copyright &copy; Tony's Studio 2023 - 2024

----

**Require .NET 8.0 or higher**

A tool to transfer files to the server without FTP enabled. This is especially useful for static site publishing.

## Quick start

### Configuration

First, you need to create a `config.json` under the working directory of `PseudoFTP.Client.exe`.

If you want to compile it from source, you should add `config.json` under the `PseudoFTP.Client` project folder.

```json
{
    "Server": "http://localhost:6256",
    "Username": "Username",
    "Password": "Password",
    "MaxTimeout": 1000,
    "MaxRetry": 10
}
```

- The Api project will use port 6256 by default, you can change it in that project depends on your situation.
- The username and password are manually set in database. (It does not support registration.)
- `MaxTimeout` is the timeout for HTTP connection, in millisecond.
- `MaxRetry` is how many times should the client wait for transfer to complete. The total waiting time is `MaxTimeout * MaxRetry*`.

### Transfer files

To transfer files and sub-directories under local `.\public` to remote `C:\Users\Administrator\Desktop\wwwroot\Blog` with a message, you can use the following command.

```pwsh
> .\PseudoFTP.Client.exe transfer -s .\public\ -d C:\Users\Administrator\Desktop\wwwroot\Blog -o -m "blog deployment"
```

You can also add this long directory as a profile.

```pwsh
> .\PseudoFTP.Client.exe profile -a blog C:\Users\Administrator\Desktop\wwwroot\Blog
> .\PseudoFTP.Client.exe profile -l
Profiles:
  blog -- C:\Users\Administrator\Desktop\wwwroot\Blog
> .\PseudoFTP.Client.exe transfer -s .\public\ -p blog -o -m "with profile"
```

## Usage

### Checking server status

```pwsh
PseudoFTP.Client.exe status
```

### Managing profiles

```
PseudoFTP.Client.exe profile -l, --list      List all the profiles: profile --list

PseudoFTP.Client.exe profile -a, --add       Add a new profile: profile --add {name} {path}

PseudoFTP.Client.exe profile -r, --remove    Remove a profile: profile --remove {name}
```

## Transfering file/directory

```
  -s, --source         Local source directory or file

  -p, --profile        If specified, will use profile settings

  -d, --destination    Remote destination directory

  -m, --message        (Default: ) Transfer message

  -o, --overwrite      (Default: false) Whether to overwrite the existing files

  -f, --force          (Default: false) Whether to purge all the previous files

  -k, --keep           (Default: false) Whether to keep the original .zip archive

  --histories          (Default: false) Get latest 10 transfer histories
```

Local path can be relative or absolute, but remote path must be absolute.

> Notice that, when transfering directory, it will only transfer the contents of the directory, without the directory itself.
