using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class TournamentDisplay : MonoBehaviour
{
    public TMP_Text[] leaderBoards;
    public Image image;
    public TMP_Text counterTMP;

    [Space()]
    public string cachePath;
    private IEnumerator coroutine;

    private void Start()
    {
        TournamentManager.Instance.TournamentImageEvent += LoadImage;
        coroutine = Counter();
    }

    private void OnEnable()
    {
        DisplayLeaderBoard();
        TournamentManager.Instance.TournamentDataChangedEvent += DisplayLeaderBoard;
        LoadImage();

        if (coroutine == null)
            coroutine = Counter();

        StartCoroutine(coroutine);
    }


    public void LoadImage()
    {
        string newCachePath = TournamentManager.Instance.cachePath;
        if (!newCachePath.Equals(cachePath))
        {
            cachePath = newCachePath;
            Davinci.get().setCached(true).setEnableLog(true).load(cachePath).setFadeTime(2).into(image).start();
        }
    }


    private void OnDisable()
    {
        StopCoroutine(coroutine);
        TournamentManager.Instance.TournamentDataChangedEvent -= DisplayLeaderBoard;
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }

    IEnumerator Counter()
    {
        System.DateTime now = System.DateTime.Now;
        System.DateTime endDate = TournamentManager.Instance.tournamentRule.endDate;

        Debug.Log(endDate > now);
        while (endDate > now)
        {
            yield return new WaitForSeconds(1);
            now = System.DateTime.Now;
            var difference = endDate - now;
            counterTMP.text = $"{difference.Days}d {difference.Hours}h {difference.Minutes}m {difference.Seconds}s";
        }
        yield return null;
    }

    public void DisplayLeaderBoard()
    {
        Dictionary<int, string> leaderboard = TournamentManager.Instance.leaderBoardData;

        foreach (var item in leaderboard)
        {
            leaderBoards[item.Key].text = $"{item.Key + 1}    {item.Value}";
        }
    }

}
