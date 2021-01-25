using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type
    {
        BOW,

        // CROSSBOW,           
        // ROCKET_LAUNCHER
        // (Possible extension for later weapon types)
    }

    // Type of the weapon
    public Type                 WeaponType;

    // Type of the ammo
    public GameObject           AmmoPrefab;

    // The container of all fired shots. Will be the parent of created ammo.
    public Transform            AmmoContainer;

    // Saved ammo. Can be cleared, if the level is reset
    private List<GameObject>    _allAmmo;

    // The transform of the player figure
    private Transform           _playerTransform;

    // Current saved angle & force
    private float               angle;
    private float               force;

    // Current saved min- and maxforce
    private float               minForce = 5f;
    private float               maxForce = 30f;


    private void Awake()
    {
        _playerTransform    = GameObject.Find("Player").transform;
        _allAmmo            = new List<GameObject>();
    }


    public void AimToPosition(Vector2 target)
    {
        Vector2 rotationVector = Camera.main.ScreenToWorldPoint(target)     // Position of the mouse curser or touch position
                                 - _playerTransform.position;               // Position of the player


        // Set force based on distance of the target to the player
        force = rotationVector.magnitude;
        force = force < minForce ? minForce : force;
        force = force > maxForce ? maxForce : force;

        // Don't reset the rotation, if aiming of Y is under 0, so the player can't shoot to the ground
        // and if aiming of X is under 0, so the player can't shoot backwards.
        if (rotationVector.x >= 0
         && rotationVector.y >= 0)
        {
            // Set angle
            angle = Mathf.Acos(rotationVector.normalized.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }


    public Transform Shoot()
    {
        GameObject ammoGO = Instantiate(AmmoPrefab, transform.position, Quaternion.identity);

        ammoGO.GetComponent<Ammo>().Init(angle, force);

        ammoGO.transform.parent = AmmoContainer;

        _allAmmo.Add(ammoGO);

        return ammoGO.transform;
    }


    public void ClearAmmoContainer()
    {
        foreach (GameObject ammo in _allAmmo)
        {
            Destroy(ammo);
        }
    }

}
