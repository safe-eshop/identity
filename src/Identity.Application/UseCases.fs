namespace Identity.Application
open System.Threading.Tasks

module UseCases = 
    open Identity.Domain.Model
    open Identity.Domain.Types
    open Identity.Application
    open FSharp.Control.Tasks.V2

    let login(getUser: GetUser)(generateToken: GenerateToken)(publishDomainEvent: PublishDomainEvent)(user: UserDto) : Task<Result<UserToken, DomainError>> = 
        task {
            let! evt = getUser(user) 
                        |> TaskResult.bind (fun user -> generateToken { username = user.username; userId = user.id; roles = user.roles } |> TaskResult.bind(fun t -> Task.FromResult(Ok({| user = user; token = t |}))))
                        |> TaskResult.bind(fun res -> publishDomainEvent(UserAuthenticated(res)))
           
            return evt |> Result.map(fun evt -> match evt with | UserAuthenticated u -> u.token)
        }