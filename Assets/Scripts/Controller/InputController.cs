using UnityEngine;

public class InputController : MonoBehaviour
{
    // The UI-Controller is needed, to refresh the shot counter
    private UIController    _uiController;

    // The Camera-View is needed, to start the follow-sequence at a shot, or to move the camera in Editor-Mode
    private CameraView      _cameraView;

    // The Player-Weapon is needed to refresh the aiming or to trigger the shot with touch-release (or left-click in Editor-Mode)
    private Weapon          _playerWeapon;


    private bool            _hasTouched;


    private void Awake()
    {
        _uiController       = GameObject.Find("Main")   .GetComponent<UIController>();
        _cameraView         = GameObject.Find("Main")   .GetComponent<CameraView>();
        _playerWeapon       = GameObject.Find("Player") .GetComponentInChildren<Weapon>();

        _hasTouched         = false;
    }



    void Update()
    {
        // SMARTPHONE TOUCH INTERACTION
        //
        //
        if (LevelData.CurrentTurnState == LevelData.TurnState.AIMING)
        {
            // TOUCH DETECTED ==> Aim
            //
            if (Input.touchCount > 0
              && (  Input.GetTouch(0).phase == TouchPhase.Began
                 || Input.GetTouch(0).phase == TouchPhase.Moved))
            {
                _hasTouched = true;

                Aim(Input.GetTouch(0).position);
            }

            // TOUCH RELEASED => Shoot
            //
            else if (Input.touchCount == 0
                  && _hasTouched)
            {
                _hasTouched = false;

                Shot();
            }
        }


        // KEYBOARD INTERACTION (only for Editor-Mode)
        //
        //

#if UNITY_EDITOR

        if (LevelData.CurrentTurnState == LevelData.TurnState.AIMING)
        {
            // MOUSE POSITION => Aim
            //
            Aim(Input.mousePosition);

            // LEFT CLICK => Shoot
            //
            if (Input.GetMouseButton(0))
            {
                Shot();
            }
        }

        // ZOOMING (Keyboard)
        //
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            _cameraView.SetZoom(-Input.GetAxis("Mouse ScrollWheel"));
        }

        // SCROLL CAMERA LEFT (Keyboard)
        //
        if ( Input.GetKey(KeyCode.A)
          || Input.GetKey(KeyCode.LeftArrow))
        {
            _cameraView.MoveCamera(Vector3.left);
        }

        // SCROLL CAMERA RIGHT (Keyboard)
        //
        if ( Input.GetKey(KeyCode.D)
          || Input.GetKey(KeyCode.RightArrow))
        {
            _cameraView.MoveCamera(Vector3.right);
        }
#endif

    }

    private void Aim(Vector3 aimPosition)
    {
        _playerWeapon.AimToPosition(aimPosition);
    }

    private void Shot()
    {
        LevelData.CurrentTurnState = LevelData.TurnState.SHOT_SEQUENCE;
        LevelData.ShotCount++;

        _uiController.RefreshShotCounter();

        _cameraView.StartFollowSequence(_playerWeapon.Shoot());
    }

}
