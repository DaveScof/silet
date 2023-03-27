using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class TournamentDisplay : MonoBehaviour
{
    public TMP_Text[] leaderBoardsTMPTexts;
    public Image image;
    public TMP_Text counterTMP;

    [Space()]
    public string tournamentImage;
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
        string newCachePath = TournamentManager.Instance.tournamentImage;
        if (!newCachePath.Equals(tournamentImage))
        {
            tournamentImage = newCachePath;
            Davinci.get().setCached(true).setEnableLog(true).load(tournamentImage).setFadeTime(2).into(image).start();
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
        List<int> leaderboard = TournamentManager.Instance.leaderBoardData;

        foreach (var (i, index) in leaderboard.Select((v, i) => (v, i)))
        {
            if (index < leaderBoardsTMPTexts.Length)
                leaderBoardsTMPTexts[index].text = $"{index + 1}    {i}";

        }
    }

}
