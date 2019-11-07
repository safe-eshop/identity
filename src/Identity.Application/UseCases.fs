namespace Identity.Application
open Identity.Domain.Types
open System.Threading.Tasks
open Identity.Application
module UseCases = 
    open Identity.Domain.Model
    open Identity.Application.Types
    open FSharp.Control.Tasks.V2

    type LoginUser = LoginUserDto -> Task<Result<UserToken, ApplicationError>>

    let login(getUser: GetUser)(generateToken: GenerateToken)(publishIntegrationEvent: PublishIntegrationEvent)(user: LoginUserDto) : Task<Result<UserToken, ApplicationError>> = 
        task {
            let! evt = getUser({ username = user.username; password = user.password}) 
                        |> TaskResult.mapError(ApplicationErrorMapper.map)
                        |> TaskResult.bind (fun user -> generateToken { username = user.username; userId = user.id; roles = user.roles } |> TaskResult.bind(fun t -> Task.FromResult(Ok({| user = user; token = t |}))))
                        |> TaskResult.bind(fun res -> publishIntegrationEvent(UserAuthenticated(res.user, res.token)))
           
            return evt |> Result.map(fun evt -> match evt with | UserAuthenticated(_, token) -> token)
        }
