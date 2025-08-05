using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Q.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

MyDbContext dbContext = new();

dbContext.Orders.Add(new Order(/*...*/));
dbContext.OrderEvents.Publish(new OrderPosted(/*...*/));

await dbContext.SaveChangesAsync();

await foreach (var item in dbContext.OrderEvents)
{
    //do something with consumed item
}

namespace Q.EntityFrameworkCore
{
    public class MyDbContext : DbContext, IRMQDbContext
    {
        public DbSet<Order> Orders { get; set; }
        public Messaging<OrderPosted> OrderEvents { get; set; }

        public void OnMessagingCreating()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer("Server=localhost;Database=MyDatabase;User Id=myuser;Password=mypassword;");
            optionsBuilder.UseRabbitMQ("localhost:5674");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

    public static class DbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder UseRabbitMQ(this DbContextOptionsBuilder optionsBuilder, string connectionString)
        {
            // Here you would configure the RabbitMQ connection and other settings
            // For example, you might set up a connection string or other options
            return optionsBuilder;
        }
    }

    public interface IRMQDbContext
    {
        public void OnMessagingCreating();

        //IConnection Connection { get; set; }
    }

    public class Messaging<T> : IAsyncDisposable
    {
        public Exchange Exchange { get; set; }
        public Queue Queue { get; set; }

        public async IAsyncEnumerator<T> GetAsyncEnumerator()
        {
            throw new NotImplementedException();
        }

        public void Publish(T message)
        {

        }

        public async Task PublishAsync(T message)
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
