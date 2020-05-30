using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QueueMessageSender.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QueueMessageSender.Logic
{
    public class Initializer
    {
        public static async Task InitializeAsync(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<DataContext>())
                {
                    context.Database.Migrate();

                    if (context.Roles.Count() <= 0)
                    {
                        var roleManager = serviceScope.ServiceProvider.GetService<IRoleManager>();
                        var userManager = serviceScope.ServiceProvider.GetService<IUserManager>();

                        foreach (string name in Enum.GetNames(typeof(RolesDefault)))
                        {
                            await roleManager.AddAsync(name);
                            await userManager.CreateAsync(name, "Password", name);
                        }
                    }
                }
            }
        }
    }
}
