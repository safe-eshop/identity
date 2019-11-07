namespace Identity.Infrastructure

module Repository = 
    open Identity.Domain.Types
    open Identity.Application.Types
    open FSharp.Control.Tasks.V2
    open System
    open Identity.Domain.Model

    let private USER_ID = "35f55aa6-2f62-498d-87d1-1050e0392956"
    //GetUserDto -> Task<Result<User, DomainError>>
    let fakeGetUser: GetUser =
        fun userDto ->
            task {
                if userDto.username = "admin" && userDto.password = "admin" then
                    return Ok({ id = USER_ID; username = userDto.username; roles = [||] })
                else 
                    return Error(UserNotExists(userDto.username))
            }