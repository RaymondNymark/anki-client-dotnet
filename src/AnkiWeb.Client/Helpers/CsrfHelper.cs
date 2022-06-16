using System.Text.RegularExpressions;
namespace AnkiWeb.Client.Helpers;
/// <summary>
/// Fetching the csrf_token using regex to locate it in the html content.
/// </summary>
internal static class CsrfHelper
{
    internal static string FindAnkiWebCsrfTokenInHtml(string htmlMarkdown)
    {
        Regex csrfRegex = new("<input type=\"hidden\" name=\"csrf_token\" value=\"(.*?)\">");
        string csrfToken = csrfRegex.Match(htmlMarkdown).Groups[1].ToString();

        return csrfToken ?? string.Empty;
    }

    internal static string FindAnkiUserCsrfTokenInHtml(string htmlMarkdown)
    {
        Regex csrfRegex = new(@"anki.Editor\('(.*?)', mode\)");
        string csrfToken = csrfRegex.Match(htmlMarkdown).Groups[1].ToString();

        return csrfToken ?? string.Empty;
    }
}

