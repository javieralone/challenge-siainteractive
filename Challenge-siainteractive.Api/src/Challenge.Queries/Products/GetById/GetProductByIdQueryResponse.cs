namespace Challenge.Queries.Products.GetById;

public record GetProductByIdQueryResponse(long Id, string Name, string Description, string? Image);

