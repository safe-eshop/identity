namespace Identity.Application
open System.Threading.Tasks
open FSharp.Control.Tasks.V2
open Identity.Domain.Model

module ApplicationErrorMapper =
    open Identity.Application.Types
    let map(domainError: DomainError): ApplicationError =
            match domainError with
            | UserNotExists(username) | UserPasswordIncorrect(username)->
                UserNotFound(username)

module TaskResult =
    
    let bind (f : 'a -> Task<Result<'b, 'c>>) (a : Task<Result<'a, 'c>>)  : Task<Result<'b, 'c>> = task {
        match! a with
        | Ok value ->
            let next : Task<Result<'b, 'c>> = f value
            return! next
        | Error err -> return (Error err)
    }
    
    let mapError (ferr : 'b -> 'c) (a : Task<Result<'a, 'b>>)  : Task<Result<'a, 'c>> = task {
        match! a with
        | Ok value -> return Ok(value)
        | Error err -> 
          let newErr = ferr(err)
          return Error(newErr)
    }

    let compose (f : 'a -> Task<Result<'b, 'e>>) (g : 'b -> Task<Result<'c, 'e>>) : 'a -> Task<Result<'c, 'e>> =
        fun x -> bind g (f x)

    let valueOrDefault f result =
      async {
        match! result with
        | Ok ok -> return ok
        | Error err -> return f err
      }

    let (>>=) a f = bind f a
    let (>=>) f g = compose f g