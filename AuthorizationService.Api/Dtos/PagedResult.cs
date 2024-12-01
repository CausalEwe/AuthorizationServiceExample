namespace AuthorizationService.Api.Dtos;

public sealed record PagedResult<T>(long Total, IReadOnlyCollection<T> Items);