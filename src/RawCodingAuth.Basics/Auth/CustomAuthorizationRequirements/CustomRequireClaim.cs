﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace RawCodingAuth.Basics.Auth.CustomAuthorizationRequirements
{
    public class CustomRequireClaimRequirement : IAuthorizationRequirement
    {
        public string Claim { get; }

        // Internal olan RequireClaim'ı taklit ettiğimizden dolayı ctor dan claim name alıyoruz
        public CustomRequireClaimRequirement(string claim)
        {
            Claim = claim;
        }
    }

    // Bu handler'ı enjeksiyon servisine eklemeyi unutma
    public class CustomRequireClaimRequirementHandler : AuthorizationHandler<CustomRequireClaimRequirement>
    {
        public CustomRequireClaimRequirementHandler()
        {

        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            CustomRequireClaimRequirement requirement)
        {
            if (context.User.HasClaim(t => t.Type == requirement.Claim))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
