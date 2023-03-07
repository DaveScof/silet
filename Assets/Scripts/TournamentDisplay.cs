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
    public string cachePath;

    private void Start()
    {
        TournamentManager.Instance.TournamentImageEvent += LoadImage;
    }

    private void OnEnable()
    {
        DisplayLeaderBoard();
        TournamentManager.Instance.TournamentDataChangedEvent += DisplayLeaderBoard;
        LoadImage();
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

    Texture2D LoadTextureFromCache(string path)
    {
        if (File.Exists(path))
        {
            byte[] bytes = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(bytes);
            return texture;
        }

        return null;
    }

    private void OnDisable()
    {
        TournamentManager.Instance.TournamentDataChangedEvent -= DisplayLeaderBoard;
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
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
