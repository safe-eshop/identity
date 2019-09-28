namespace Identity.Api.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open Identity.Api
open Identity.Domain

[<ApiController>]
[<Route("[controller]")>]
type TokenController (logger : ILogger<TokenController>) =
    inherit ControllerBase()
    
    [<HttpGet("{name}")>]
    member __.Get(name) : String = Say.hello name
