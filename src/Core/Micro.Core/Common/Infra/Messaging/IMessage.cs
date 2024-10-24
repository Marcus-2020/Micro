namespace Micro.Core.Common.Infra.Messaging;

public interface IMessage
{
    public string Type { get; }
    public string Message { get; }
}