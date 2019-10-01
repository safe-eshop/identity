namespace Identity.Domain.Types
open Identity.Domain.Model
open System.Threading.Tasks

type UserDto = { username: string; password: string }

type PublishDomainEvent = DomainEvent -> Task<Result<DomainEvent, DomainError>>

type LoginUser =  UserDto -> Task<Result<UserToken, DomainError>>

type TokenGeneratorParams = { userId: string; username: string; roles: string[] }

type GetUser = UserDto -> Task<Result<User, DomainError>>

type GenerateToken = TokenGeneratorParams -> Task<Result<UserToken, DomainError>>
