namespace Identity.Infrastructure


module Messaging = 
    open Identity.Application.Types
    open FSharp.Control.Tasks.V2

    let fakePublisher(log: string -> unit): PublishIntegrationEvent =
            fun evt -> 
                task {
                    match evt with 
                    | UserAuthenticated(user, token) -> 
                        log("User authenticated")
                    return Ok(evt)
                }