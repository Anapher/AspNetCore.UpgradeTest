using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Asp.UpgradeTest.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var port = 62323;

            var tcpClient = new TcpClient();
            await tcpClient.ConnectAsync("localhost", port);
            var stream = tcpClient.GetStream();


            var data = $@"GET /ws HTTP/1.1
Upgrade: websocket
Connection: Upgrade
Host: localhost:{port}
User-Agent: AspSocketTest/1.0
Sec-WebSocket-Key: PG178ygiZKSkeXGGZ6P9KQ==
Sec-WebSocket-Version: 13

";

            var buffer = Encoding.UTF8.GetBytes(data);
            await stream.WriteAsync(buffer, 0, buffer.Length);

            using (var streamReader = new StreamReader(stream, Encoding.UTF8, false, 8192, true))
            {
                var statusLine = await streamReader.ReadLineAsync();
                Console.WriteLine(statusLine); //check if Switching Protocol

                var headers = new Dictionary<string, string>();

                string line;
                while (!string.IsNullOrEmpty(line = await streamReader.ReadLineAsync()))
                {
                    var split = line.Split(new[] { ": " }, 2, StringSplitOptions.None);
                    headers.Add(split[0], split[1]);
                }
            }

            buffer = new byte[11];
            await stream.ReadAsync(buffer, 0, 11);

            Console.WriteLine(Encoding.UTF8.GetString(buffer));

            Console.ReadLine();
        }
    }
}
