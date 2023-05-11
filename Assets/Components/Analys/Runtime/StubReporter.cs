using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StubReporter : Reporter
{
    public override bool DebugLog => true;

    public override string DebugLogPrefix => "<color=#888888><b>Stub ► </b></color>";

    protected override void OnInitialize()
    {

    }

    protected override void OnLog(EventParameters eventParameters)
    {

    }

    protected override void OnLog(string eventId)
    {

    }

    protected override void OnLog(string eventId, Dictionary<string, object> parameters)
    {

    }
}
