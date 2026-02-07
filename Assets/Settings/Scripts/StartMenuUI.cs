using UnityEngine;
using TMPro;

public class StartMenuUI : MonoBehaviour
{
    public TMP_InputField nameInput;
    public GameObject radialMenu;

    public void OnNewUser()
    {
        string name = nameInput.text;
        if (string.IsNullOrEmpty(name)) return;

        GameDataManager.Instance.AddNewPlayer(name);
        radialMenu.SetActive(true);
        gameObject.SetActive(false);
    }

    public void OnExistingUser()
    {
        string name = nameInput.text;
        PlayerData p = GameDataManager.Instance.GetPlayer(name);

        if (p != null)
        {
            p.sessionsPlayed++;
            GameDataManager.Instance.SaveCSV();

            radialMenu.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
