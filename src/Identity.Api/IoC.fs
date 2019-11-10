namespace Identity.Api
open Microsoft.Extensions.DependencyInjection
open FSharp.Control.Tasks.V2.ContextInsensitive
open Identity.Infrastructure.Token
open Identity.Application.UseCases
open Identity.Application
open Microsoft.Extensions.Logging
open Identity.Application.Types
open Microsoft.Extensions.Options
open Identity.Infrastructure
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Diagnostics.HealthChecks
open Microsoft.AspNetCore.Diagnostics.HealthChecks
open HealthChecks.UI.Client

module IoC = 
    let private addTransient(f: System.IServiceProvider -> 'TService)(services: IServiceCollection) = 
        services.AddTransient<'TService>(System.Func<System.IServiceProvider, 'TService>(f))

    let private addLogin(provider: System.IServiceProvider): LoginUser =
        let logger = provider.GetService<ILogger<unit>>()
        let config = provider.GetService<IOptions<JwtConfig>>()
        let tokenGenerator cfg : GenerateToken = fun par -> task { return Ok(Token.generateJwt(cfg) par)}
        let myLogger (logger: ILogger<unit>) = fun msg -> logger.LogInformation(msg)
        UseCases.login Repository.fakeGetUser (tokenGenerator(config.Value)) (Messaging.fakePublisher(myLogger(logger)))
    
    let addInfrastructure (services: IServiceCollection) = 
        services |> addTransient(addLogin)

module HealthCheck = 
    open System
    open System.Threading.Tasks

    let private writePongResponse: Func<HttpContext, HealthReport, Task> =
        let f (httpContext: HttpContext)(_: HealthReport) =
            httpContext.Response.ContentType <- "application/json"
            httpContext.Response.WriteAsync("pong")
        Func<HttpContext, HealthReport, Task>(f)

    let private selfPredicate: Func<HealthCheckRegistration, bool> =
        let f (req: HealthCheckRegistration) = req.Name.Contains("self")
        Func<HealthCheckRegistration, bool>(f)

    let private alwaysTruePredicate: Func<HealthCheckRegistration, bool> =
        let f (_: HealthCheckRegistration) = true
        Func<HealthCheckRegistration, bool>(f)

    let private writeHealthCheckUIResponse: Func<HttpContext, HealthReport, Task> =
        let f (httpContext: HttpContext)(report: HealthReport) =
            UIResponseWriter.WriteHealthCheckUIResponse(httpContext, report)
        Func<HttpContext, HealthReport, Task>(f)

    let asPathString(str: String): PathString =
        str |> PathString.op_Implicit

    let addHealthCheck(services: IServiceCollection) =
        services.AddHealthChecks()

    let useHealthCheck(app: IApplicationBuilder) =
        let pingOpt = HealthCheckOptions(Predicate = selfPredicate, ResponseWriter = writePongResponse)
        let healthOpt = HealthCheckOptions(Predicate = alwaysTruePredicate, ResponseWriter = writeHealthCheckUIResponse)
        app.UseHealthChecks("/ping" |> asPathString, pingOpt) |> ignore
        app.UseHealthChecks("/health" |> asPathString, healthOpt) |> ignore
        app
