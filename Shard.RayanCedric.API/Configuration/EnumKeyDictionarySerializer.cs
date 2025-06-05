using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.IO;

namespace Shard.RayanCedric.API.Configuration;

public class EnumKeyDictionarySerializer<TKey, TValue> : SerializerBase<Dictionary<TKey, TValue>>
    where TKey : struct, Enum
{
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Dictionary<TKey, TValue> value)
    {
        context.Writer.WriteStartDocument();

        foreach (var item in value)
        {
            context.Writer.WriteName(item.Key.ToString());
            BsonSerializer.Serialize(context.Writer, item.Value);
        }

        context.Writer.WriteEndDocument();
    }

    public override Dictionary<TKey, TValue> Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var dictionary = new Dictionary<TKey, TValue>();

        context.Reader.ReadStartDocument();
        while (context.Reader.State != BsonReaderState.EndOfDocument)
        {
            var keyString = context.Reader.ReadName();
            
            if (!Enum.TryParse<TKey>(keyString, out var key))
                throw new BsonSerializationException($"Cannot convert '{keyString}' to {typeof(TKey)}.");

            var value = BsonSerializer.Deserialize<TValue>(context.Reader);
            dictionary[key] = value;
        }
        context.Reader.ReadEndDocument();

        return dictionary;
    }
}