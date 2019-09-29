namespace Identity.Infrastructure
open Identity.Domain.UseCases
open System.Threading.Tasks

module Token = 
    open Identity.Domain
    open Identity.Domain.Model

    type JwtConfig = { expiryHours: int64; secretKey: string; issuer: string; audience: string }
   
    let generateJwt(config: JwtConfig)(user: TokenGeneratorParams) : UserToken = 
        failwith "chgadsjhgdsah"