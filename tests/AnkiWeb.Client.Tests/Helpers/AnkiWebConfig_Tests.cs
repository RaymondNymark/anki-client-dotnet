using AnkiWeb.Client.Helpers;

namespace AnkiWeb.Client.Tests.Helpers;

public class AnkiWebConfig_Tests
{
    [Fact]
    public void IsConfigured_Returns_Correct_Value()
    {
        AnkiWebConfig config = new();

        config.AnkiCookieExists = true;
        config.AnkiWebCsrfToken = "12345";
        config.AnkiUserCsrfToken = "12345";

        Assert.True(config.IsConfigured);
    }
}
