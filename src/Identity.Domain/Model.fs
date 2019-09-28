namespace Identity.Domain.Model

type User = { id: string; username: string }

type DomainError = 
    | UserNotExists of username: string