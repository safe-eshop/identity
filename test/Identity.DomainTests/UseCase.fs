namespace Identity.Domain.UnitTests
open System

module UseCase = 
    open Expecto
    open Identity.Domain.UseCases
    open Identity.Domain.Model
    open FSharp.Control.Tasks

    [<Tests>]
    let tests = 
        testList "UseCases" [
            testList "Login" [
                testCaseAsync "Login when user not found" <| async {
                    let getUser: GetUser = (fun u -> task { return Error(UserNotExists(u.username)) })
                    let generateToken: GenerateToken = (fun u -> task { return Ok({ token = ""; refreshToken = ""; expiry = 2L }) })
                    let! subject = login getUser generateToken {| username = "test"; password = "test" |} |> Async.AwaitTask
                    Expect.equal subject (Error(UserNotExists("test"))) """result should beError(UserNotExists("test"))"""
                }

                testCaseAsync "Login when user is found" <| async {
                    let user = { id = Guid.NewGuid().ToString(); username = "test"; roles = [||] }
                    let token = { token = "adasds"; refreshToken = "adsdsadsa"; expiry = 2L }
                    let getUser: GetUser = (fun u -> task { return Ok(user) })
                    let generateToken: GenerateToken = (fun u -> task { return Ok(token) })
                    let! subject = login getUser generateToken {| username = "test"; password = "test" |} |> Async.AwaitTask
                    Expect.equal subject (Ok(token)) """result should beError(UserNotExists("test"))"""
                } 
            ]
        ]