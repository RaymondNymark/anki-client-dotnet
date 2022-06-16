using AnkiWeb.Client.Common.Models;
using AnkiWeb.Client.Common.Results;
using AnkiWeb.Client.Helpers;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AnkiWeb.Client;
public class AnkiClient : IAnkiClient
{
    private readonly HttpClient _httpClient;
    private readonly LoginCredentials _loginCredentials;
    private AnkiWebConfig _ankiWebConfig;

    public AnkiClient(HttpClient httpClient, LoginCredentials loginCredentials)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _httpClient.Timeout = TimeSpan.FromSeconds(15);
        _loginCredentials = loginCredentials;
        _ankiWebConfig = new();
    }

    // Finds 1st csrf token, finds 2nd csrf token, also calls a specific website for cookies, and tries to log in.
    private async Task<Result> ConfigureCookiesAndAuthorization()
    {
        if (_ankiWebConfig.IsConfigured)
        {
            return new SuccessResult();
        }
        try
        {
            AnkiWebConfig ankiWebConfig = new();

            var ankiWebRequest = new HttpRequestMessage(HttpMethod.Get, "https://ankiweb.net/account/login");

            using (var ankiWebResponse = await _httpClient.SendAsync(ankiWebRequest))
            {
                // Tries to find AnkiWebCsrfToken in the Html content.
                var htmlContent = await ankiWebResponse.Content.ReadAsStringAsync();
                var ankiWebCsrfToken = CsrfHelper.FindAnkiWebCsrfTokenInHtml(htmlContent);

                if (string.IsNullOrEmpty(ankiWebCsrfToken))
                {
                    return new CsrfErrorResult("Csrf token on ankiWeb was not found.");
                }
                ankiWebConfig.AnkiWebCsrfToken = ankiWebCsrfToken;

                // Required header values to make authorized get calls.
                Dictionary<string, string> headerValues = new()
                {
                    { "submitted", "1" },
                    { "csrf_token", ankiWebConfig.AnkiWebCsrfToken },
                    { "username", _loginCredentials.Username },
                    { "password", _loginCredentials.Password }
                };

                var requestHeaders = new FormUrlEncodedContent(headerValues);

                // Logs in, which genertes a cookie that's stored in the httpClient. 
                using (var loginResponse = await _httpClient.PostAsync("https://ankiweb.net/account/login", requestHeaders))
                {
                    string responsePath = loginResponse.RequestMessage.RequestUri.AbsolutePath;
                    if (responsePath == "/decks/")
                    {
                        // Cookie exists.
                        ankiWebConfig.AnkiCookieExists = true;
                    }
                    else
                    {
                        return new LoginErrorResult("Failed logging onto ankiweb.net. Check login details.");
                    }
                }
            }

            // Tries to find AnkiUserCsrfToken in the Html content.
            var ankiUserRequest = new HttpRequestMessage(HttpMethod.Get, "https://ankiuser.net/edit/");
            using (var ankiUserResponse = await _httpClient.SendAsync(ankiUserRequest))
            {
                var ankiUserHtmlContent = await ankiUserResponse.Content.ReadAsStringAsync();
                var ankiUserCsrfToken = CsrfHelper.FindAnkiUserCsrfTokenInHtml(ankiUserHtmlContent);

                if (string.IsNullOrEmpty(ankiUserCsrfToken))
                {
                    return new CsrfErrorResult("Csrf token on ankiUser was not found.");
                }
                ankiWebConfig.AnkiUserCsrfToken = ankiUserCsrfToken;
            }

            _ankiWebConfig = ankiWebConfig;
            return new SuccessResult();
        }
        catch (TimeoutException)
        {
            return new HttpTimeoutErrorResult($"The HTTP request to AnkiWeb took longer than {_httpClient.Timeout.TotalSeconds} seconds. AnkiWeb may be down.");
        }
        catch (HttpRequestException e)
        {
            return new ErrorResult($"HttpRequestException! Status: {e.StatusCode}");
        }
        catch (Exception e)
        {
            return new ErrorResult($"Unexpected error: {e.Message}");
        }
    }

    private async Task<Result> EnsureClientIsConfiguredAsync()
    {
        // Retry policy?
        var result = await ConfigureCookiesAndAuthorization();

        return result switch
        {
            SuccessResult successResult => result,
            LoginErrorResult loginErrorResult => result,
            _ => await HandleRetryClientIsConfiguredAsync(result)
        };
    }

    private async Task<Result> HandleRetryClientIsConfiguredAsync(Result result, int retries = 2, int delayLenght = 5000)
    {
        for (int i = 1; i < retries; i++)
        {
            await Task.Delay(delayLenght);
            var newResult = await ConfigureCookiesAndAuthorization();

            if (newResult.Success)
            {
                return newResult;
            }
        }

        return new ErrorResult("Configuring client failed.");
    }

    private async Task<bool> IsUserLoggedIn()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://ankiweb.net/decks/");
        using (var response = await _httpClient.SendAsync(request))
        {
            var responseAsHtml = await response.Content.ReadAsStringAsync();
            Regex loggedInRegex = new(@"(?:^|\W)Log out(?:$|\W)");
            return loggedInRegex.IsMatch(responseAsHtml);
        }
    }

    public async Task<(CollectionInfo collectionInfo, AnkiClientStatus status)> GetCollectionInfoAsync()
    {
        Result result = await EnsureClientIsConfiguredAsync();
        if (result.Success)
        {
            using (var response = await _httpClient.GetAsync("https://ankiuser.net/edit/getAddInfo"))
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var collectionInfo = JsonSerializer.Deserialize<CollectionInfo>(jsonResponse) ?? new();

                return (collectionInfo, new(success: true, ""));
            }
        }

        return result switch
        {
            ErrorResult errorResult => (new(), new(false, $"Failed getting collection. Error: {errorResult.Message}")),
            _ => (new(), new(false, $"Failed getting collection. Unknown error."))
        };
    }

    private async Task<Result> AddNewCardToDeck(Card card, string deckId)
    {
        Result result = await EnsureClientIsConfiguredAsync();
        if (result.Success)
        {
            Result CardIsValid = await ValidateCardFields(card);
            if (!CardIsValid.Success)
            {
                return new InvalidCardResult("");
            }

            Dictionary<string, string?> headerValues = new()
            {
                { "nid", null },
                { "data", card.EncodedData },
                { "csrf_token", _ankiWebConfig.AnkiUserCsrfToken },
                { "mid", card.TypeId }, // mid = typeID.
                { "deck", deckId }
            };

            var requestHeaders = new FormUrlEncodedContent(headerValues);

            // TODO verifying the card was actually added.
            using (var response = await _httpClient.PostAsync("https://ankiuser.net/edit/save", requestHeaders))
            {
                response.EnsureSuccessStatusCode();
                return new SuccessResult();
            }
        }
        else
        {
            return result;
        }

    }

    private async Task<Result> ValidateCardFields(Card card)
    {
        Result result = await EnsureClientIsConfiguredAsync();
        if (result.Success)
        {
            // Gets how many fields a specific type ID is supposed to have.
            // Amount of fields type ID expects, and fields in card need to match.
            using (var response = await _httpClient.GetAsync($"https://ankiuser.net/edit/getNotetypeFields?ntid={card.TypeId}"))
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                NoteTypeFields noteTypeFields = JsonSerializer.Deserialize<NoteTypeFields>(jsonResponse) ?? new();
                int fieldCount = noteTypeFields.Fields.Count;

                int inputFieldCount = card.Fields.Count;

                if (fieldCount == inputFieldCount)
                {
                    return new SuccessResult();
                }
                else
                {
                    return new ErrorResult($"The specific type requires {fieldCount} fields to be added. Only {inputFieldCount} fields were provided.");
                }
            };

        }
        else
        {
            return result;
        }
    }

    public async Task<AnkiClientStatus> AddNewCardToDeckAsync(Card card, string deckId)
    {
        var result = await AddNewCardToDeck(card, deckId);

        return result switch
        {
            SuccessResult successResult => new(true, "Added new card to deck."),
            ErrorResult errorResult => new(false, $"Failed to add new card. Reason: {errorResult.Message}"),
            _ => new(false, "Failed to add new card to deck. Unknown error.")
        };
    }
}

