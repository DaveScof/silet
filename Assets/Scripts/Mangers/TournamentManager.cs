using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scaffold;
using Firebase.Database;
using Firebase.Auth;
using Firebase;
using AppAdvisory.MathGame;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Linq;
using UnityEngine.Networking;
using System.IO;

public class TournamentManager : Manager<TournamentManager>
{
    private FirebaseAuth auth;
    private DatabaseReference reference;
    private FirebaseApp app;
    public TournamentRule tournamentRule;
    public TournamentData userTournamentData;
    public string tournamentName = "tournament-1";
    public Dictionary<int, string> leaderBoardData;


    public string cachePath;

    #region events

    public delegate void Notify();
    public event Notify TournamentDataChangedEvent;
    public event Notify TournamentImageEvent;


    #endregion

    private void Start()
    {
        leaderBoardData = new Dictionary<int, string>();
        app = FirebaseApp.Create(
            options: new Firebase.AppOptions
            {
                ApiKey = "AIzaSyDEQABN30EW-rzx3jBCsnngQf7aO1PLstA",
                DatabaseUrl = new System.Uri("https://mathgame-9c6ce.firebaseio.com"),
                ProjectId = "tras-9c6ce",
            }
        );

        KinetManager.Instance.InitializeAuth(app);
        reference = FirebaseDatabase.GetInstance(app).RootReference;

        reference.Child("tournament-active").ValueChanged += ActiveTournament;
    }

    public void ActiveTournament(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError("Firebase Realtime Database error: " + args.DatabaseError);
            return;
        }
        // DataSnapshot is a snapshot of the data at a Firebase Database location.
        DataSnapshot snapshot = args.Snapshot;
        Debug.Log(snapshot.Value);
        tournamentName = snapshot.Value.ToString();
        reference.Child(tournamentName).Child("rules").ValueChanged += HandleTournamentRuleChanged;
        reference.Child(tournamentName).Child("players").OrderByChild("score").LimitToLast(10).ValueChanged += HandlePlayerValueChanged;

    }

    public async void GetUserProfile()
    {
        var tournament = await reference.Child(tournamentName).Child("players").Child(KinetManager.Instance.token).GetValueAsync();
        if (tournament.Exists)
        {
            userTournamentData = JsonUtility.FromJson<TournamentData>(tournament.GetRawJsonValue());
        }
        else
        {
            ScoreManager.ResetScore();
            userTournamentData = new TournamentData(KinetManager.Instance.PhoneNumber, new Score("0", System.DateTime.Now.ToString(), 0), KinetManager.Instance.token);
            StoreScore("0", 0);
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
            leaderBoardData.Add(index, dict2["score"].ToString());
        }
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
        StartCoroutine(CacheImage(imagePath));
        tournamentRule = new TournamentRule(date, snapshot.Child("status").Value.ToString());
    }

    public void StoreScore(string score, int level)
    {
        if (tournamentRule.status == TournamentStatus.STOPPED) return;

        string kinteID = KinetManager.Instance.token;
        string phone = KinetManager.Instance.PhoneNumber;

        if (int.Parse(score) < int.Parse(userTournamentData.score)) return;

        Score scoreJson = new Score(score, System.DateTime.Now.ToString(), level);
        TournamentData tournamentData = new TournamentData(phone, scoreJson, kinteID);

        reference.Child(tournamentName).Child("players").Child(kinteID).SetRawJsonValueAsync(JsonUtility.ToJson(tournamentData)).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Firebase Realtime Database error: " + task.Exception);
                return;
            }
        });
        reference.Child(tournamentName).Child("players").Child(kinteID).Child("scores").Child(score).SetRawJsonValueAsync(JsonUtility.ToJson(scoreJson)).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Firebase Realtime Database error: " + task.Exception);
                return;
            }
        });
    }

    IEnumerator CacheImage(string imageUrl)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(www);
            byte[] bytes = texture.EncodeToPNG();

            File.WriteAllBytes(cachePath, bytes);
            TournamentImageEvent?.Invoke();

            Debug.Log("Image cached: " + cachePath);
        }
        else
        {
            Debug.LogError("Failed to cache image: " + www.error);
        }
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
    public string score;
    public int level;

    public PlayersData(string score, int level)
    {
        this.score = score;
        this.level = level;
    }
}

[System.Serializable]
public class TournamentData
{
    public string phone;
    public string score;
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
    public string score;
    public string date;
    public int level;

    public Score(string score, string date, int level)
    {
        this.score = score;
        this.date = date;
        this.level = level;
    }
}