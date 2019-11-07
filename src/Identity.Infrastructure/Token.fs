namespace Identity.Infrastructure
open System.Threading.Tasks
open System.Security.Claims
open System.IdentityModel.Tokens.Jwt
open System.Security.Cryptography
open Microsoft.IdentityModel.Tokens
open System.Text
open Identity.Domain.Types
open Identity.Application.Types
open System.Diagnostics

module Token = 
    open Identity.Domain
    open Identity.Domain.Model
    open System

    [<CLIMutable>]
    type JwtConfig = { expiryHours: int64; secretKey: string; issuer: string; audience: string }

    let private generateRefreshToken(size: int option) =
        let finalSize = defaultArg size 32
        let numbers = Array.zeroCreate(finalSize) 
        use rng = RandomNumberGenerator.Create()
        rng.GetBytes(numbers)
        Convert.ToBase64String(numbers)

    let toEpoch(dateTime: DateTime) =
        dateTime.Subtract(DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds |> int64

    let generateJwt(config: JwtConfig)(user: TokenGeneratorParams) : UserToken = 
        let now = DateTime.UtcNow
        let refresh = generateRefreshToken(Some(32))
        let expiry = now.AddHours(config.expiryHours |> float)
        let roles = user.roles |> Array.map(fun r -> Claim(ClaimTypes.Role, r))
        let claims = Array.concat [ roles; ([| Claim(JwtRegisteredClaimNames.Sub, user.userId); Claim(JwtRegisteredClaimNames.Jti, refresh); |])]
        let key = SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.secretKey))
        let credentials = SigningCredentials(key, SecurityAlgorithms.HmacSha512)
        let token = JwtSecurityToken(issuer = config.issuer, audience = config.audience, expires = Nullable(expiry), claims = claims, signingCredentials = credentials)
        let jwt = JwtSecurityTokenHandler().WriteToken(token)
        { token = jwt; refreshToken = refresh; expiry = toEpoch(expiry) }