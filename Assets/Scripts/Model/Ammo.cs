using UnityEngine;

public class Ammo : MonoBehaviour
{
    public enum Type
    {
        ARROW,
        BOLT,
        ROCKET
    }

    // Most ammo got a trail in form of a Particle System.
    public bool             HasTrail;

    // The damage, this ammo will deal
    public float            Damage;

    // Optional acceleration (e.g. for a rocket)
    public float            Acceleration;

    // The graphic turns off, if velocity is 0.
    public GameObject       GraphicGO;

    // The physics Component
    private Rigidbody2D     _rigidbody;

    // The trail animation (particle system)
    private ParticleSystem  _trail;

    // The status of flying. If it is not flying, there wont be calculations at runtime.
    private bool            _isFlying;

    // Force multiplier to make the force on rigidbody stronger.
    private float           _forceMultiplier = 50;

    void FixedUpdate()
    {
        if (_isFlying)
        {
            ResetRotation();

            if (Acceleration > 0)
            {
                _rigidbody.AddForce(_rigidbody.velocity * Acceleration * Time.deltaTime);
            }
        }

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name != gameObject.name)
        {
            LevelData.CurrentTurnState = LevelData.TurnState.AMMO_LANDED;

            _isFlying = false;

            if (HasTrail)
            {
                _trail.Stop();
            }

            _rigidbody.Sleep();


            // Because each ammo will call this function only once, the UI-Controller is declared once. There is no need to save it as instance variable.
            UIController uiController = GameObject.Find("Main").GetComponent<UIController>();

            // The collided object has a "Target"-script ==> It was a hit!
            if (collision.GetComponent<Target>())
            {
                Target target = collision.GetComponent<Target>();

                target.DealDamage(Damage);

                // The target is "still alive" ==> Hit!
                if (target.CurrentHitPoints > 0)
                {
                    uiController.ShowText("HIT!");
                }
                // The target is "dead" ==> The game is over
                else
                {
                    uiController.ShowText($"YOU WON!\n\nNeeded Shots: {LevelData.ShotCount}");

                    LevelData.CurrentTurnState = LevelData.TurnState.GAME_OVER;
                }
            }

            // The collided object has no Target-script ==> Missed!
            else
            {
                uiController.ShowText("Missed...");
            }
        }
    }

    // Init method, called from the weapon
    public void Init(float initAngle, float initForce)
    {
        _isFlying = true;

        if (HasTrail)
        {
            _trail = GetComponentInChildren<ParticleSystem>();
        }

        _rigidbody = GetComponent<Rigidbody2D>();

        // Add a force to the rigidbody, rotated by init-angle and multiplied with the init-force.
        _rigidbody.AddForce(CalculateStartForce(initAngle) * initForce * _forceMultiplier);

        ResetRotation();
    }


    private void ResetRotation()
    {
        Vector3 currentVelocity = _rigidbody.velocity;

        // Degree calculated by trigonometric formular:
        //     Tan(alpha)  =           Opposite leg  /  Adjacent leg
        // ==>   alpha     =  Arctan ( Opposite leg  /  Adjacent leg ) 
        //
        float degree = Mathf.Atan2(currentVelocity.y, currentVelocity.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, degree));

        // Turn off Graphic, if the velocity is zero. (e.g. in the first frame after intantiation)
        GraphicGO.SetActive(currentVelocity.magnitude != 0);
    }


    // Start force based on the start angle. 
    // 
    // Vector2.right ==> 0°
    // Following method rotates the vector based on angle:
    private Vector2 CalculateStartForce(float angle)
    {
        Vector2 startForce = Vector2.right;

        float sin = Mathf.Sin(angle * Mathf.Deg2Rad);
        float cos = Mathf.Cos(angle * Mathf.Deg2Rad);

        float tx = startForce.x;
        float ty = startForce.y;

        startForce.x = (cos * tx) - (sin * ty);
        startForce.y = (sin * tx) + (cos * ty);

        return startForce;
    }
}
