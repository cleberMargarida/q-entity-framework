using Microsoft.EntityFrameworkCore;
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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer("Server=localhost;Database=MyDatabase;User Id=myuser;Password=mypassword;");
            optionsBuilder.UseRabbitMQ("localhost:5674");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .HasData();

            modelBuilder.Message<OrderPosted>()
                .HasExchange(exchange: "");
        }
    }

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
        internal void HasExchange(string exchange)
        {
            throw new NotImplementedException();
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
