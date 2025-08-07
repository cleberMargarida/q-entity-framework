using Q.EntityFrameworkCore;

MyDbContext dbContext = new();

dbContext.Orders.Add(new Order(/*...*/));
dbContext.OrderEvents.Publish(new OrderPosted(/*...*/));

await dbContext.SaveChangesAsync();

await foreach (var item in dbContext.OrderEvents)
{
    //do something with consumed item
}
