namespace Identity.Infrastructure.UnitTests
open System

module Token = 
    open Expecto
    open Identity.Infrastructure
    open Identity.Infrastructure.Token
    open FSharp.Control.Tasks

    [<Tests>]
    let tests = 
        testList "Token" [
            testList "generate" [
                testCase "Login when user not found" <| fun _ -> 
                    let userId = Guid.NewGuid().ToString()
                    let config = { issuer = "d"; audience = "d"; expiryHours = 1L; secretKey = "hjhdasghdasghjdsaghjdsaghjadsghjdsag"  }
                    let subject = Token.generateJwt config { userId = userId; username = "test"; roles = [||] }
                    Expect.isNotEmpty subject.token "token should be not empty"
                    Expect.isNotEmpty subject.refreshToken "refreshToken should be not empty"
                    Expect.isGreaterThan subject.expiry 0L "expiry should be grater than 0"
            ]
        ]