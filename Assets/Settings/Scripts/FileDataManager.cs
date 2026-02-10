using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class FileDataManager : MonoBehaviour
{
    public static FileDataManager Instance;

    string leaderboardPath;
    string sessionPath;
    string playersPath;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        leaderboardPath = Path.Combine(Application.persistentDataPath, "leaderboard.csv");
        sessionPath = Path.Combine(Application.persistentDataPath, "session.csv");
        playersPath = Path.Combine(Application.persistentDataPath, "players.csv");

        CreateFilesIfMissing();

        Debug.Log("CSV Path: " + Application.persistentDataPath);
    }

    void CreateFilesIfMissing()
    {
        if (!File.Exists(playersPath))
            File.WriteAllText(playersPath, "Player\n");

        if (!File.Exists(leaderboardPath))
            File.WriteAllText(leaderboardPath, "Player,Score,Accuracy,TotalHits,TotalArrows\n");

        if (!File.Exists(sessionPath))
            File.WriteAllText(sessionPath, "Player,Arrows,Hits,Misses,Accuracy\n");
    }

    // -------- PLAYER LIST ----------
    public List<string> GetAllPlayers()
    {
        List<string> players = new List<string>();

        string[] lines = File.ReadAllLines(playersPath);
        for (int i = 1; i < lines.Length; i++) // skip header
            players.Add(lines[i]);

        return players;
    }

    public bool PlayerExists(string playerName)
    {
        List<string> players = GetAllPlayers();
        return players.Contains(playerName);
    }

    public void AddPlayer(string playerName)
    {
        if (!PlayerExists(playerName))
        {
            File.AppendAllText(playersPath, playerName + "\n");
            Debug.Log("Player added: " + playerName);
        }
        else
        {
            Debug.Log("Player already exists: " + playerName);
        }
    }

    // -------- LEADERBOARD ----------
    public void UpdateLeaderboard(string playerName, int score, float accuracy, int hits, int arrows)
    {
        List<string> lines = new List<string>(File.ReadAllLines(leaderboardPath));

        bool found = false;

        for (int i = 1; i < lines.Count; i++)
        {
            string[] data = lines[i].Split(',');

            if (data[0] == playerName)
            {
                lines[i] = $"{playerName},{score},{accuracy},{hits},{arrows}";
                found = true;
                break;
            }
        }

        if (!found)
        {
            lines.Add($"{playerName},{score},{accuracy},{hits},{arrows}");
        }

        File.WriteAllLines(leaderboardPath, lines);
    }

    public List<string[]> GetLeaderboard()
    {
        List<string[]> rows = new List<string[]>();
        string[] lines = File.ReadAllLines(leaderboardPath);

        for (int i = 1; i < lines.Length; i++)
            rows.Add(lines[i].Split(','));

        return rows;
    }
}
