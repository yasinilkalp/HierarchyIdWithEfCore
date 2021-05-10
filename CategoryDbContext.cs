using Microsoft.EntityFrameworkCore;

public class CategoryDbContext : DbContext
{
    public DbSet<Category> Categories { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        var connectionString = @"Data Source=DESKTOP-GKVEQ3F; Initial Catalog=TestHierarchyId; Integrated Security=True; Persist Security Info=False";
        options.UseSqlServer(connectionString, conf =>
        {
            conf.UseHierarchyId();
        });

    }
}