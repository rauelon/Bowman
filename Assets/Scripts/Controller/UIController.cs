using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    // Text in the top left corner
    public Text ShotCounterText;

    // Text that appears, if the player hit, missed or won the game.
    public Text DisplayText;

    public void RefreshShotCounter()
    {
        ShotCounterText.text = $"Shots: {LevelData.ShotCount}";
    }

    public void ShowText(string text)
    {
        DisplayText.gameObject.SetActive(true);

        DisplayText.text = text;
    }


    public void HideDisplayText()
    {
        DisplayText.gameObject.SetActive(false);
    }

}
