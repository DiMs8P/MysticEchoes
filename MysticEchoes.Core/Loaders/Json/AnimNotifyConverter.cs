using MysticEchoes.Core.Animations;
using Newtonsoft.Json;

namespace MysticEchoes.Core.Loaders.Json;

public class AnimNotifyConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(AnimNotify);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        string typeName = (string)reader.Value;
        Type type = Type.GetType(typeName);
        if (type != null)
        {
            return Activator.CreateInstance(type);
        }

        throw new JsonSerializationException("Type not found: " + typeName);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}