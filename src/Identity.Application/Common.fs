namespace Identity.Application
open System.Threading.Tasks
open FSharp.Control.Tasks.V2

module TaskResult =
    
    let bind (f : 'a -> Task<Result<'b, 'c>>) (a : Task<Result<'a, 'c>>)  : Task<Result<'b, 'c>> = task {
        match! a with
        | Ok value ->
            let next : Task<Result<'b, 'c>> = f value
            return! next
        | Error err -> return (Error err)
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