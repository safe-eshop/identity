namespace Identity.Api
open Giraffe
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Diagnostics.HealthChecks
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open HealthChecks.UI.Client
open System
open Microsoft.Extensions.Diagnostics.HealthChecks
open System.Threading.Tasks

module Messages = 
    open  Identity.Application.Types
    type ApiError = { errorMessage: string }

    let identityerror(appMsg: ApplicationError) = 
        match appMsg with
        | UserNotFound(username) -> 
            RequestErrors.BAD_REQUEST { errorMessage = (sprintf "User: %s not exists" username) }