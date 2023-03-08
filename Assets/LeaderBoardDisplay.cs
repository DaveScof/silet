using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderBoardDisplay : MonoBehaviour
{
    public GameObject tournamentPanel;
    public GameObject tournamentLeaderBoard;
    public GameObject noTournament;

    public void OpenTournament()
    {
        tournamentPanel.SetActive(true);
        if (TournamentManager.Instance.isTournamentAvaiable())
        {
            tournamentLeaderBoard.SetActive(true);
        }
        else
        {
            noTournament.SetActive(true);
        }
    }

    public void Close()
    {
        tournamentPanel.SetActive(false);
        tournamentLeaderBoard.SetActive(false);
        noTournament.SetActive(false);
    }
}
