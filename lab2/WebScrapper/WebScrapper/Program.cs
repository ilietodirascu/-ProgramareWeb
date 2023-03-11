using System.Net.Http.Headers;
using WebScrapper.Extensions;

internal class Program
{
    public static async Task Main(string[] args)
    {
        using HttpClient client = new();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
        client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
       
        await ProcessRepositoriesAsync(client, string.Join(" ", args)); 
    }
    static async Task ProcessRepositoriesAsync(HttpClient client, string args)
    {
        try
        {
            var options = args.Split(" ");
            var option = options[0];
            Func<Task> reqFunction = option switch
            {
                "u" => async () =>
                {
                    var url = options[1];
                    var json = await client.GetStringAsync(url);
                    Console.WriteLine(json.GetTextFromHtml());
                }
                ,
                "s" => async () =>
                {
                    var json = await client.GetStringAsync($"https://www.google.com/search?q={string.Join("", options[1..])}");
                    json.GetLinks();
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
}