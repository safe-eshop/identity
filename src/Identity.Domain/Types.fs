namespace Identity.Domain.Types
open Identity.Domain.Model
open System.Threading.Tasks

type GetUserDto = { username: string; password: string }

[<CLIMutable>]
type LoginUserDto = { username: string; password: string }

type GetUser = GetUserDto -> Task<Result<User, DomainError>>
