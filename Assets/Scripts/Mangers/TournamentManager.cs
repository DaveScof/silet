using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using Firebase;
using AppAdvisory.MathGame;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Linq;

public class TournamentManager : Manager<TournamentManager>
{
    [TextArea]
    public string firebaseJsonConfig= "{\"project_info\":{\"project_number\":\"383446369866\",\"firebase_url\":\"https://silet-86a6b-default-rtdb.europe-west1.firebasedatabase.app\",\"project_id\":\"silet-86a6b\",\"storage_bucket\":\"silet-86a6b.appspot.com\"},\"client\":[{\"client_info\":{\"mobilesdk_app_id\":\"1:383446369866:android:47a4a800e0f4b6621208b3\",\"android_client_info\":{\"package_name\":\"com.QeneGames.Silet\"}},\"oauth_client\":[{\"client_id\":\"383446369866-eaf91at2sccsn8naqk972vl80chlvrra.apps.googleusercontent.com\",\"client_type\":3}],\"api_key\":[{\"current_key\":\"AIzaSyDs1Qd9fAQR5f_3veiKfb6emTeIqNQpsEo\"}],\"services\":{\"appinvite_service\":{\"other_platform_oauth_client\":[{\"client_id\":\"383446369866-eaf91at2sccsn8naqk972vl80chlvrra.apps.googleusercontent.com\",\"client_type\":3},{\"client_id\":\"383446369866-edmn7kmojnf8ud2i6kpqaol4nsq35514.apps.googleusercontent.com\",\"client_type\":2,\"ios_info\":{\"bundle_id\":\"com.QeneGames.Silet\"}}]}}}],\"configuration_version\":\"1\"}";

    //private FirebaseAuth auth;
    private DatabaseReference reference;
    private FirebaseApp app;
    [HideInInspector]
    public TournamentRule tournamentRule;
    [HideInInspector]
    public TournamentData userTournamentData;
    [HideInInspector]
    public string tournamentName;
    [HideInInspector]
    public List<int> leaderBoardData;

    Firebase.Auth.FirebaseUser user;
    [HideInInspector]
    public string tournamentImage;

    #region events

    public delegate void Notify();
    public event Notify TournamentDataChangedEvent;
    public event Notify TournamentImageEvent;


    #endregion

    private void Awake()
    {
        tournamentName = "";
        tournamentRule = null;
        userTournamentData = null;
    }

    public bool isTournamentAvailable()
    {
        if (tournamentRule == null)
        {
            return false;
        }
        else
        {
            return tournamentRule.status != TournamentStatus.STOPPED;
        }
    }

    private void Start()

    {
        leaderBoardData = new List<int>();

        FirebaseApp.Create(AppOptions.LoadFromJsonConfig(firebaseJsonConfig), "Silet");
        app = FirebaseApp.GetInstance("Silet");
        reference = FirebaseDatabase.GetInstance(app).RootReference;

        reference.Child("tournament-active").ValueChanged += ActiveTournament;
    }



    public void ActiveTournament(object sender, ValueChangedEventArgs args)
    {
        tournamentName = null;
        tournamentRule = null;

        if (args.DatabaseError != null)
        {
            Debug.LogError("Firebase Realtime Database error: " + args.DatabaseError);
            return;
        }

        DataSnapshot snapshot = args.Snapshot;
        Debug.Log(snapshot.Value);
        if (snapshot.Value != null)
        {
            if (userTournamentData == null)
                GetUserProfile();
            tournamentName = snapshot.Value.ToString();

            reference.Child(tournamentName).Child("rules").ValueChanged += HandleTournamentRuleChanged;
            reference.Child(tournamentName).Child("players").OrderByChild("score").LimitToLast(10).ValueChanged += HandlePlayerValueChanged;
        }
    }


    //public void Login(string token)
    //{
    //    auth.SignInWithCustomTokenAsync(token).ContinueWith(task =>
    //    {
    //        if (task.IsCanceled)
    //        {
    //            Debug.LogError("SignInWithCustomTokenAsync was canceled.");
    //            return;
    //        }
    //        if (task.IsFaulted)
    //        {
    //            Debug.LogError("SignInWithCustomTokenAsync encountered an error: " + task.Exception);
    //            return;
    //        }
    //        user = task.Result;
    //        GetUserProfile();
    //    });
    //}

    public async void GetUserProfile()
    {
        if (string.IsNullOrEmpty(tournamentName)) return;

        string phone = KinetManager.Instance.kinetSubscription.User.Phone;
        var tournament = await reference.Child(tournamentName).Child("players").Child(user.UserId).GetValueAsync();
        if (tournament.Exists)
        {
            userTournamentData = JsonUtility.FromJson<TournamentData>(tournament.GetRawJsonValue());
        }
        else
        {
            ScoreManager.ResetScore();
            userTournamentData = new TournamentData(phone, new Score(0, System.DateTime.Now.ToString(), 0), user.UserId);
            StoreScore(0, 0);
        }
    }

    private void HandlePlayerValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError("Firebase Realtime Database error: " + args.DatabaseError);
            return;
        }
        // DataSnapshot is a snapshot of the data at a Firebase Database location.
        DataSnapshot snapshot = args.Snapshot;
        Dictionary<string, object> dict = snapshot.Value as Dictionary<string, object>;
        leaderBoardData.Clear();
        foreach (var (i, index) in dict.Select((v, i) => (v, i)))
        {
            var dict2 = i.Value as Dictionary<string, object>;
            leaderBoardData.Add(int.Parse(dict2["score"].ToString()));
        }
        leaderBoardData.Sort();
        leaderBoardData.Reverse();
        if (TournamentDataChangedEvent != null)
            TournamentDataChangedEvent.Invoke();
    }

    private void HandleTournamentRuleChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError("Firebase Realtime Database error: " + args.DatabaseError);
            return;
        }
        // DataSnapshot is a snapshot of the data at a Firebase Database location.
        DataSnapshot snapshot = args.Snapshot;
        string date = snapshot.Child("endDate").Value.ToString();
        string imagePath = snapshot.Child("imagePath").Value.ToString();
        tournamentImage = imagePath;
        tournamentRule = new TournamentRule(date, snapshot.Child("status").Value.ToString());
    }

    public void StoreScore(int score, int level)
    {
        if (tournamentRule.status == TournamentStatus.STOPPED) return;

        string kinetID = user.UserId;
        string phone = KinetManager.Instance.kinetSubscription.User.Phone;
        Debug.Log(phone);
        if (score < userTournamentData.score) return;

        Score scoreJson = new Score(score, System.DateTime.Now.ToString(), level);
        TournamentData tournamentData = new TournamentData(phone, scoreJson, kinetID);

        reference.Child(tournamentName).Child("players").Child(kinetID).SetRawJsonValueAsync(JsonUtility.ToJson(tournamentData)).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Firebase Realtime Database error: " + task.Exception);
                return;
            }
        });
        reference.Child(tournamentName).Child("players").Child(kinetID).Child("scores").Child(score.ToString()).SetRawJsonValueAsync(JsonUtility.ToJson(scoreJson)).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Firebase Realtime Database error: " + task.Exception);
                return;
            }
        });
    }

}

[JsonConverter(typeof(StringEnumConverter))]
public enum TournamentStatus
{
    RUNNING, STOPPED
}

[System.Serializable]
public class TournamentRule
{
    public System.DateTime endDate;
    public TournamentStatus status;
    public TournamentRule(string endDate, string status)
    {
        this.endDate = new System.DateTime(System.Convert.ToInt64(endDate));
        if (status == "RUNNING")
            this.status = TournamentStatus.RUNNING;
        else
            this.status = TournamentStatus.STOPPED;
    }
}


public class PlayersData
{
    public int score;
    public int level;

    public PlayersData(int score, int level)
    {
        this.score = score;
        this.level = level;
    }
}

[System.Serializable]
public class TournamentData
{
    public string phone;
    public int score;
    public string date;
    public int level;
    public string kinteID;

    public TournamentData(string phone, Score score, string kinteID)
    {
        this.phone = phone;
        this.score = score.score;
        this.date = score.date;
        this.kinteID = kinteID;
        this.level = score.level;
    }
}


[System.Serializable]
public class Score
{
    public int score;
    public string date;
    public int level;

    public Score(int score, string date, int level)
    {
        this.score = score;
        this.date = date;
        this.level = level;
    }
}