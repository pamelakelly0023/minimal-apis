using Microsoft.EntityFrameworkCore;
using MinimalApi.Models;

namespace MinimalApi.Persistence;

class MinimalApiDbContext : DbContext
{
    public DbSet<Todo> Todos => Set <Todo>();

    public MinimalApiDbContext(DbContextOptions<MinimalApiDbContext> options)
        :base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        =>   modelBuilder.Entity<Todo>(x => x.HasKey(x => x.Id));
    
}