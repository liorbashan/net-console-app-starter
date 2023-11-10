using Application.Abstractions;
using Application.Models.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application
{
    public class App
    {
        private readonly IApiClient _apiClient;
        private ApplicationSettings _appSettings;
        private const string REQUEST_PATH = "/food/enforcement.json?search=report_date:[20120725+TO+20120725]&limit=1000&sort=recall_initiation_date:[asc]";
        private ILogger<App> _logger;
        public App(IApiClient apiClient, ILogger<App> logger, IOptions<ApplicationSettings> appSettings)
        {
            _apiClient = apiClient;
            _logger = logger;
            _appSettings = appSettings.Value;
        }

        public async Task Run()
        {
            try
            {
                var response = await _apiClient.GetAsync($"{_appSettings.BaseUrl}{REQUEST_PATH}");
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Response Content: {content}");
                response.Dispose();
            }
            catch
            {
                throw;
            }
        }
    }
}
