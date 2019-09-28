namespace Identity.Api.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open Identity.Api
open Identity.Domain
open FSharp.Control.Tasks.V2

[<CLIMutable>]
type LoginRequest = { username: string; password: string }

[<ApiController>]
[<Route("[controller]")>]
type TokenController (logger : ILogger<TokenController>) =
    inherit ControllerBase()

    [<HttpPost("")>]
    member __.Get([<FromBody>]user: LoginRequest) :Task<String> = 
        task {
            let! login = Identity.Domain.UseCases.login {| username = user.username; password = user.password |}
            return "test"
        }
