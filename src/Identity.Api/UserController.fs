namespace Identity.Api.User
open FSharp.Control.Tasks.V2.ContextInsensitive
open Identity.Infrastructure.Token
open Identity.Domain.Types
open Identity.Application.UseCases
open Microsoft.Extensions.Logging
open Identity.Application.Types
open Microsoft.Extensions.Options
open Identity.Infrastructure
open Identity.Api.Messages

module Controller = 
    open Microsoft.AspNetCore.Http
    open Giraffe
    
    let getTokenHandler: HttpHandler =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
               
                let! loginDto = ctx.BindJsonAsync<LoginUserDto>()              
                let login = ctx.GetService<LoginUser>()
                match! login(loginDto) with
                | Ok(token) -> 
                     return! json token next ctx
                | Error(err) ->
                    return! identityerror err next ctx
            }