using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using UserService.Model;

namespace UserService.Service;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly string _authVerify;

    public AuthService(IHttpClientFactory httpClientFactory, IOptions<AuthServiceSettings> authServiceSetting)
    {
        _httpClient = httpClientFactory.CreateClient();
        _authVerify = authServiceSetting.Value.AuthServiceVerify;
    }

    public async Task<(bool, Guid)> Verify(string bearerToken)
    {
        if (string.IsNullOrEmpty(bearerToken))
        {
            return (false, Guid.Empty);
        }
        var splitted = bearerToken.Split(' ');
        if (splitted.Length != 2)
        {
            return (false, Guid.Empty);
        }

        var jsonBody = JsonContent.Create(new { token = splitted[1] });

        var response = await _httpClient.PostAsync(_authVerify, jsonBody);

        if (!response.IsSuccessStatusCode)
        {
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
                Console.WriteLine("Invalid JSON.");
            }
        }
        return (true, userId);

    }

    private class VerifyId
    {
        public string Id { get; set; } = null!;
    }
}