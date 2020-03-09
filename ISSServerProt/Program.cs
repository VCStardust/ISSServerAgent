using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CoreRCON;
using Fclp;

namespace IssServerProt
{
    public class Server
    {
        private static bool errorOccured;
        public static void Main(string[] args)
        {
            string startArgs = default;
            ushort rconPort = default;
            string serverName = default;
            string rconPassword = default;
            var parser = new FluentCommandLineParser();
            parser.Setup<string>("argsFile").Required().Callback(it => startArgs = File.ReadAllText(it));
            parser.Setup<int>("rconPort").Required().Callback(it => rconPort = (ushort)it);
            parser.Setup<string>("name").Callback(it => serverName = it);
            parser.Setup<string>("rconPassword").Required().Callback(it => rconPassword = it);
            parser.Parse(args);

            Console.Title = $"Insurgency: Sandstorm {serverName} Server";

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
                Console.WriteLine("正在启动服务器");
                server.Start();
                server.BeginOutputReadLine();
            }
            server.Exited += async (sender, args) =>
            {
                await Task.Run(() =>
                {
                    if (errorOccured)
                    {
                        Console.WriteLine("重启中");
                        server.Refresh();
                        Start();
                    }
                    errorOccured = false;
                });
            };
            server.OutputDataReceived += async (sender, args) =>
            {
                var line = args.Data;
                Console.WriteLine(line);
                switch (line)
                {
                    case "LogGameMode: Display: State: GameStarting -> PreRound":
                        Console.WriteLine("检测到游戏开局");
                        await rcon.ConnectAsync();
                        await rcon.SendCommandAsync("gamemodeproperty MinimumEnemies 2");
                        await rcon.SendCommandAsync("gamemodeproperty bDisableVehicles false");
                        await rcon.SendCommandAsync("gamemodeproperty bKillerInfoRevealDistance true");
                        await rcon.SendCommandAsync("gamemodeproperty bDeadSay true");
                        Console.WriteLine($"{serverName} Server Modified");
                        break;
                    case "LogWindows: Error: Fatal error!":
                        Console.WriteLine("检测到错误");
                        errorOccured = true;
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