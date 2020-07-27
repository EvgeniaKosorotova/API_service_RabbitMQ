using Microsoft.EntityFrameworkCore;
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
        private readonly DataContext _context;
        private readonly IRoleManager _roleManager;
        private readonly IUserManager _userManager;

        public Initializer(DataContext context, IRoleManager roleManager, IUserManager userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public void Initialize()
        {
            Console.Write("Initializer");
            _context.Database.Migrate();

            if (_context.Roles.Count() <= 0)
            {
                foreach (string name in roles)
                {
                    Task.Run(async () =>
                    {
                        var role = await _roleManager.AddAsync(name);
                        await _userManager.CreateAsync(name, "Password", role.Id);
                    });
                }
            }
        }
    }
}
