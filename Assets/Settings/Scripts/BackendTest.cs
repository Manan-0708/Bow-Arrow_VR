using UnityEngine;

public class BackendTest : MonoBehaviour
{
    void Start()
    {
        PlayerManager.Instance.RegisterNewPlayer("Manan");
        PlayerManager.Instance.RegisterNewPlayer("Vedant");

        FileDataManager.Instance.UpdateLeaderboard(
            "Manan", 850, 78.5f, 47, 60
        );

        FileDataManager.Instance.UpdateLeaderboard(
            "Vedant", 920, 81.2f, 52, 64
        );
    }
}
