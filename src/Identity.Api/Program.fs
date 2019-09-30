namespace Identity.Api


module App = 
    open Saturn
    open Giraffe.Core
    open Giraffe.ResponseWriters
    open FSharp.Control.Tasks.V2.ContextInsensitive
    open System
    
   
    
    let otherRouter = router {
        get "/dsa" (text "")
        getf "/dsa/%s" (text)
        not_found_handler (setStatusCode 404 >=> text "Not Found")
    }
    
    let topRouter = router {
        forwardf "/%s/%s/abc" (fun (_ : string * string) -> otherRouter)
    }
    
    let app = application {
        use_router topRouter
        url "http://0.0.0.0:8085/"
    }
    
    [<EntryPoint>]
    let main _ =
        run app
        0