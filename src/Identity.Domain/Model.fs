namespace Identity.Domain.Model

type Role = string

type Roles = Role array

type User = { id: string; username: string; roles: Roles }

type UserToken = { token: string; refreshToken: string; expiry: int64 }

type DomainError = 
    | UserNotExists of username: string

type DomainEvent = 
    | UserAuthenticated of user: User * token: UserToken