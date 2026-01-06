using MediatR;

namespace Challenge.Commands.Products.UploadImage;

public record UploadProductImageCommandRequest(long ProductId, Stream ImageStream, string FileName, string ContentType) : IRequest<UploadProductImageCommandResponse>
{
}

