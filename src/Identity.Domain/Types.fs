namespace Identity.Domain.Types
open Identity.Domain.Model
open System.Threading.Tasks

type GetUserDto = { username: string; password: string }
type LoginUserDto = { username: string; password: string }

type LoginUser =  LoginUserDto -> Task<Result<UserToken, DomainError>>

type TokenGeneratorParams = { userId: string; username: string; roles: string[] }

type GetUser = GetUserDto -> Task<Result<User, DomainError>>

type GenerateToken = TokenGeneratorParams -> Task<Result<UserToken, DomainError>>
