using BrandexBusinessSuite.Services.Messages;

namespace BrandexBusinessSuite.Services.Data;

using System.Linq;
using System.Threading.Tasks;
using BrandexBusinessSuite.Data;
using BrandexBusinessSuite.Data.Models;
using Microsoft.EntityFrameworkCore;

public abstract class DataService<TEntity> : IDataService<TEntity>
    where TEntity : class
{
    protected DataService(DbContext data,
        IPublisher publisher
        )
    {
        this.Data = data;
        this.Publisher = publisher;
    }

    protected DbContext Data { get; }

    protected IPublisher Publisher { get; }

    protected IQueryable<TEntity> All() => this.Data.Set<TEntity>();

    public void Add(TEntity entity)
        => Data.Add(entity);

    public async Task Save(params object[] messages)
    {
        var dataMessages = messages
            .ToDictionary(data => data, data => new Message(data));

        if (Data is MessageDbContext)
        {
            foreach (var (_, message) in dataMessages)
            {
                Data.Add(message);
            }
        }

        await Data.SaveChangesAsync();

        if (this.Data is MessageDbContext)
        {
            foreach (var (data, message) in dataMessages)
            {
                await this.Publisher.Publish(data);
        
                message.MarkAsPublished();
        
                await this.Data.SaveChangesAsync();
            }
        }
    }
}