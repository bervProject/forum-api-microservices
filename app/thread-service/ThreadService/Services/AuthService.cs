using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using ThreadService.Model;

namespace ThreadService.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly string _authVerify;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IHttpClientFactory httpClientFactory, IOptions<AuthServiceSettings> authServiceSetting, ILogger<AuthService> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _authVerify = authServiceSetting.Value.AuthServiceVerify;
        _logger = logger;
    }

    public async Task<(bool, Guid)> Verify(string bearerToken)
    {
        if (string.IsNullOrEmpty(bearerToken))
        {
            _logger.LogDebug("Getting empty bearer.");
            return (false, Guid.Empty);
        }
        var splitted = bearerToken.Split(' ');
        if (splitted.Length != 2)
        {
            _logger.LogDebug("The Bearer Not Valid.");
            return (false, Guid.Empty);
        }

        var jsonBody = JsonContent.Create(new { token = splitted[1] });

        var response = await _httpClient.PostAsync(_authVerify, jsonBody);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogDebug("Getting non 200 response from Auth Service. Response: {}", response.StatusCode);
            return (false, Guid.Empty);
        }

        Guid userId = Guid.Empty;

        if (response.Content is object && response.Content.Headers.ContentType != null && response.Content.Headers.ContentType.MediaType == "application/json")
        {
            var contentStream = await response.Content.ReadAsStreamAsync();
            using var streamReader = new StreamReader(contentStream);
            using var jsonReader = new JsonTextReader(streamReader);

            JsonSerializer serializer = new JsonSerializer();

            try
            {
                var responseData = serializer.Deserialize<VerifyId>(jsonReader);
                if (responseData != null)
                {
                    var byteArray = Utils.StringToByteArrayFastest(responseData.Id);
                    userId = new Guid(byteArray);
                }
            }
            catch (JsonReaderException)
            {
                _logger.LogDebug("Invalid JSON from Auth Service.");
            }
        }
        return (true, userId);

    }

    private class VerifyId
    {
        public string Id { get; set; } = null!;
    }
}