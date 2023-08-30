﻿using EvolvedTax.Data.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace EvolvedTax.Web.Middlewares
{

    public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<User, IdentityRole>
    {
        public CustomClaimsPrincipalFactory(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            user = await UserManager.Users.FirstAsync(p=>p.UserName == user.UserName);
            var roles = await UserManager.GetRolesAsync(user);
            // Add custom claims here
            identity.AddClaim(new Claim("FirstName", user.FirstName ?? string.Empty));
            identity.AddClaim(new Claim("LastName", user.LastName ?? string.Empty));
            identity.AddClaim(new Claim("UserId", user.Id ?? string.Empty));
            identity.AddClaim(new Claim("UserName", user.UserName ?? string.Empty));
            identity.AddClaim(new Claim("UserRole", roles.First() ?? string.Empty));
            identity.AddClaim(new Claim("InstituteId", user.InstituteId.ToString() ?? string.Empty));
            return identity;
        }
    }
}
