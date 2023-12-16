using System.Globalization;
using System.Numerics;
using System.Reflection;
using MysticEchoes.Core.Loaders.Prefabs;
using MysticEchoes.Core.Scene;

namespace MysticEchoes.Core.Loaders;

// TODO store List<objects> - components for each prefab
public class PrefabManager
{
    private IDataLoader _dataLoader;

    private Dictionary<PrefabType, Prefab> _prefabs;
    
    public PrefabManager(IDataLoader dataLoader)
    {
        _dataLoader = dataLoader;
        
        _prefabs = dataLoader.LoadPrefabs();
    }

    public int CreateEntityFromPrefab(EntityFactory factory, PrefabType prefabId)
    {
        Prefab prefab = GetPrefab(prefabId);

        factory.Create();

        foreach (var compPref in prefab.Components)
        {
            Type type = GetComponentTypeFromName(compPref.TypeName);
            object component = CreateComponentInstance(type);
            SetComponentProperties(component, type, compPref.Parameters);
            AddComponentToFactory(factory, type, component);
        }

        return factory.End();
    }
    
    private Prefab GetPrefab(PrefabType prefabId)
    {
        if (!_prefabs.TryGetValue(prefabId, out Prefab prefab))
        {
            throw new ArgumentException($"Prefab with '{prefabId}' is not found.");
        }

        return prefab;
    }

    private Type GetComponentTypeFromName(string typeName)
    {
        Type? type = Type.GetType(typeName);
        if (type == null || !type.IsValueType || Nullable.GetUnderlyingType(type) != null)
        {
            throw new ApplicationException($"Can't get type from namespace for '{typeName}.'");
        }

        return type;
    }

    private object CreateComponentInstance(Type type)
    {
        object? component = Activator.CreateInstance(type);
        if (component == null)
        {
            throw new ApplicationException($"Failed to create an instance of type '{type.FullName}'.");
        }

        return component;
    }

    private void SetComponentProperties(object component, Type type, Dictionary<string, object> parameters)
    {
        foreach (var param in parameters)
        {
            PropertyInfo propertyInfo = type.GetProperty(param.Key)
                                        ?? throw new ApplicationException($"Wrong property name '{param.Key}'.");

            object value = ConvertParamToPropertyValue(propertyInfo, param.Value);
            propertyInfo.SetValue(component, value);
        }
    }
    
    private object ConvertParamToPropertyValue(PropertyInfo propertyInfo, object paramValue)
    {
        // TODO TRY refactor
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
                throw new ApplicationException("Vector2 must be written like \"0.5 0.5\" with space between numbers");
            }
                    
            // Convert string to Vector2
            float x = float.Parse(parts[0], CultureInfo.InvariantCulture);
            float y = float.Parse(parts[1], CultureInfo.InvariantCulture);
            value = new Vector2(x, y); 
        }
        else if (propertyInfo.PropertyType.IsGenericType)
        {
            value = _dataLoader.LoadObject(paramValue, propertyInfo.PropertyType);
        }
        else
        {
            // Convert string to basic types like float/string/int etc
            value = Convert.ChangeType(paramValue, propertyInfo.PropertyType);
        }

        return value;
    }

    private void AddComponentToFactory(EntityFactory factory, Type type, object component)
    {
        MethodInfo addMethod = factory.GetType().GetMethod("Add").MakeGenericMethod(type);
        addMethod.Invoke(factory, new[] { component });
    }
}
