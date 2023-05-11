using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using FlurrySDKInternal;
using System.Reflection;
#if DPN_FLURRY
using FlurrySDK;
#endif

[assembly: RequireDependency("FLURRY", sourceClass = "FlurrySDK.Flurry")]

#if DPN_FLURRY

public class FlurryReporter : Reporter
{
    public Platform platform;
    public string apiKey;

    public override bool DebugLog => true;
    public override string DebugLogPrefix => "<color=#4cd05c><b>Flurry ► </b></color>";

    FlurryAgent flurryAgent;

    protected override void OnInitialize()
    {
        if (Application.platform == RuntimePlatform.Android && platform == Platform.Android)
            flurryAgent = Build(apiKey);
        else if (Application.platform == RuntimePlatform.IPhonePlayer && platform == Platform.IOS)
            flurryAgent = Build(apiKey);
        else if (Application.isEditor)
        {
#if UNITY_ANDROID
            if (platform == Platform.Android)
                flurryAgent = Build(apiKey);
#elif UNITY_IOS
            if (platform == Platform.IOS)
                flurryAgent = Build(apiKey);
#endif
        }

        Debug.Log("Build agent " + flurryAgent);
    }

    FlurryAgent Build(string apiKey)
    {
        new Flurry.Builder()
                  .WithCrashReporting(true)
                  .WithLogEnabled(true)
                  .WithLogLevel(Flurry.LogLevel.VERBOSE)
                  .WithMessaging(true)
                  .Build(apiKey);

        FlurryAgent flurryAgent = typeof(Flurry).GetField("flurryAgent", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) as FlurryAgent;
        return flurryAgent;
    }

    protected override void OnLog(EventParameters eventParameters)
    {
        Dictionary<string, string> stringDictionary = eventParameters.ToDictionary(EventParameters.Case.Pascal).ToDictionary(pair => pair.Key, pair => pair.Value.ToString());
        
        if (flurryAgent != null)
            flurryAgent.LogEvent(eventParameters.Name, stringDictionary);
    }

    protected override void OnLog(string eventId)
    {
        if (flurryAgent != null)
            flurryAgent.LogEvent(eventId);
    }

    protected override void OnLog(string eventId, Dictionary<string, object> parameters)
    {
        if (flurryAgent != null)
            flurryAgent.LogEvent(eventId, parameters.ToDictionary(pair => pair.Key, pair => pair.Value.ToString()));
    }

    public enum Platform
    {
        Android,
        IOS
    }

}

#endif