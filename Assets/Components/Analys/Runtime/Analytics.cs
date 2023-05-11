using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;
using Newtonsoft;
using System.Text.RegularExpressions;

#if DPN_FIREBASE_ANALYTICS
using Firebase.Analytics;
#endif

namespace Analys
{
    public partial class Analytics
    {
        static List<EventParameters> queuedEventParameters = new List<EventParameters>();
        static List<Taxonomy> taxonomies = new List<Taxonomy>();

        public static void Initialize(List<Taxonomy> taxonomies)
        {
            Analytics.taxonomies = taxonomies;

            foreach (Taxonomy taxonomy in taxonomies)
                foreach (Reporter reporter in taxonomy.Reporters)
                    reporter.Initialize();

            SendQueuedEvents();
        }

        static void SendQueuedEvents()
        {
            foreach (EventParameters eventParameters in queuedEventParameters)
                LogEvent(eventParameters);

            queuedEventParameters.Clear();
        }

        public static void LogEvent(EventParameters eventParameters)
        {
            foreach (Taxonomy taxonomy in taxonomies)
                if (eventParameters.TaxonomyId == taxonomy.id)
                    foreach (Reporter reporter in taxonomy.Reporters)
                        reporter.Log(eventParameters);
        }
        
        public static void LogEvent(string eventId, string taxonomyId)
        {
            Taxonomy taxonomy = taxonomies.Find(perTaxonomy => perTaxonomy.id == taxonomyId);

            if (taxonomy == null)
                Debug.LogError($"No taxonomy with id '{taxonomyId}' found.");

            foreach (Reporter reporter in taxonomy.Reporters)
                reporter.Log(eventId);

        }

        public static void LogEvent(string eventId, string taxonomyId, Dictionary<string, object> parameters)
        {
            Taxonomy taxonomy = taxonomies.Find(perTaxonomy => perTaxonomy.id == taxonomyId);

            if (taxonomy == null)
                Debug.LogError($"No taxonomy with id '{taxonomyId}' found.");

            foreach (Reporter reporter in taxonomy.Reporters)
                reporter.Log(eventId, parameters);

        }

        public class LoginEventParameter : EventParameters
        {
            public override string Name => "LoginEventParameter";
            public override string TaxonomyId => "KINET";

            public string status;
            public string category;
            public LoginEventParameter(string status)
            {
                this.status = status;
            }
        }
    }

}