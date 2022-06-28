using System;
using System.Linq;
using Highstreetly.Infrastructure.Extensions;
using Highstreetly.Permissions.Resources;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Highstreetly.Ids
{
    public static class SeedData
    {
        public static void EnsureSeedData(IServiceProvider serviceProvider)
        {
            Console.WriteLine("Seeding database...");

            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                //scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                var idsApplicationDbContext = scope.ServiceProvider.GetRequiredService<Permissions.PermissionsDbContext>();
                var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
                //context.Database.Migrate();
                try
                {
                    EnsureSeedData(context, roleManager, idsApplicationDbContext, serviceProvider.GetService<IConfiguration>());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            Console.WriteLine("Done seeding database.");
            Console.WriteLine();
        }

        private static void EnsureSeedData(ConfigurationDbContext context,
            RoleManager<Role> roleManager,
            Permissions.PermissionsDbContext ids,
            IConfiguration configuration)
        {
            var secret = new Secret(StringExtensions.Sha256(configuration.GetSection("IdentityServer")["ClientSecret"]));

            if (Environment.GetEnvironmentVariable("CLEAR_DOWN_IDS") == "true")
            {
                ids.Roles.Clear();
               // ids.Users.Clear();
                ids.RoleClaims.Clear();
                ids.UserClaims.Clear();
                ids.UserLogins.Clear();
                ids.UserRoles.Clear();
                ids.UserTokens.Clear();


                var applicationRoleAdmin = new Role
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                };
                ids.Roles.Add(applicationRoleAdmin);

                var applicationRoleEventOrganiser = new Role
                {
                    Name = "EventOrganiser",
                    NormalizedName = "EVENTORGANISER"
                };
                ids.Roles.Add(applicationRoleEventOrganiser);

                var operatorRole = new Role
                {
                    Name = "Operator",
                    NormalizedName = "OPERATOR"
                };
                ids.Roles.Add(operatorRole);

                var dashUserRole = new Role
                {
                    Name = "DashUser",
                    NormalizedName = "DASHUSER"
                };
                ids.Roles.Add(dashUserRole);

                ids.SaveChanges();
                context.Clients.Clear();
                context.IdentityResources.Clear();
                context.ApiResources.Clear();
                context.ApiScopes.Clear();
                context.SaveChanges();
            }



            foreach (var client in Config.Clients(configuration))
            {
                if (!context.Clients.Any(x => x.ClientId == client.ClientId))
                {
                    Console.WriteLine("Adding client " + client.ClientId + " " + client.ClientName);
                    context.Clients.Add(client.ToEntity());
                }
            }
            context.SaveChanges();

            foreach (var resource in Config.GetIdentityResources())
            {
                if (!context.IdentityResources.Any(x => x.Name == resource.Name))
                {
                    Console.WriteLine("Adding IdentityResources " + resource.Name);
                    context.IdentityResources.Add(resource.ToEntity());
                }
            }
            context.SaveChanges();

            foreach (var resource in Config.GetApiResources())
            {
                if (!context.ApiResources.Any(x => x.Name == resource.Name))
                {
                    Console.WriteLine("Adding ApiResources " + resource.Name);
                    context.ApiResources.Add(resource.ToEntity());
                }
            }
            context.SaveChanges();

            foreach (var resource in Config.GetApiScopes())
            {
                if (!context.ApiScopes.Any(x => x.Name == resource.Name))
                {
                    Console.WriteLine("Adding ApiScopes " + resource.Name);
                    context.ApiScopes.Add(resource.ToEntity());
                }
            }

            context.SaveChanges();
        }
    }
}