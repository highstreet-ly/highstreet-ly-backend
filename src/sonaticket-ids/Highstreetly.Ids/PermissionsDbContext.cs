namespace Highstreetly.Ids
{
    // public class IdsDbContext : IdentityDbContext<User, Role, Guid, Claim, UserRole, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
    // {
    //     public IdsDbContext(DbContextOptions<IdsDbContext> options)
    //         : base(options)
    //     {
    //
    //     }
    //
    //     protected override void OnModelCreating(ModelBuilder builder)
    //     {
    //         base.OnModelCreating(builder);
    //         // Customize the ASP.NET Identity model and override the defaults if needed.
    //         // For example, you can rename the ASP.NET Identity table names and more.
    //         // Add your customizations after calling base.OnModelCreating(builder);
    //         
    //         builder.Entity<Claim>()
    //             .HasOne<User>(x => x.User)
    //             .WithMany(x => x.Claims)
    //             .HasForeignKey(x => x.UserId);
    //
    //         builder.Entity<UserRole>()
    //             .ToTable("AspNetUserRoles")
    //             .HasKey(sc => new { sc.UserId, sc.RoleId });
    //
    //         builder.Entity<UserRole>()
    //             .HasOne<User>(sc => sc.User)
    //             .WithMany(s => s.UserRoles)
    //             .HasForeignKey(sc => sc.UserId);
    //
    //         builder.Entity<UserRole>()
    //             .HasOne<Role>(sc => sc.Role)
    //             .WithMany(s => s.UserRoles)
    //             .HasForeignKey(sc => sc.RoleId);
    //
    //         builder.HasPostgresExtension("uuid-ossp")
    //             .Entity<User>()
    //             .Property(e => e.Id)
    //             .HasDefaultValueSql("uuid_generate_v4()");
    //     }
    // }
}