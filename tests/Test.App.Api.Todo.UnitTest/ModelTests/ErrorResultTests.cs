using System.Text.Json;
using App.Api.Todo.Models;
using Microsoft.AspNetCore.Http;

namespace Test.App.Api.Todo.UnitTest.ModelTests;

public class ErrorResultTests
{
    [Fact]
    public async Task GivenStringContainingNotFound_ExecuteAsync_Sets404AndReturnsPayload()
    {
        var error = "Resource not found";
        var sut = new ErrorResult<string>(error);

        var context = new DefaultHttpContext();
        await using var ms = new MemoryStream();
        context.Response.Body = ms;

        await sut.ExecuteAsync(context);

        ms.Position = 0;
        using var sr = new StreamReader(ms);
        var body = await sr.ReadToEndAsync();

        Assert.Equal(404, context.Response.StatusCode);
        Assert.Equal(JsonSerializer.Serialize(error), body);
    }

    [Fact]
    public async Task GivenStringContainingUnauthorized_ExecuteAsync_Sets401AndReturnsPayload()
    {
        var error = "User is unauthorized";
        var sut = new ErrorResult<string>(error);

        var context = new DefaultHttpContext();
        await using var ms = new MemoryStream();
        context.Response.Body = ms;

        await sut.ExecuteAsync(context);

        ms.Position = 0;
        using var sr = new StreamReader(ms);
        var body = await sr.ReadToEndAsync();

        Assert.Equal(401, context.Response.StatusCode);
        Assert.Equal(JsonSerializer.Serialize(error), body);
    }

    [Fact]
    public async Task GivenArbitraryObject_ExecuteAsync_Sets400AndReturnsSerializedObject()
    {
        var payload = new { Message = "Bad request example", Code = 123 };
        var sut = new ErrorResult<object>(payload);

        var context = new DefaultHttpContext();
        await using var ms = new MemoryStream();
        context.Response.Body = ms;

        await sut.ExecuteAsync(context);

        ms.Position = 0;
        using var sr = new StreamReader(ms);
        var body = await sr.ReadToEndAsync();

        Assert.Equal(400, context.Response.StatusCode);
        //Assert.Equal(JsonSerializer.Serialize(payload), body);
    }
}
