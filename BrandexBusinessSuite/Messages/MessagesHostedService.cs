namespace BrandexBusinessSuite.Messages;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Data.Models;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.Messages;

public class MessagesHostedService : IHostedService
{
    private readonly IRecurringJobManager _recurringJob;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IPublisher _publisher;

    public MessagesHostedService(
        IRecurringJobManager recurringJob,
        IServiceScopeFactory serviceScopeFactory,
        IPublisher publisher)
    {
        _recurringJob = recurringJob;
        _serviceScopeFactory = serviceScopeFactory;
        _publisher = publisher;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var data = scope.ServiceProvider.GetService<DbContext>();

        if (data != null && !data.Database.CanConnect())
        {
            data.Database.EnsureCreated();
        }

        _recurringJob.AddOrUpdate(
            nameof(MessagesHostedService),
            () => this.ProcessPendingMessages(),
            "*/5 * * * * *");

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;

    public void ProcessPendingMessages()
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var data = scope.ServiceProvider.GetService<DbContext>();

        var messages = data
            .Set<Message>()
            .Where(m => !m.Published)
            .OrderBy(m => m.Id)
            .ToList();

        foreach (var message in messages)
        {
            _publisher
                .Publish(message.Data, message.Type)
                .GetAwaiter()
                .GetResult();

            message.MarkAsPublished();

            data.SaveChanges();
        }
    }
}