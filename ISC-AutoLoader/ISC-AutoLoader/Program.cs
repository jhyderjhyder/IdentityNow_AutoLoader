using ISC_AutoLoader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

foreach (var arg in Environment.GetCommandLineArgs())
{
    if (arg.ToLower().Equals("-v"))
    {
        Console.WriteLine("Version 11-07-2024");
        Environment.Exit(0);
    }
    if (arg.ToLower().Equals("-e"))
    {
        UserInputs u = new UserInputs();
        String key = null;
        if (u.config.ContainsKey("EncryptKey"))
        {
            key = u.config["EncryptKey"];
        } 
        Encrypt e = new Encrypt(key); ;
        Console.WriteLine("Provide the value to encrypt");
        var input = Console.ReadLine();
        var result = e.encrypt(input);
        Console.WriteLine(result);
        Environment.Exit(0);
    }

    if (arg.ToLower().Equals("-d"))
    {
        UserInputs u = new UserInputs();
        String key = null;
        if (u.config.ContainsKey("EncryptKey"))
        {
            key = u.config["EncryptKey"];
        }
        Encrypt e = new Encrypt(key);
        Console.WriteLine("Provide the value to Decrypt");
        var input = Console.ReadLine();
        var result = e.decrypt(input);
        Console.WriteLine(result);
        Environment.Exit(0);
    }

}

IHost host = Host.CreateDefaultBuilder(args).UseWindowsService(options =>
{
    options.ServiceName = "ISC AutoLoader";

})
    .ConfigureServices(services =>
{
    services.AddHostedService<FileScanner>();
})
.Build();

await host.RunAsync();  