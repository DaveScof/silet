using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public abstract class EventParameters
{
    public static Dictionary<Type, FieldInfo[]> typeFieldInfos = new Dictionary<Type, FieldInfo[]>();
    public abstract string TaxonomyId { get; }
    public abstract string Name { get; }

    public Dictionary<string, object> ToDictionary(Case fieldCase)
    {
        return ToDictionary(this, fieldCase);
    }


    public static Dictionary<string, object> ToDictionary(object parameters, Case fieldCase)
    {
        Dictionary<string, object> parameterDictionary = new Dictionary<string, object>();
        Type type = parameters.GetType();

        foreach (FieldInfo field in GetFields(type))
        {
            string fieldName = field.Name;

            if (fieldCase == Case.Title)
                fieldName = CoreU.PascalToTitle(field.Name);
            else if (fieldCase == Case.Snake)
                fieldName = CoreU.PascalToSnake(field.Name);

            parameterDictionary.Add(fieldName, field.GetValue(parameters));
        }

        return FilterNulls(parameterDictionary);

    }
    static Dictionary<string, object> FilterNulls(Dictionary<string, object> dictionary)
    {
        List<string> toRemove = new List<string>();
        foreach (string key in dictionary.Keys)
            if (dictionary[key] == null)
                toRemove.Add(key);

        foreach (string remove in toRemove)
            dictionary.Remove(remove);

        return dictionary;
    }

    public static FieldInfo[] GetFields(Type type)
    {
        if (typeFieldInfos.ContainsKey(type))
            return typeFieldInfos[type];

        FieldInfo[] fieldInfos = type.GetFields();
        typeFieldInfos[type] = fieldInfos;
        return fieldInfos;
    }

    public enum Case
    {
        Pascal,
        Title,
        Snake
    }
}
