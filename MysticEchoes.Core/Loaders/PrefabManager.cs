using System.Globalization;
using System.Numerics;
using System.Reflection;
using MysticEchoes.Core.Loaders.Prefabs;
using MysticEchoes.Core.Scene;

namespace MysticEchoes.Core.Loaders;

public class PrefabManager
{
    private readonly IDataLoader _dataLoader;

    private readonly Dictionary<PrefabType, List<KeyValuePair<Type, object>>> _initializedPrefabs;
    
    public PrefabManager(IDataLoader dataLoader)
    {
        _dataLoader = dataLoader ?? throw new ArgumentNullException(nameof(dataLoader));

        Dictionary<PrefabType, Prefab> prefabs = dataLoader.LoadPrefabs();
        _initializedPrefabs = Initialize(prefabs);
    }

    private Dictionary<PrefabType, List<KeyValuePair<Type, object>>> Initialize(Dictionary<PrefabType, Prefab> prefabs)
    {
        var initializedPrefabs = new Dictionary<PrefabType, List<KeyValuePair<Type, object>>>();
        
        foreach(KeyValuePair<PrefabType, Prefab> prefab in prefabs)
        {
            List<KeyValuePair<Type, object>> initializedComponents = new List<KeyValuePair<Type, object>>();
            foreach (var compPref in prefab.Value.Components)
            {
                Type type = GetComponentTypeFromName(compPref.TypeName);
                object component = CreateComponentInstance(type);
                SetComponentProperties(component, type, compPref.Parameters);
                
                initializedComponents.Add(new KeyValuePair<Type, object>(type, component));
            }
            
            initializedPrefabs.Add(prefab.Key, initializedComponents);
        }

        return initializedPrefabs;
    }

    public int CreateEntityFromPrefab(EntityFactory factory, PrefabType prefabId)
    {
        if (!_initializedPrefabs.TryGetValue(prefabId, out List<KeyValuePair<Type, object>> loadedPrefab))
        {
            throw new ArgumentException($"Prefab with '{prefabId}' is not found.");
        }
            
        factory.Create();

        foreach (KeyValuePair<Type, object> component in loadedPrefab)
        {
            AddComponentToFactory(factory, component.Key, component.Value);
        }
            
        return factory.End();
    }

    private Type GetComponentTypeFromName(string typeName)
    {
        Type? type = Type.GetType(typeName);
        if (type == null || !type.IsValueType || Nullable.GetUnderlyingType(type) != null)
        {
            throw new ArgumentException($"Can't get type from namespace for '{typeName}.'");
        }

        return type;
    }

    private object CreateComponentInstance(Type type)
    {
        object? component = Activator.CreateInstance(type);
        if (component == null)
        {
            throw new ArgumentException($"Failed to create an instance of type '{type.FullName}'.");
        }

        return component;
    }

    private void SetComponentProperties(object component, Type type, Dictionary<string, object> parameters)
    {
        foreach (var param in parameters)
        {
            PropertyInfo propertyInfo = type.GetProperty(param.Key)
                                        ?? throw new ArgumentException($"Wrong property name '{param.Key}'.");

            object value = ConvertParamToPropertyValue(propertyInfo, param.Value);
            propertyInfo.SetValue(component, value);
        }
    }
    
    private object ConvertParamToPropertyValue(PropertyInfo propertyInfo, object paramValue)
    {
        object value;
        if (propertyInfo.PropertyType.IsEnum)
        {
            // Convert string to enum
            value = Enum.Parse(propertyInfo.PropertyType, (string)paramValue);
        }
        else if (propertyInfo.PropertyType == typeof(Vector2))
        {
            var parts = ((string)paramValue).Split(' ');
            if (parts.Length != 2)
            {
                throw new ArgumentException("Vector2 must be written like \"0.5 0.5\" with space between numbers");
            }
                    
            // Convert string to Vector2
            float x = float.Parse(parts[0], CultureInfo.InvariantCulture);
            float y = float.Parse(parts[1], CultureInfo.InvariantCulture);
            value = new Vector2(x, y); 
        }
        else if (propertyInfo.PropertyType.IsPrimitive || propertyInfo.PropertyType == typeof(string))
        {
            // Convert string to basic types like float/string/int etc
            value = Convert.ChangeType(paramValue, propertyInfo.PropertyType);
        }
        else
        {
            // Convert string to complicated types like arrays/classes/dictionaries etc
            value = _dataLoader.LoadObject(paramValue, propertyInfo.PropertyType);
        }

        return value;
    }

    private void AddComponentToFactory(EntityFactory factory, Type type, object component)
    {
        MethodInfo addMethod = factory.GetType().GetMethod("Add").MakeGenericMethod(type);
        addMethod.Invoke(factory, new[] { component });
    }
}
