namespace Identity.Application.Types
open Identity.Domain.Model
open System.Threading.Tasks

type IntegrationEvent = 
    | UserAuthenticated of user: User * token: UserToken

type ApplicationError = 
    | UserNotFound of username: string

type MapDomainToApplicationError = DomainError -> ApplicationError

type PublishIntegrationEvent = IntegrationEvent -> Task<Result<IntegrationEvent, ApplicationError>>