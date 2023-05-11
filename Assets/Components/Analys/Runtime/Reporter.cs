using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Reporter : MonoBehaviour
{
    public abstract bool DebugLog { get; }
    public abstract string DebugLogPrefix { get; }

    bool isInitialized;

    public void Initialize()
    {
        if (isInitialized)
            return;

        OnInitialize();
        isInitialized = true;
    }

    protected abstract void OnInitialize();

    public void Log(EventParameters eventParameters)
    {
        OnLog(eventParameters);

        if (DebugLog)
            Debug.Log($"{DebugLogPrefix} Event: {eventParameters.Name} \n{JsonUtility.ToJson(eventParameters, true)}");
    }

    public void Log(string eventId)
    {
        OnLog(eventId);

        if (DebugLog)
            Debug.Log($"{DebugLogPrefix} Event: {eventId}");
    }

    public void Log(string eventId, Dictionary<string, object> parameters)
    {
        OnLog(eventId, parameters);

        if (DebugLog)
            Debug.Log($"{DebugLogPrefix} Event: {eventId} \n{parameters.PrintVal(true)}");
    }

    protected abstract void OnLog(EventParameters eventParameters);
    protected abstract void OnLog(string eventId);
    protected abstract void OnLog(string eventId, Dictionary<string, object> parameters);

}
