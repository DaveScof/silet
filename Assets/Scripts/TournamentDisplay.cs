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


    private void OnEnable()
    {
        DisplayLeaderBoard();
        TournamentManager.Instance.TournamentDataChangedEvent += DisplayLeaderBoard;
        TournamentManager.Instance.TournamentImageEvent += LoadImage;
        LoadImage();
    }

    public void LoadImage()
    {
        string cachePth = TournamentManager.Instance.cachePath;
        if (string.IsNullOrEmpty(cachePth))
        {
            Texture2D texture = LoadTextureFromCache(cachePth);
            image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
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
