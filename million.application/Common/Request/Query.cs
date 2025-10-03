using MediatR;

namespace million.application.Common.Request;

public record Query<T> : IRequest<T>
{
    public int Offset { get; set; } = 0;
    
    public int Limit { get; set; } = 10;
}