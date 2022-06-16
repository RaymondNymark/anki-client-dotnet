namespace AnkiWeb.Client.Helpers;
internal class AnkiWebConfig
{
    public string AnkiWebCsrfToken { get; set; } = string.Empty;
    public string AnkiUserCsrfToken { get; set; } = string.Empty;
    public bool AnkiCookieExists { get; set; } = false;

    public bool IsConfigured { get => (string.IsNullOrEmpty(AnkiWebCsrfToken) & string.IsNullOrEmpty(AnkiUserCsrfToken) & AnkiCookieExists); }

    public AnkiWebConfig()
    {

    }
}

