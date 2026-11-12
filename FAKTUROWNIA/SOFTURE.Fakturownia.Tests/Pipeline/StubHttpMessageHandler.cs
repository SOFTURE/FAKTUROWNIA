using System.Net;
using System.Text;

namespace SOFTURE.Fakturownia.Tests.Pipeline;

internal sealed class StubHttpMessageHandler : HttpMessageHandler
{
    private readonly Queue<Func<HttpResponseMessage>> _responses = new();

    public List<HttpRequestMessage> Requests { get; } = [];

    public void RespondWith(HttpStatusCode statusCode, string body, string mediaType = "application/json")
    {
        _responses.Enqueue(() => new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(body, Encoding.UTF8, mediaType)
        });
    }

    public void RespondWithEmptyBody(HttpStatusCode statusCode)
    {
        _responses.Enqueue(() => new HttpResponseMessage(statusCode)
        {
            Content = new ByteArrayContent([])
        });
    }

    public void FailWith(Exception exception)
    {
        _responses.Enqueue(() => throw exception);
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Requests.Add(request);

        if (_responses.Count == 0)
        {
            throw new InvalidOperationException($"No stubbed response left for {request.Method} {request.RequestUri}");
        }

        var response = _responses.Dequeue().Invoke();

        response.RequestMessage = request;

        return Task.FromResult(response);
    }
}
