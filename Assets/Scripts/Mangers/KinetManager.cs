using Firebase.Auth;
using UnityEngine;

public class KinetManager : Manager<KinetManager>, KinetSubscription.ServiceTokenRequester
{
    public KinetSubscription kinetSubscription;

    public string PhoneNumber => "251910994481";
    public string token;

    string KinetSubscription.ServiceTokenRequester.Service => "tras";
    FirebaseAuth auth;

    bool KinetSubscription.ServiceTokenRequester.ShouldFetchServiceToken => true;

    void Start()
    {
        kinetSubscription.RequestServiceToken(this);
    }

    public void InitializeAuth(Firebase.FirebaseApp app)
    {
        auth = FirebaseAuth.GetAuth(app);
    }

    public bool CheckIsPlayable()
    {
        return kinetSubscription.CheckIsPlayable();
    }

    void KinetSubscription.ServiceTokenRequester.OnServiceTokenFetched(string token)
    {
        this.token = token;
        TournamentManager.Instance.Login(token);
    }

    public void AddListenerForSubscription(System.Action listener)
    {
        if (kinetSubscription.IsLoggedIn)
        {
            kinetSubscription.onFetchSubscriptionStatus += listener;
        }
        else
        {
            listener();
        }
    }

    public void LogOut()
    {

        kinetSubscription.Logout();
        kinetSubscription.CheckIsPlayable();
    }


    //
}
