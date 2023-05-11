using Firebase.Analytics;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[assembly: RequireDependency("FIREBASE_ANALYTICS", sourceClass = "Firebase.Analytics.FirebaseAnalytics")]

#if DPN_FIREBASE_ANALYTICS

public class FirebaseReporter : Reporter
{
    public override bool DebugLog => true;
    public override string DebugLogPrefix => "<color=#00FF00><b>Firebase ► </b></color>";

    protected override void OnInitialize()
    {

    }

    public Parameter[] ToFirebaseParameters(object parametersObject)
    {
        List<Parameter> firebaseParameters = new List<Parameter>();

        FieldInfo[] fieldsOfParameterObject = EventParameters.GetFields(parametersObject.GetType());

        foreach (FieldInfo field in fieldsOfParameterObject)
            if (field.FieldType == typeof(float) || field.FieldType == typeof(double))
                firebaseParameters.Add(new Parameter(CoreU.PascalToTitle(field.Name), (double)field.GetValue(parametersObject)));
            else if (field.FieldType == typeof(long) || field.FieldType == typeof(int))
                firebaseParameters.Add(new Parameter(CoreU.PascalToTitle(field.Name), (long)field.GetValue(parametersObject)));
            else if (field.FieldType == typeof(string) && field.GetValue(parametersObject) != null)
                firebaseParameters.Add(new Parameter(CoreU.PascalToTitle(field.Name), (string)field.GetValue(parametersObject)));

        return firebaseParameters.ToArray();
    }

    protected override void OnLog(EventParameters eventParameters)
    {
        FirebaseAnalytics.LogEvent(CoreU.TitleToSnake(eventParameters.Name), ToFirebaseParameters(eventParameters));
    }

    protected override void OnLog(string eventId)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnLog(string eventId, Dictionary<string, object> parameters)
    {
        throw new System.NotImplementedException();
    }
}

#endif
