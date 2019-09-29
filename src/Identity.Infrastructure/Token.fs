namespace Identity.Infrastructure
open Identity.Domain.UseCases
open System.Threading.Tasks
open System.Security.Claims
open System.IdentityModel.Tokens.Jwt

module Token = 
    open Identity.Domain
    open Identity.Domain.Model
    open System

    type JwtConfig = { expiryHours: int64; secretKey: string; issuer: string; audience: string }
   
    let generateJwt(config: JwtConfig)(user: TokenGeneratorParams) : UserToken = 
        let now = DateTime.UtcNow
        let expiry = now.AddHours(config.expiryHours |> float)
        let claims = [| Claim(JwtRegisteredClaimNames.Sub, user.userId); Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) |]
        failwith "daasd"