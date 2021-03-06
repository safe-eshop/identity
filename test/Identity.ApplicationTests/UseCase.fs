namespace Identity.Application.UnitTests
open Identity.Application.Types
open System

module UseCase = 
    open Expecto
    open Identity.Application.UseCases
    open Identity.Application.Types
    open Identity.Application
    open Identity.Domain.Types
    open Identity.Domain.Model
    open FSharp.Control.Tasks

    [<Tests>]
    let tests = 
        testList "UseCases" [
            testList "Login" [
                testCaseAsync "Login when user not found" <| async {
                    let fakePublisher: PublishIntegrationEvent = fun de -> task { return Ok(de) }
                    let getUser: GetUser = (fun u -> task { return Error(UserNotExists(u.username)) })
                    let generateToken: GenerateToken = (fun u -> task { return Ok({ token = ""; refreshToken = ""; expiry = 2L }) })
                    let! subject = login getUser generateToken fakePublisher { username = "test"; password = "test" } |> Async.AwaitTask
                    Expect.equal subject (Error(UserNotFound("test"))) """result should beError(UserNotExists("test"))"""
                }

                testCaseAsync "Login when user is found" <| async {
                    let user = { id = Guid.NewGuid().ToString(); username = "test"; roles = [||] }
                    let token = { token = "adasds"; refreshToken = "adsdsadsa"; expiry = 2L }
                    let fakePublisher: PublishIntegrationEvent = fun de -> task { return Ok(de) }
                    let getUser: GetUser = (fun u -> task { return Ok(user) })
                    let generateToken: GenerateToken = (fun u -> task { return Ok(token) })
                    let! subject = login getUser generateToken fakePublisher { username = "test"; password = "test" } |> Async.AwaitTask
                    Expect.equal subject (Ok(token)) """result should beError(UserNotExists("test"))"""
                } 
            ]
        ]