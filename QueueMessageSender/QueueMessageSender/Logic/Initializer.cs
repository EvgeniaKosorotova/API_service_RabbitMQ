using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QueueMessageSender.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QueueMessageSender.Logic
{
    public class Initializer
    {
        readonly List<string> roles = new List<string> { "Admin", "User" };

        public Initializer(DataContext context, IRoleManager roleManager, IUserManager userManager)
        {
            //Console.Write("Initializer");
            context.Database.Migrate();

            if (context.Roles.Count() <= 0)
            {
                foreach (string name in roles)
                {
                    Task.Run(async () =>
                    {
                        var role = await roleManager.AddAsync(name);
                        await userManager.CreateAsync(name, "Password", role.Id);
                    });
                }
            }
        }
    }
}
