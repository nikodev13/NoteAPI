using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace NoteAPI.Authorization;

public static class Extensions
{
   public static IServiceCollection ConfigureAuthorization(this IServiceCollection services)
   {
      services.AddAuthorization(options =>
      {
         options.DefaultPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireClaim(JwtRegisteredClaimNames.Sub)
            .Build();
         
      });
      
      return services;
   }
}