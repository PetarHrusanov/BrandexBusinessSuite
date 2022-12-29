namespace BrandexBusinessSuite.Services.Messages;

using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;

public class Publisher : IPublisher
{
    private const int TimeoutMilliseconds = 2000;

    private readonly IBus _bus;

    public Publisher(IBus bus) => this._bus = bus;

    public Task Publish<TMessage>(TMessage message)
        => _bus.Publish(message, GetCancellationToken());

    public Task Publish<TMessage>(TMessage message, Type messageType)
        => _bus.Publish(message, messageType, GetCancellationToken());

    private static CancellationToken GetCancellationToken()
    {
        var timeout = TimeSpan.FromMilliseconds(TimeoutMilliseconds);
        var cancellationTokenSource = new CancellationTokenSource(timeout);
        return cancellationTokenSource.Token;
    }
}