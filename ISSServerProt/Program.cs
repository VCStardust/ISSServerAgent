using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CoreRCON;
using Fclp;

namespace IssServerProt
{
    public class Server
    {
        private static bool ErrorOccured;
        public static void Main(string[] args)
        {
            ushort rconPort = default;
            string serverName = default;
            string rconPassword = default;
            string AllConfig = File.ReadAllText("Config.txt");
            var ConfigE = AllConfig.Split(Environment.NewLine);
            var parser = new FluentCommandLineParser();
            parser.Setup<int>("rconPort").Required().Callback(it => rconPort = (ushort)it);
            parser.Setup<string>("name").Callback(it => serverName = it);
            parser.Setup<string>("rconPassword").Required().Callback(it => rconPassword = it);
            parser.Parse(args);

            string startArgs = ConfigE.First;

            var CmdLength = ConfigE.Length -1;

            Console.Title = $"ISS {serverName} Server";

            var rcon = new RCON(IPAddress.Loopback, rconPort, rconPassword);

            using Process server = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    FileName =
                        "Insurgency\\Binaries\\Win64\\InsurgencyServer-Win64-Shipping.exe",
                    Arguments =
                        startArgs,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                },
                EnableRaisingEvents = true
            };

            void Start()
            {
                Console.WriteLine("======Starting Server======");
                server.Start();
                server.BeginOutputReadLine();
                Thread.Sleep(45000);
                rcon.ConnectAsync();
            }
            server.Exited += async (sender, args) =>
            {
                await Task.Run(() =>
                {
                    if (ErrorOccured)
                    {
                        Console.WriteLine("======Restarting======");
                        server.Refresh();
                        Start();
                    }
                    ErrorOccured = false;
                });
            };
            server.OutputDataReceived += (sender, args) =>
            {
                var line = args.Data;
                Console.WriteLine(line);
                switch (line)
                {
                    case "LogGameMode: Display: State: GameStarting -> PreRound":
                        Console.WriteLine("======Round Start Detected======");
                        int n = 1;
                        while (n <= CmdLength)
                        {
                            rcon.SendCommandAsync(ConfigE.GetValue(n).ToString());
                            n++;
                        }
                        Console.WriteLine($"{serverName} Server Modified");
                        break;
                    case "LogWindows: Error: Fatal error!":
                        Console.WriteLine("======Error Occured======");
                        ErrorOccured = true;
                        break;
                }
            };
            Start();

            while (true)
            {
                Console.ReadKey(true);
            }
        }
    }
}