namespace MysticEchoes.Core.AI.BehaviorTree;

public class Blackboard
{
    private Dictionary<string, int> _intValues = new Dictionary<string, int>();
    private Dictionary<string, string> _stringValues = new Dictionary<string, string>();
    private Dictionary<string, object> _objectValues = new Dictionary<string, object>();
    private Dictionary<string, Enum> _enumValues = new Dictionary<string, Enum>();
    private Dictionary<string, bool> _boolValues = new Dictionary<string, bool>();
    private Dictionary<string, float> _floatValues = new Dictionary<string, float>();
    
    public void SetValueAsInt(string key, int value)
    {
        _intValues[key] = value;
    }
    
    public void SetValueAsString(string key, string value)
    {
        _stringValues[key] = value;
    }
    
    public void SetValueAsObject(string key, object value)
    {
        _objectValues[key] = value;
    }
    
    public void SetValueAsEnum(string key, Enum value)
    {
        _enumValues[key] = value;
    }
    
    public void SetValueAsBool(string key, bool value)
    {
        _boolValues[key] = value;
    }
    
    public void SetValueAsFloat(string key, float value)
    {
        _floatValues[key] = value;
    }
    
    private T GetValue<T>(string key, Dictionary<string, T> values)
    {
        if (!values.TryGetValue(key, out T value))
        {
            throw new KeyNotFoundException($"Can't get value by key {key}");
        }
        return value;
    }

    public int GetValueAsInt(string key)
    {
        return GetValue<int>(key, _intValues);
    }

    public string GetValueAsString(string key)
    {
        return GetValue<string>(key, _stringValues);
    }
    
    public object GetValueAsObject(string key)
    {
        return GetValue<object>(key, _objectValues);
    }

    public Enum GetValueAsEnum(string key)
    {
        return GetValue<Enum>(key, _enumValues);
    }
    
    public bool GetValueAsBool(string key)
    {
        return GetValue<bool>(key, _boolValues);
    }
    
    public float GetValueAsFloat(string key)
    {
        return GetValue<float>(key, _floatValues);
    }
}