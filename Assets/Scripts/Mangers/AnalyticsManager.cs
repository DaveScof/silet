using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsManager : Manager<AnalyticsManager>
{
    public List<Taxonomy> taxonomies;

    public override void AfterInitialized()
    {
        Analys.Analytics.Initialize(taxonomies);
    }

}
