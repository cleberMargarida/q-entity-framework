using Microsoft.EntityFrameworkCore;

namespace Q.EntityFrameworkCore;

public class MyDbContext : RMQDbContext
{
    public DbSet<Order> Orders { get; set; }

    //Messaging in runtime must have different types,
    //eg.: MessagingWithOutbox, MessagingWithInbox,
    //if needed to insert on db, by doing so we can use ef tracking
    //if plain Messaging, can just publish in fire and forget?
    public Messaging<OrderPosted> OrderEvents { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=MyDatabase;Integrated Security=True;");
        optionsBuilder.UseRabbitMQ("localhost:5674");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Message<OrderPosted>()
            .HasExchange(exchange: "a")
            //.BindingToExchange("b", bind => bind.HasRoutingKey(order => order.Id))
            .BindingToQueue("q");

        //modelBuilder
        //    .Message<OrderPosted>()
        //    .HasQueue(queue: "")
        //    .BindingFromExchange(exchange: "");
    }
}
