namespace Micro.Core.Common.Responses;

public abstract record PagedResponse
{
    public int Count { get; private set; }
    public int Skip { get; private set; }
    public int Take { get; private set; }

    protected PagedResponse(int count, int skip, int take)
    {
        Count = count;
        Skip = skip;
        Take = take;
    }
}