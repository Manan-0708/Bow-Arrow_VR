using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;

    public List<PlayerData> players = new List<PlayerData>();

    private string filePath;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);


        filePath = Application.persistentDataPath + "/player_data.csv";

        LoadCSV();
    }

    public void AddNewPlayer(string name)
    {
        PlayerData p = new PlayerData();
        p.playerName = name;
        p.arrowsShot = 0;
        p.hits = 0;
        p.misses = 0;
        p.accuracy = 0;
        p.sessionsPlayed = 1;

        players.Add(p);
        SaveCSV();
    }

    public PlayerData GetPlayer(string name)
    {
        return players.Find(p => p.playerName == name);
    }

    public void SaveCSV()
    {
        List<string> lines = new List<string>();
        lines.Add("Name,ArrowsShot,Hits,Misses,Accuracy,SessionsPlayed");

        foreach (PlayerData p in players)
        {
            lines.Add($"{p.playerName},{p.arrowsShot},{p.hits},{p.misses},{p.accuracy},{p.sessionsPlayed}");
        }

        File.WriteAllLines(filePath, lines);
    }

    public void LoadCSV()
    {
        players.Clear();

        if (!File.Exists(filePath)) return;

        string[] lines = File.ReadAllLines(filePath);

        for (int i = 1; i < lines.Length; i++) // skip header
        {
            string[] values = lines[i].Split(',');

            PlayerData p = new PlayerData();
            p.playerName = values[0];
            p.arrowsShot = int.Parse(values[1]);
            p.hits = int.Parse(values[2]);
            p.misses = int.Parse(values[3]);
            p.accuracy = float.Parse(values[4]);
            p.sessionsPlayed = int.Parse(values[5]);

            players.Add(p);
        }
    }
}
