namespace Micro.Core.Common.Requests;

public abstract record PagedRequest(int Skip, int Take);