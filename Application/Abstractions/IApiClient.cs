
namespace Application.Abstractions
{
    public interface IApiClient
    {
        public Task<HttpResponseMessage> GetAsync(string requestUrl);
        public Task<HttpResponseMessage> PostAsync<T>(string requestUrl, T payload);
        public void SetBearerToken(string token);
    }
}
