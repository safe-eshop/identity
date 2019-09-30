namespace Identity.Domain
open System.Threading.Tasks

module UseCases = 
    open Identity.Domain.Model
    open FSharp.Control.Tasks.V2

    type PublishDomainEvent = DomainEvent -> Task<Result<DomainEvent, DomainError>>
    
    type LoginUser =  {| username: string; password: string |} -> Task<Result<UserToken, DomainError>>

    type TokenGeneratorParams = { userId: string; username: string; roles: string[] }

    type GetUser = {| username: string; password: string |} -> Task<Result<User, DomainError>>

    type GenerateToken = TokenGeneratorParams -> Task<Result<UserToken, DomainError>>

    let login(getUser: GetUser)(generateToken: GenerateToken)(publishDomainEvent: PublishDomainEvent)(user: {| username: string; password: string |}) : Task<Result<UserToken, DomainError>> = 
        task {
            let! evt = getUser(user) 
                        |> TaskResult.bind (fun user -> generateToken { username = user.username; userId = user.id; roles = user.roles } |> TaskResult.bind(fun t -> Task.FromResult(Ok({| user = user; token = t |}))))
                        |> TaskResult.bind(fun res -> publishDomainEvent(UserAuthenticated(res)))
           
            return evt |> Result.map(fun evt -> match evt with | UserAuthenticated u -> u.token)
        }