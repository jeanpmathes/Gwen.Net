using System;
using System.Collections.Generic;

namespace Gwen.Net.New.Utilities;

/// <summary>
/// A dictionary that maps types to values.
/// It returns the value for the most specific type that is a base type of the requested type.
/// </summary>
public class TypeDictionary<TValue>
{
    private readonly Dictionary<Type, TValue> values = new();
    
    /// <summary>
    /// Get or set the value for the given type.
    /// If the value for the exact type is not found, it will return the value for the most specific base type that has a value.
    /// If no base type has a value, it will return the default value for the type.
    /// Setting the value to null will remove the value for the type from the dictionary.
    /// </summary>
    /// <param name="type">The type to get or set the value for.</param>
    public TValue? this[Type type]
    {
        get
        {
            if (values.TryGetValue(type, out TValue? value)) 
                return value;
            
            Type? baseType = type.BaseType;
            
            while (baseType != null)
            {
                if (values.TryGetValue(baseType, out value)) return value;
                
                baseType = baseType.BaseType;
            }
            
            return default;
        }
        set
        {
            if (value == null)
                values.Remove(type);
            else
                values[type] = value;
        }
    }
}
