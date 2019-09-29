namespace Identity.Domain
open System.Threading.Tasks

module UseCases = 
    open Identity.Domain.Model
    open FSharp.Control.Tasks.V2

    type UserToken = { token: string; refreshToken: string; expiry: int64 }
    
    type LoginUser =  {| username: string; password: string |} -> Task<Result<UserToken, DomainError>>

    let login(user: {| username: string; password: string |}) : Task<Result<UserToken, DomainError>> = 
        task {
           return Error(UserNotExists(user.username)) 
        }