using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WebScrapper.Extensions;

internal class Program
{
    public static async Task Main(string[] args)
    {
        //using HttpClient client = new();
        //client.DefaultRequestHeaders.Accept.Clear();
        //client.DefaultRequestHeaders.Accept.Add(
        //    new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
        //client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
        using var tcpClient = new TcpClient();
        await ProcessRepositoriesAsync(tcpClient, string.Join(" ", args));
    }
    static async Task ProcessRepositoriesAsync(TcpClient client, string args)
    {
        //args = string.IsNullOrEmpty(args) ? "u https://www.utm.md" : args;
        try
        {

            var options = args.Split(" ");
            var option = options[0];

            Func<Task> reqFunction = option switch
            {
                "u" => async () =>
                {
                    var url = options[1];
                    var uri = new Uri(url);
                    var host = uri.Host;
                    client.Connect(host, 443);
                    using SslStream sslStream = new SslStream(client.GetStream(), false,
        new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                    // This is where you read and send data
                    var message = @$"GET {uri.AbsolutePath} HTTP/1.1
Accept: text/html, charset=utf-8
Connection: close
User-Agent: C# program
Host: {host}
" + "\r\n\r\n";
                    sslStream.AuthenticateAsClient(host);
                    using var reader = new StreamReader(sslStream, Encoding.UTF8);
                    byte[] bytes = Encoding.UTF8.GetBytes(message);
                    sslStream.Write(bytes, 0, bytes.Length);
                    var response = reader.ReadToEnd();
                    if (response.Contains("301 Moved Permanently")){
                        var newUrl = string.Join("", response[(response.IndexOf("location: ") + "location: ".Length)..].TakeWhile(x => x != '\n')).Trim();
                        await ProcessRepositoriesAsync(new TcpClient(), $"{option} {newUrl}");
                        return;
                    }
                    var html = $"<{response}"[response.IndexOf("<html")..];
                    Console.WriteLine( html.GetTextFromHtml());
                }
                ,
                "s" => async () =>
                {
                    var host = $"www.google.com";
                    client.Connect(host, 80);
                    var message = @$"GET /search?q={string.Join("", options[1..])} HTTP/1.1
Accept: text/html, charset=utf-8
Connection: close
Host: {host}
" + "\r\n\r\n";
                    using NetworkStream networkStream = client.GetStream();
                    networkStream.ReadTimeout = 2000;
                    using var reader = new StreamReader(networkStream, Encoding.UTF8);
                    byte[] bytes = Encoding.UTF8.GetBytes(message);
                    networkStream.Write(bytes, 0, bytes.Length);
                    var response = reader.ReadToEnd();
                    var html = response[response.IndexOf("<!doctype html")..];
                    //var json = await client.GetStringAsync($"https://www.google.com/search?q={string.Join("", options[1..])}");
                    html.GetLinks();
                }
                ,
                _ => throw new Exception("Unknown Option")
            };
            await reqFunction();

        }
        catch (Exception ex)
        {

            Console.WriteLine($"Something bad happend.\tProbably {ex.Message} has something to do with it, but you can never be sure.");
        }
    }
    public static bool ValidateServerCertificate(object sender, X509Certificate certificate,
X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }

}