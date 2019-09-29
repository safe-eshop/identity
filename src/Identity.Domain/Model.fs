namespace Identity.Domain.Model

type Role = string

type Roles = Role array

type User = { id: string; username: string; roles: Roles }

type DomainError = 
    | UserNotExists of username: string