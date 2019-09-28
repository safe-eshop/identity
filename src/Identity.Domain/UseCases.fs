namespace Identity.Domain

module UseCases = 
    open Identity.Domain.Model
    open FSharp.Control.Tasks.V2

    let login(user: {| username: string; password: string |}) = 
        task {
           return Error(UserNotExists(user.username)) 
        }