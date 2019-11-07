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

module IoC = 
    let private addTransient(f: System.IServiceProvider -> 'TService)(services: IServiceCollection) = 
        services.AddTransient<'TService>(System.Func<System.IServiceProvider, 'TService>(f))

    let private addLogin(provider: System.IServiceProvider): LoginUser =
        let logger = provider.GetService<ILogger<unit>>()
        let config = provider.GetService<IOptions<JwtConfig>>()
        let tokenGenerator cfg : GenerateToken = fun par -> task { return Ok(Token.generateJwt(cfg) par)}
        let myLogger (logger: ILogger<unit>) = fun msg -> logger.LogInformation(msg)
        UseCases.login Repository.fakeGetUser (tokenGenerator(config.Value)) (Messaging.fakePublisher(myLogger(logger)))(ApplicationErrorMapper.map)
    
    let addInfrastructure (services: IServiceCollection) = 
        services |> addTransient(addLogin)