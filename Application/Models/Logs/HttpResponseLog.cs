namespace Application.Models.Logs
{
    public class HttpResponseLog
    {
        private HttpResponseMessage _httpResponse;
        public int? ResponseStatusCode { get; set; }
        public string? ResponseHeaders { get; set; }
        public string? ResponseContent { get; set; }
        public string? RequestUrl { get; set; }
        public string? RequestHeaders { get; set; }
        public string? RequestBody { get; set; }
        public HttpResponseLog(HttpResponseMessage httpResponse)
        {
            _httpResponse = httpResponse;

            ResponseStatusCode = (int?)_httpResponse.StatusCode;
            ResponseHeaders = _httpResponse.Headers.ToString();
            ResponseContent = _httpResponse.Content.ReadAsStringAsync().Result;

            RequestUrl = _httpResponse.RequestMessage?.RequestUri?.AbsoluteUri?.ToString();
            RequestHeaders = _httpResponse.RequestMessage?.Headers?.ToString();
            RequestBody = _httpResponse.RequestMessage?.Content?.ReadAsStringAsync().Result;
        }
    }
}
