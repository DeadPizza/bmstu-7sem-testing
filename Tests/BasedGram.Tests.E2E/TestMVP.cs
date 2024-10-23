using System.Net;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
namespace BasedGram.Tests.E2E;

public class TestMVP : IClassFixture<PgWebApplicationFactory<Program>>
{
    private readonly PgWebApplicationFactory<Program> _factory;

    public TestMVP(PgWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task RegisterUser_Ok()
    {
        var client = _factory.CreateClient();

        var request = new
        {
            login = "string",
            password = "string"
        };
        var httpContent = new StringContent(
            JsonConvert.SerializeObject(request),
            Encoding.UTF8,
            "application/json"
        );
        var response = await client.PostAsync(
            "/api/v2/auth",
            httpContent
        );
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var str = await response.Content.ReadAsStringAsync();
        string? id,
            token;
        using (JsonDocument doc = JsonDocument.Parse(str))
        {
            JsonElement root = doc.RootElement;
            token = root.GetProperty("jwt_token").GetString()!;
        }
    }
}