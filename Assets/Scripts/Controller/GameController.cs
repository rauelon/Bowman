using UnityEngine;

public class GameController : MonoBehaviour
{
    // Target is needed, to reset its live to full
    public Target           Target;

    // Weapon is needed, to clear all shot arrows
    public Weapon           Weapon;

    // UI-Controller is needed, to refresh the counter, after shots are reset to 0.
    private UIController    _uiController;

    private void Awake()
    {
        _uiController = GameObject.Find("Main").GetComponent<UIController>();
    }


    public void ResetLevel()
    {
        LevelData.ShotCount = 0;

        _uiController.RefreshShotCounter();

        Target.CurrentHitPoints = Target.MaxHitPoints;

        Target.ResetHitbar();

        Weapon.ClearAmmoContainer();
    }

}
