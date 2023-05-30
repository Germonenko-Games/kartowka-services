using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Kartowka.Authorization.Core.Contracts;
using Kartowka.Authorization.Core.Models;
using Kartowka.Authorization.Infrastructure.Options;
using Kartowka.Core.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Kartowka.Authorization.Infrastructure.Contracts;

public class JwtAccessTokenGenerator : IAccessTokenGenerator
{
    private readonly JwtOptions _options;

    public JwtAccessTokenGenerator(IOptionsSnapshot<JwtOptions> options)
    {
        _options = options.Value;
    }

    public TokenInfo GenerateToken(User user)
    {
        var issueDate = DateTime.UtcNow;
        var expireDate = issueDate.Add(_options.TokenLifespan);

        var tokenParameters = GetTokenValidationParameters();

        var userIdClaim = new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString());

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            IssuedAt = issueDate,
            Expires = expireDate,
            Subject = new (new []{userIdClaim}),
            SigningCredentials = new SigningCredentials(
                tokenParameters.IssuerSigningKey,
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        if (tokenParameters.ValidateIssuer)
        {
            tokenDescriptor.Issuer = tokenParameters.ValidIssuer;
        }

        if (tokenParameters.ValidateAudience)
        {
            tokenDescriptor.Issuer = tokenParameters.ValidAudience;
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenStringRepresentation = tokenHandler.CreateToken(tokenDescriptor);
        return new TokenInfo
        {
            IssueDate = issueDate,
            ExpireDate = expireDate,
            AccessToken = tokenHandler.WriteToken(tokenStringRepresentation),
        };
    }
    
    private TokenValidationParameters GetTokenValidationParameters()
    {
        var jwtSecretBytes = Encoding.UTF8.GetBytes(_options.Secret);
        var tokenParameters = new TokenValidationParameters
        {
            ValidateLifetime = true,
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(jwtSecretBytes)
        };

        return tokenParameters;
    }
}