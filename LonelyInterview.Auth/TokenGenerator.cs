using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LonelyInterview.Auth
{
    public class TokenGenerator(IOptions<AuthOptions> authOptions/*, UserManager<User> userManager, PhotosContext dbContext*/)
    {
        //private const string SystemName = "LonelyInterview";
        public string GenerateToken(List<Claim> claims)
        {
            var jwt = new JwtSecurityToken(
                issuer: authOptions.Value.Issuer,
                audience: authOptions.Value.Audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromSeconds(20)),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.Value.Key)), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }



        //    public async Task<string> GenerateRefreshTokenAsync(User user)
        //    {

        //        var refreshToken = await userManager.GenerateUserTokenAsync(user, SystemName, "RefreshToken");

        //        var t = new UserToken()
        //        {
        //            UserId = user.Id,
        //            Value = refreshToken,
        //            ExpirationDate = DateTime.Now.AddDays(7),
        //            LoginProvider = SystemName,
        //            Name = "RefreshToken"
        //        };

        //        var users = await dbContext.UserTokens.ToListAsync();


        //        var token = dbContext.UserTokens.Where(t => t.UserId == user.Id).FirstOrDefault();
        //        if (token is not null)
        //        {
        //            token.Value = t.Value;
        //            token.UserId = user.Id;
        //            token.ExpirationDate = DateTime.Now.AddDays(7);
        //            token.Name = t.Name;

        //        }
        //        else
        //        {
        //            await dbContext.UserTokens.AddAsync(t);

        //        }
        //        await dbContext.SaveChangesAsync();

        //        //await userManager.SetAuthenticationTokenAsync(user, SystemName, "RefreshTokenExpirationDate", DateTime.Now.AddDays(7).ToString());
        //        return refreshToken;
        //    }
        //}

    }
}
