namespace ThreadService.Model;

public record struct Paginated
{
    public int Page { get; init; } = default!;
    public int Limit { get; init; } = default!;
}