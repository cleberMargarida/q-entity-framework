using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Q.EntityFrameworkCore
{

    public static class ModelBuilderExtensions
    {
        public static MessageTypeBuilder<T> Message<T>(this ModelBuilder modelBuilder)
        {
            // Here you would configure the messaging settings for the entity
            // For example, you might set up an exchange and queue for RabbitMQ
            throw new NotImplementedException("This method should configure the messaging settings for the entity.");
        }
    }

    public class MessageTypeBuilder<T>
    {
        public ExchangeTypeBuilder<T> HasExchange(string exchange)
        {
            throw new NotImplementedException();
        }

        internal QueueTypeBuilder<T> HasQueue(string queue)
        {
            throw new NotImplementedException();
        }

        public ExchangeTypeBuilder<T> HasTransactionalInbox()
        {
            throw new NotImplementedException();
        }

        public ExchangeTypeBuilder<T> HasTransactionalOutbox()
        {
            throw new NotImplementedException();
        }
    }

    public class QueueTypeBuilder<T>
    {
        internal void BindingFromExchange(string exchange, Action<BindExchangeBuilder<T>> action = null)
        {
            throw new NotImplementedException();
        }
    }


    public class ExchangeTypeBuilder<T>
    {
        public ExchangeTypeBuilder<T> HasExchange(string exchange)
        {
            throw new NotImplementedException();
        }

        public ExchangeTypeBuilder<T> BindingToExchange(string exchange, Action<BindExchangeBuilder<T>> action = null)
        {
            throw new NotImplementedException();
        }

        public ExchangeTypeBuilder<T> BindingToQueue(string queue, Action<BindExchangeBuilder<T>> action = null)
        {
            throw new NotImplementedException();
        }
    }

    public class BindExchangeBuilder<T>
    {
        internal BindExchangeBuilder<T> HasRoutingKey<TProp>(Expression<Func<T, TProp>> routingKeyProperty)
        {
            throw new NotImplementedException();
        }

        public BindExchangeBuilder<T> HasArgument(string key, string value)
        {
            throw new NotImplementedException();
        }
    }

    public static class DbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder UseRabbitMQ(this DbContextOptionsBuilder optionsBuilder, string connectionString)
        {
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(new RabbitMQDbContextOptionsExtension(connectionString));
            return optionsBuilder;
        }
    }

    internal class RabbitMQDbContextOptionsExtension : IDbContextOptionsExtension
    {
        private readonly string connectionString;

        public RabbitMQDbContextOptionsExtension(string connectionString)
        {
            this.connectionString = connectionString;
            Info = new RabbitMQDbContextOptionsExtensionInfo(this);
        }

        public DbContextOptionsExtensionInfo Info { get; }

        public void ApplyServices(IServiceCollection services)
        {
            services.AddSingleton(() =>
            {
                var factory = new ConnectionFactory { Uri = new Uri(connectionString) };
                return new ConnectionHolder(factory.CreateConnectionAsync);
            });
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedService, ConnectionInitializer>());
        }

        public void Validate(IDbContextOptions options)
        {
        }
    }

    internal class ConnectionHolder(Func<CancellationToken, Task<IConnection>> createConnectionAsync) : IAsyncDisposable
    {
        [MaybeNull]
        public IConnection Connection { get; private set; }

        public async Task<IConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
        {
            return Connection ??= await createConnectionAsync(cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            if (Connection is null)
                return;

            await Connection.DisposeAsync();
        }
    }

    internal class ConnectionInitializer(ConnectionHolder connectionHolder) : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return connectionHolder.GetConnectionAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    internal class RabbitMQDbContextOptionsExtensionInfo(IDbContextOptionsExtension extension) : DbContextOptionsExtensionInfo(extension)
    {
        public override bool IsDatabaseProvider => false;

        public override string LogFragment => string.Empty;

        public override int GetServiceProviderHashCode()
        {
            return 0;
        }

        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
        {
        }

        public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
        {
            return true;
        }
    }

    public abstract class RMQDbContext : DbContext
    {
        public RMQDbContext() : this(new DbContextOptions<DbContext>())
        {
        }

        public RMQDbContext(DbContextOptions options) : base(options)
        {
            InitializeMessaging(this);
        }

        private static void InitializeMessaging(RMQDbContext context)
        {
            throw new NotImplementedException();
            //foreach (var setInfo in _setFinder.FindSets(context.GetType()).Where(p => p.Setter != null))
            //{
            //    setInfo.Setter!.SetClrValue(
            //        context,
            //        ((IDbSetCache)context).GetOrAddSet(_setSource, setInfo.Type));
            //}
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            var connection = this.GetInfrastructure().GetRequiredService<ConnectionHolder>().Connection;
            using var channel = connection.CreateChannelAsync().GetAwaiter().GetResult();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            var connection = this.GetInfrastructure().GetRequiredService<ConnectionHolder>().Connection;
            await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);
            //channel.BasicPublishAsync();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        //IConnection Connection { get; set; }
    }

    public class Messaging<T> : IAsyncDisposable
    {
        public Exchange Exchange { get; set; }
        public Queue Queue { get; set; }

        public IAsyncEnumerator<T> GetAsyncEnumerator()
        {
            throw new NotImplementedException();
        }

        public void Publish(T message)
        {

        }

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }
    }

    public class Exchange { }
    public class Queue { }

    public class Order
    {
        public Guid Id { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class OrderPosted
    {
        public Guid Id { get; set; }
        public DateTime PostedAt { get; set; }
    }
}
