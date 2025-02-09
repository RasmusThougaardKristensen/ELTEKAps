using ELTEKAps.Management.Infrastructure.Repositories.Comments;
using ELTEKAps.Management.Infrastructure.Repositories.Customers;
using ELTEKAps.Management.Infrastructure.Repositories.Operations;
using ELTEKAps.Management.Infrastructure.Repositories.Photos;
using ELTEKAps.Management.Infrastructure.Repositories.Users;
using Microsoft.EntityFrameworkCore;

namespace ELTEKAps.Management.Infrastructure.Repositories.Tasks;
public class TaskContext : DbContext
{
    public TaskContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<OperationEntity>? Operations { get; set; }
    public DbSet<TaskEntity>? Tasks { get; set; }
    public DbSet<CommentEntity>? Comments { get; set; }
    public DbSet<PhotosEntity>? Photos { get; set; }
    public DbSet<UserEntity>? Users { get; set; }
    public DbSet<CustomerEntity>? Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new OperationEntityConfiguration());
        modelBuilder.ApplyConfiguration(new TaskConfiguration());
        modelBuilder.ApplyConfiguration(new CommentConfiguration());
        modelBuilder.ApplyConfiguration(new PhotoConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerConfiguration());

    }
}
