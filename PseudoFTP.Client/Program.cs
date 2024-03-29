﻿using CommandLine;
using CommandLine.Text;
using Microsoft.Extensions.Logging;
using PseudoFTP.Client.Common;
using PseudoFTP.Client.Services;
using PseudoFTP.Client.Utils;
using RestSharp;

namespace PseudoFTP.Client;

class Program
{
    private static readonly Configuration _config;

    static Program()
    {
        _config = Configuration.Load("config.json");
    }

    private static void Main(string[] args)
    {
        var parser = new Parser(config => config.HelpWriter = null);

        ParserResult<object>? result = parser.ParseArguments<TransferOptions, ProfileOptions, StatusOptions>(args);
        Delegate action;
        result.MapResult(
            (TransferOptions options) => RunTransfer(options),
            (ProfileOptions options) => RunProfile(options),
            (StatusOptions options) => RunStatus(options),
            _ => {
                var helpText = HelpText.AutoBuild(result,
                    h => {
                        h.Heading = "PseudoFTP Client v1.2.0 - A tool to transfer files to remote server.";
                        h.Copyright = "Copyright (C) Tony's Studio 2023 - " + DateTime.Now.Year;
                        h.AutoVersion = false;
                        return h;
                    },
                    e => e);
                Console.WriteLine(helpText);
                return 1;
            });
    }

    private static IRestClient InitRestClient()
    {
        return new RestClient(new RestClientOptions(_config.Server) {
            MaxTimeout = _config.MaxTimeout
        });
    }

    private static int RunTransfer(TransferOptions options)
    {
        OverrideConfig(options);

        // Transfer files to the server.
        IRestClient client = InitRestClient();
        var service = new TransferService(_config, client, options);
        if (options.Histories)
        {
            return new ProgressHandler<int>("Getting transfer histories", service.GetTransferHistories())
                .StaticPerform();
        }

        return service.Transfer();
    }

    private static int RunProfile(ProfileOptions options)
    {
        OverrideConfig(options);

        // Manage the profiles.
        IRestClient client = InitRestClient();
        var service = new ProfileService(_config, client, options);
        if (options.List)
        {
            return new ProgressHandler<int>("Getting profiles", service.GetProfiles())
                .StaticPerform();
        }

        if (options.Add != null && options.Add.Count() == 2)
        {
            return new ProgressHandler<int>("Adding profile", service.AddProfile())
                .StaticPerform();
        }

        if (options.Remove != null)
        {
            return new ProgressHandler<int>("Deleting profile", service.DeleteProfile())
                .StaticPerform();
        }

        Console.WriteLine("No action specified.");
        return 0;
    }

    private static int RunStatus(StatusOptions options)
    {
        IRestClient client = InitRestClient();
        var service = new StatusService(_config, client, options);
        return new ProgressHandler<int>("Getting server status", service.GetStatus())
            .StaticPerform();
    }

    private static void OverrideConfig(BaseOptions options)
    {
        if (options.Username != null)
        {
            _config.Username = options.Username;
        }

        if (options.Password != null)
        {
            _config.Password = options.Password;
        }

        if (options.Server != null)
        {
            _config.Server = options.Server;
        }

        LogHelper.Init(options.Verbose ? LogLevel.Debug : LogLevel.Information);
    }
}