﻿using leave_system.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_system
{
    public static class SeedData
    {
        public static void Seed(UserManager<Employee> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }

        private static void SeedUsers(UserManager<Employee> userManager)
        {
            if(userManager.FindByNameAsync("admin").Result == null)
            {
                var user = new Employee
                {
                    UserName = "admin@localhost.com",
                    Email = "admin@localhost.com"
                };

                var result = userManager.CreateAsync(user, "P@ssword1").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Administrator").Wait();
                }
            }

            if (userManager.FindByNameAsync("regularemployee").Result == null)
            {
                var user = new Employee
                {
                    UserName = "employee@localhost.com",
                    Email = "employee@localhost.com"
                };
                var user1 = new Employee
                {
                    UserName = "employee@test.com",
                    Email = "employee@test.com"
                };

                var result = userManager.CreateAsync(user, "P@ssword1").Result;
                var result2 = userManager.CreateAsync(user1, "P@ssword1").Result;

                if (result.Succeeded && result2.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Employee").Wait();
                    userManager.AddToRoleAsync(user1, "Employee").Wait();
                }
            }
        }

        private static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("Administrator").Result)
            {
                var role = new IdentityRole
                {
                    Name = "Administrator"
                };
                roleManager.CreateAsync(role);
            }

            if (!roleManager.RoleExistsAsync("Employee").Result)
            {
                var role = new IdentityRole
                {
                    Name = "Employee"
                };
                roleManager.CreateAsync(role);
            }
        }
    }
}
