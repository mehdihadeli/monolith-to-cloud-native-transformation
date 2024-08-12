using System.Text;
using BuildingBlocks.Abstractions.Events;
using BuildingBlocks.Abstractions.Messaging;
using BuildingBlocks.Abstractions.Serialization;
using BuildingBlocks.Core.Events;
using BuildingBlocks.Core.Types;
using Newtonsoft.Json;

namespace BuildingBlocks.Core.Serialization;

public class DefaultMessageSerializer : DefaultSerializer, IMessageSerializer
{
    public new string ContentType => "application/json";

    public string Serialize(IEventEnvelope eventEnvelope)
    {
        return JsonConvert.SerializeObject(eventEnvelope, new JsonSerializerSettings());
    }

    public string Serialize<TMessage>(TMessage message)
        where TMessage : IMessage
    {
        return JsonConvert.SerializeObject(message, CreateSerializerSettings());
    }

    public TMessage? Deserialize<TMessage>(string message)
        where TMessage : IMessage
    {
        return JsonConvert.DeserializeObject<TMessage>(message, CreateSerializerSettings());
    }

    public object? Deserialize(string payload, string payloadType)
    {
        var type = TypeMapper.GetType(payloadType);
        var deserializedData = JsonConvert.DeserializeObject(payload, type, CreateSerializerSettings());

        return deserializedData;
    }

    public IEventEnvelope? Deserialize(string json)
    {
        return JsonConvert.DeserializeObject<EventEnvelope<object>>(json, CreateSerializerSettings());
    }

    public IMessage? Deserialize(ReadOnlySpan<byte> data, string payloadType)
    {
        var type = TypeMapper.GetType(payloadType);

        var json = Encoding.UTF8.GetString(data);
        var deserializedData = JsonConvert.DeserializeObject(json, type, CreateSerializerSettings());

        return deserializedData as IMessage;
    }
}
