namespace Identity.Domain
open System.Threading.Tasks

module UseCases = 
    open Identity.Domain.Model
    open FSharp.Control.Tasks.V2

    type UserToken = { token: string; refreshToken: string; expiry: int64 }
    
    type LoginUser =  {| username: string; password: string |} -> Task<Result<UserToken, DomainError>>

    type TokenGeneratorParams = { userId: string; username: string; roles: string[] }

    type GetUser = {| username: string; password: string |} -> Task<Result<User, DomainError>>

    type GenerateToken = TokenGeneratorParams -> Task<Result<UserToken, DomainError>>

    let login(getUser: GetUser)(generateToken: GenerateToken)(user: {| username: string; password: string |}) : Task<Result<UserToken, DomainError>> = 
        task {
            return! getUser(user) 
                        |> TaskResult.bind (fun user -> generateToken { username = user.username; userId = user.id; roles = user.roles } )
        }