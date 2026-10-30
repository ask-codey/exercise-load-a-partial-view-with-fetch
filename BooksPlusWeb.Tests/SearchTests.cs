using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BooksPlusWeb.Tests;

// These tests start your whole web app in memory and request its pages, the way a browser would.
// The app reads your own BooksPlus database, so SQL Server must be running and BooksPlus created.
// You don't need to change this file.
[TestClass]
public class SearchTests
{
    private static WebApplicationFactory<Program> factory = null!;
    private static HttpClient browser = null!;

    // The BOOKSPLUS_CONNECTION environment variable, if set, points the tests
    // at another database; otherwise the app uses its own appsettings.json.
    private static string? OtherDatabase => Environment.GetEnvironmentVariable("BOOKSPLUS_CONNECTION");

    [ClassInitialize]
    public static void StartTheApp(TestContext context)
    {
        factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            if (OtherDatabase is not null)
            {
                builder.UseSetting("ConnectionStrings:BooksPlus", OtherDatabase);
            }
        });
        browser = factory.CreateClient();
    }

    [ClassCleanup]
    public static void StopTheApp()
    {
        browser.Dispose();
        factory.Dispose();
    }

    private static async Task<string> Get(string path)
    {
        HttpResponseMessage response = await browser.GetAsync(path);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, $"GET {path} should answer 200.");
        return await response.Content.ReadAsStringAsync();
    }

    private static int CountResults(string html)
    {
        return Regex.Matches(html, "class=\"list-group-item\"").Count;
    }

    [TestMethod]
    public async Task SearchResults_Text_AnswersOnlyTheResults()
    {
        string html = await Get("/books/search/results?q=SQL");

        Assert.DoesNotContain("<html", html, "The results are a partial view, without the layout.");
        Assert.Contains("28 books match \"SQL\".", html);
        Assert.AreEqual(28, CountResults(html));
    }

    [TestMethod]
    public async Task SearchResults_EachResult_LinksToItsBook()
    {
        string html = await Get("/books/search/results?q=PostgreSQL");

        Assert.MatchesRegex(new Regex("<a href=\"/books/\\d+\">Learning PostgreSQL 11</a>"), html);
    }

    [TestMethod]
    public async Task SearchResults_NoMatch_SaysSo()
    {
        string html = await Get("/books/search/results?q=zzz");

        Assert.Contains("0 books match \"zzz\".", html);
        Assert.AreEqual(0, CountResults(html));
    }

    [TestMethod]
    public async Task SearchResults_NoText_IsEmpty()
    {
        string html = await Get("/books/search/results?q=");

        Assert.AreEqual("", html.Trim());
    }

    [TestMethod]
    public async Task SearchPage_ShowsTheResultsInsideTheirBox()
    {
        string html = await Get("/books/search?q=SQL");

        Assert.Contains("<html", html, "The search page is still a whole page.");
        Match box = Regex.Match(html, "<div id=\"search-results\">([\\s\\S]*?)</ul>");
        Assert.IsTrue(box.Success, "The results are inside <div id=\"search-results\">.");
        Assert.AreEqual(28, CountResults(box.Value));
    }

    [TestMethod]
    public async Task SearchPage_LoadsTheSearchScript()
    {
        string html = await Get("/books/search");

        Assert.MatchesRegex(new Regex("<script src=\"/js/search\\.js(\\?v=[^\"]+)?\"></script>"), html);
        Assert.Contains("id=\"q\"", html, "The search box has the id q.");
    }

    [TestMethod]
    public async Task SearchScript_FetchesTheResultsAsTheUserTypes()
    {
        string script = await Get("/js/search.js");

        Assert.Contains("fetch(", script);
        Assert.Contains("/books/search/results", script);
        Assert.Contains("addEventListener('input'", script.Replace('"', '\''));
        Assert.Contains("encodeURIComponent", script);
    }
}
