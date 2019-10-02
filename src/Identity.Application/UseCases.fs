namespace Identity.Application
open System.Threading.Tasks

module UseCases = 
    open Identity.Domain.Model
    open Identity.Domain.Types
    open Identity.Application
    open Identity.Application.Types
    open FSharp.Control.Tasks.V2


    let login(getUser: GetUser)(generateToken: GenerateToken)(publishIntegrationEvent: PublishIntegrationEvent)(mapError: MapDomainToApplicationError)(user: UserDto) : Task<Result<UserToken, ApplicationError>> = 
        task {
            let! evt = getUser(user) 
                        |> TaskResult.bind (fun user -> generateToken { username = user.username; userId = user.id; roles = user.roles } |> TaskResult.bind(fun t -> Task.FromResult(Ok({| user = user; token = t |}))))
                        |> TaskResult.mapError(mapError)
                        |> TaskResult.bind(fun res -> publishIntegrationEvent(UserAuthenticated(res.user, res.token)))
           
            return evt |> Result.map(fun evt -> match evt with | UserAuthenticated(_, token) -> token)
        }