namespace Identity.Api.User
open FSharp.Control.Tasks.V2.ContextInsensitive
open Identity.Infrastructure.Token

module Controller = 
    open Saturn.ControllerHelpers
    open Microsoft.AspNetCore.Http
    open Giraffe
    open Microsoft.AspNetCore.Identity
    open Identity.Application.Types
    open Microsoft.Extensions.Options
    open Newtonsoft.Json
    open Saturn

    let handleGetToken =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! loginDto = Controller.getModel<LoginUserDto> ctx
                let config = ctx.GetService<IOptions<JwtConfig>>()
                let! tokenResult = UsersService.loginAsync usersRepository.GetUser (Crypto.jwt(config.Value)) loginDto
                match tokenResult with
                | Ok(token) -> 
                     return! Successful.OK token next ctx
                | Error(err) ->
                    return! RequestErrors.BAD_REQUEST err next ctx
            }
    