using classmaker_models.Entities;
using Microsoft.EntityFrameworkCore;

namespace classmaker_models
{
    public class EntityContext : DbContext
    {
        public EntityContext(DbContextOptions<EntityContext> options) : base(options)
        {
            
        }
        
        public DbSet<Student> Students { get; set; }
        public DbSet<Classroom> Classrooms { get; set; }
        public DbSet<User> Users { get; set; }
    }
}