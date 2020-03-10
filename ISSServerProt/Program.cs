﻿using System;
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
            string startArgs = default;
            ushort rconPort = default;
            string serverName = default;
            string rconPassword = default;
            string Command= default;
            int n = 1;
            var parser = new FluentCommandLineParser();
            parser.Setup<string>("argsFile").Required().Callback(it => startArgs = File.ReadAllText(it));
            parser.Setup<int>("rconPort").Required().Callback(it => rconPort = (ushort)it);
            parser.Setup<string>("name").Callback(it => serverName = it);
            parser.Setup<string>("rconPassword").Required().Callback(it => rconPassword = it);
            parser.Setup<string>("Command").Required().Callback(it => Command = it);
            parser.Parse(args);

            var CommandE = Command.Split(";");

            Console.Title = $"ISS {serverName} Server";

            var rcon = new RCON(IPAddress.Loopback, rconPort, rconPassword);
            var CmdLength = CommandE.Length;

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
            server.OutputDataReceived += async (sender, args) =>
            {
                var line = args.Data;
                Console.WriteLine(line);
                switch (line)
                {
                    case "LogGameMode: Display: State: GameStarting -> PreRound":
                        Console.WriteLine("======Round Start Detected======");
                        await rcon.ConnectAsync();
                        while (n <= CmdLength)
                        {
                            await rcon.SendCommandAsync(CommandE.GetValue(n).ToString());
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