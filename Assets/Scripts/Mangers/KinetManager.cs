using Core.Net;
using Firebase.Auth;
using Firebase;
using Scaffold;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinetManager : Manager<KinetManager>, KinetSubscription.ServiceTokenRequester
{
    public KinetSubscription kinetSubscription;

    public string PhoneNumber => "251910994481";
    public string token;

    string KinetSubscription.ServiceTokenRequester.Service => "tras";
    FirebaseAuth auth;

    bool KinetSubscription.ServiceTokenRequester.ShouldFetchServiceToken => false;

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
        TournamentManager.Instance.GetUserProfile();
    }


    public void LogOut()
    {
        kinetSubscription.Logout();
    }


    //
}
