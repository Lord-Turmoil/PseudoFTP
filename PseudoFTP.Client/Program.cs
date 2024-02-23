using CommandLine;
using CommandLine.Text;
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
        // Transfer files to the server.
        return 0;
    }

    private static int RunProfile(ProfileOptions options)
    {
        // Manage the profiles.
        return 0;
    }

    private static int RunStatus(StatusOptions options)
    {
        IRestClient client = InitRestClient();
        var service = new StatusService(_config, options, client);
        return service.GetStatus().Result;
    }
}