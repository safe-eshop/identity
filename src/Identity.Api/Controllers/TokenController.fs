namespace Identity.Api.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open Identity.Api
open Identity.Domain.Model
open FSharp.Control.Tasks.V2
open Identity.Domain.UseCases

[<CLIMutable>]
type LoginRequest = { username: string; password: string }

[<ApiController>]
[<Route("[controller]")>]
type TokenController (logger : ILogger<TokenController>) =
    inherit ControllerBase()

    [<HttpPost("")>]
    member __.Get([<FromBody>]user: LoginRequest) :Task<IActionResult> =
        task {
            match! login {| username = user.username; password = user.password |} with
            | Ok(token) ->
                return base.Ok(token) :> IActionResult
            | Error(err) -> 
                match err with
                | UserNotExists username -> 
                    return __.BadRequest(sprintf "User %s not exists" username) :> IActionResult
                | _ -> 
                    return __.StatusCode(500) :> IActionResult
        }
