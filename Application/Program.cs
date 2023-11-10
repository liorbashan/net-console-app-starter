
using Application;
using Application.Models.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Application.Abstractions;
using Application.Services;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()

    .Build();

var serviceCollection = new ServiceCollection();
serviceCollection
.Configure<ApplicationSettings>(options =>
{
    configuration.GetSection("ApplicationSettings").Bind(options);
})
.AddSingleton<IConfiguration>(configuration)
.AddSingleton<IApiClient, ApiClient>()
.AddSingleton<App>()
.AddLogging(logBuilder => logBuilder.AddConsole())
.AddHttpClient("HttpClient", (sp, client) =>
{
    //client.DefaultRequestHeaders.Accept.Clear();
    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    var settings = sp.GetService<IOptions<ApplicationSettings>>()!.Value;
    client.BaseAddress = new Uri(settings.BaseUrl ?? "");
    var bearerToken = settings.BearerToken ?? "defalutBearerToken";
    if (!string.IsNullOrEmpty(bearerToken))
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
    }
});

serviceCollection.RemoveAll<IHttpMessageHandlerBuilderFilter>();

var serviceProvider = serviceCollection.BuildServiceProvider();

await serviceProvider.GetService<App>()!.Run();

