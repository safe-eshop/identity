namespace Identity.Api
open Microsoft.Extensions.DependencyInjection
open Identity.Infrastructure.Token
open Microsoft.Extensions.Configuration
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Hosting
open System
open Microsoft.AspNetCore.Http

module App = 
    open Giraffe

    let webApp =
        choose [
            subRoute "/api"
                (choose [
                    subRoute "/token" (
                        choose [
                            POST >=> Identity.Api.User.Controller.getTokenHandler
                        ]
                    )
                ])
            GET >=> choose [
                route "/ping" >=> text "pong"
            ]
            setStatusCode 404 >=> text "Not Found" ]

    let envGetOrElse key elseVal =
        match System.Environment.GetEnvironmentVariable(key) with
        | null -> elseVal
        | res -> res

    let pathBase = envGetOrElse "PATH_BASE" String.Empty

    let errorHandler (ex : Exception) (logger : ILogger) =
        logger.LogError(EventId(), ex, "An unhandled exception has occurred while executing the request.")
        clearResponse >=> setStatusCode 500 >=> text ex.Message  

    type Startup(configuration: IConfiguration) =
        member __.ConfigureServices (services : IServiceCollection) =
            services
                .AddResponseCaching()
                .AddGiraffe()
                .Configure<JwtConfig>(configuration.GetSection("jwt")) |> ignore
            services |> IoC.addInfrastructure |> ignore

        member __.Configure (app : IApplicationBuilder) =
            if pathBase |> String.IsNullOrEmpty |> not then
                app.UsePathBase(PathString(pathBase))  |> ignore
                          // Add Giraffe to the ASP.NET Core pipeline
            app.UseGiraffeErrorHandler(errorHandler)
               .UseStaticFiles()
               .UseResponseCaching()
               .UseGiraffe webApp

    [<EntryPoint>]
    let main args =
        let builder = 
            Host.CreateDefaultBuilder(args)
              .ConfigureWebHostDefaults
                  (fun webBuilder ->               
                    webBuilder.UseStartup<Startup>() |> ignore
                  );
        builder.Build().Run();
        0