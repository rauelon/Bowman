using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{

    public  bool        IsInvulnerable;
    public  bool        IsDestroyable;

    public  float       MaxHitPoints = 5;

    [HideInInspector]
    public  float       CurrentHitPoints;

    private Slider      _hitBar;


    private void Awake()
    {
        if (!IsInvulnerable)
        {
            _hitBar = GetComponentInChildren<Slider>();
            _hitBar.gameObject.SetActive(true);

            CurrentHitPoints = MaxHitPoints;
        }
    }

    public void DealDamage(float damageAmount)
    {
        if (!IsInvulnerable)
        {
            CurrentHitPoints -= damageAmount;

            ResetHitbar();

            if (CurrentHitPoints <= 0
             && IsDestroyable)
            {
                Destroy(gameObject);
            }
        }
    }

    public void ResetHitbar()
    {
        _hitBar.value = CurrentHitPoints / MaxHitPoints;
    }
}
