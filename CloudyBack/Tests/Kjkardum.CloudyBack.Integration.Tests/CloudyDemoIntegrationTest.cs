using Kjkardum.CloudyBack.Application.Response;
using Kjkardum.CloudyBack.Application.UseCases.User.Commands.Create;
using Kjkardum.CloudyBack.Application.UseCases.User.Commands.Login;
using Kjkardum.CloudyBack.Application.UseCases.User.Commands.Update;
using Kjkardum.CloudyBack.Application.UseCases.User.Dto;
using FluentAssertions;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Kjkardum.CloudyBack.Integration.Tests;

public class AlphabeticalOrderer : ITestCaseOrderer
{
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
            where TTestCase : ITestCase
    {
        var result = testCases.ToList();
        result.Sort(
            (x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.TestMethod.Method.Name, y.TestMethod.Method.Name));
        return result;
    }
}

[TestCaseOrderer("Kjkardum.CloudyBack.Integration.Tests.AlphabeticalOrderer", "Kjkardum.CloudyBack.Integration.Tests")]
public class CloudyDemoIntegrationTest : IClassFixture<WebHostTestBase>
{
    private readonly WebHostTestBase _testBase;

    public JsonSerializerOptions JsonOptions { get; set; }
        = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

    public CloudyDemoIntegrationTest(WebHostTestBase fixture)
    {
        _testBase = fixture;
    }

    [Fact]
    public async Task Test01_Login()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/Authentication/Login");
        request.Content = JsonContent.Create(new UserLoginCommand { Email = "user@cloudy.com", Password = "Pa$$w0rd" });

        // Act
        var response = await _testBase.Client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        responseString.Should().NotBeNullOrEmpty();
        var responseDto = JsonSerializer.Deserialize<LoggedInUserDto>(responseString, JsonOptions);
        responseDto.Should().NotBeNull();
        responseDto!.Token.Should().NotBeNullOrEmpty();
        _testBase.JwToken = responseDto.Token;
        _testBase.CachedValues.Add("JwToken", responseDto.Token);
        _testBase.CachedValues.Add("UserId", responseDto.Id.ToString());
    }

    [Fact]
    public async Task Test02_UdateUser()
    {
        // Arrange
        var request = new HttpRequestMessage(
            HttpMethod.Put,
            $"/api/TenantManagement/updateUser/{_testBase.CachedValues["UserId"]}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _testBase.JwToken);
        request.Content = JsonContent.Create(new UserUpdateCommand
            {
                RequestorId = Guid.Parse(_testBase.CachedValues["UserId"]!),
                Id = Guid.Parse(_testBase.CachedValues["UserId"]!),
                NewPassword = null,
            });

        // Act
        var response = await _testBase.Client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Test03_CreateUser()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/TenantManagement/createUser");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _testBase.JwToken);
        request.Content = JsonContent.Create(new UserRegistrationCommand
            {
                RequestorId = Guid.Parse(_testBase.CachedValues["UserId"]!),
                Email = "seconduser@cloudy.com",
                Password = "Pa$$w0rd",
            });

        // Act
        var response = await _testBase.Client.SendAsync(request);

        // Assert
        response.EnsureSuccessStatusCode();
    }
}
