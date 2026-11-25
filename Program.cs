using System.Text;
using Microsoft.EntityFrameworkCore;
using PlatformWellSync.Data;
using PlatformWellSync.Services;

const string baseUrl = "http://test-demo.aemenersol.com";

bool useDummy = args.Contains("dummy");
string endpoint = useDummy
    ? "/api/PlatformWell/GetPlatformWellDummy"
    : "/api/PlatformWell/GetPlatformWellActual";

string username = Environment.GetEnvironmentVariable("AEM_API_USERNAME") ?? Ask("Username: ");
string password = Environment.GetEnvironmentVariable("AEM_API_PASSWORD") ?? AskPassword("Password: ");

try
{
    await using var db = new AppDbContext();
    await db.Database.MigrateAsync();

    var api = new AemApiClient(baseUrl);

    Console.WriteLine("Logging in...");
    await api.LoginAsync(username, password);

    Console.WriteLine($"Getting data from {endpoint}...");
    var platforms = await api.GetPlatformWellsAsync(endpoint);
    Console.WriteLine($"Received {platforms.Count} platforms.");

    await new SyncService(db).SyncAsync(platforms);
    Console.WriteLine("Sync complete!");
}
catch (Exception ex)
{
    Console.WriteLine($"Something went wrong: {ex.Message}");
}

static string Ask(string label)
{
    Console.Write(label);
    return Console.ReadLine()?.Trim() ?? "";
}

static string AskPassword(string label)
{
    Console.Write(label);
    var password = new StringBuilder();
    ConsoleKeyInfo key;
    while ((key = Console.ReadKey(intercept: true)).Key != ConsoleKey.Enter)
    {
        if (key.Key == ConsoleKey.Backspace)
        {
            if (password.Length > 0)
            {
                password.Length--;
                Console.Write("\b \b");
            }
        }
        else if (!char.IsControl(key.KeyChar))
        {
            password.Append(key.KeyChar);
            Console.Write("*");
        }
    }
    Console.WriteLine();
    return password.ToString();
}
