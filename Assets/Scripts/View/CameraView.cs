using System.Collections;
using UnityEngine;

public class CameraView : MonoBehaviour
{
    // The Game-Controller is needed for resetting the level, after the final shot is done to the main target.
    private GameController  _gameController;

    // The UI-Controller is needed to hide the text, after the following-sequence
    private UIController    _uiController;

    // Seconds, the camera will stay, after the ammo collided.
    private int             _secondsToWaitAfterFollow   = 2;

    // If the camera follows the shot ammo (follow-sequence), this will be set to "true"
    private bool            _isFollowing;

    // The transform, the camera fill follow at the follow-sequence
    private Transform       _ammoToFollow;

    // Standard settings for the camera at aim-mode
    private Vector3         _standardPosition       = new Vector3(20, 0, -10);
    private float           _standardZoom           = 12.5f;

    // The zoom of the camera while the follow-sequence
    private float           _followZoom             = 7f;


    // Scrolling and zooming (For editor only!)
    private float           _scrollSpeed            = 0.3f;
    private float           _minPositionX           = -100f;
    private float           _maxPositionX           = 100f;

    private float           _zoomIntensity          = 10f;
    private float           _minZoom                = 5f;
    private float           _maxZoom                = 20f;

    private Vector3         _lastSavedCameraPosition;
    private float           _lastSavedCameraZoom;


    private void Awake()
    {
        _gameController     = GameObject.Find("Main").GetComponent<GameController>();
        _uiController       = GameObject.Find("Main").GetComponent<UIController>();
    }

    private void FixedUpdate()
    {
        switch (LevelData.CurrentTurnState)
        {
            case LevelData.TurnState.AMMO_LANDED:

                if (_isFollowing)
                {
                    _isFollowing = false;

                    StartCoroutine(QuitSequence());
                }
                break;

            case LevelData.TurnState.GAME_OVER:

                StartCoroutine(QuitSequence());
                break;

            case LevelData.TurnState.SHOT_SEQUENCE:

                FollowAmmo();
                break;

            default:
                break;
        }
    }

    // A coroutine to wait after the ammo landed
    IEnumerator QuitSequence()
    {
        yield return new WaitForSeconds(_secondsToWaitAfterFollow);

        if (LevelData.CurrentTurnState == LevelData.TurnState.GAME_OVER)
        {
            _gameController.ResetLevel();
        }

        if (LevelData.CurrentTurnState != LevelData.TurnState.SHOT_SEQUENCE)
        {
            _uiController.HideDisplayText();

            QuitFollowSequence(true);
        }
    }


    public void FollowAmmo()
    {
        Camera.main.transform.position      = _ammoToFollow.position + new Vector3(0, 0, -10);
        Camera.main.orthographicSize        = _followZoom;
    }


    public void StartFollowSequence(Transform ammoTransform)
    {
        _isFollowing                        = true;

        _ammoToFollow                       = ammoTransform;

        _lastSavedCameraPosition            = Camera.main.transform.position;
        _lastSavedCameraZoom                = Camera.main.orthographicSize;

        LevelData.CurrentTurnState           = LevelData.TurnState.SHOT_SEQUENCE;
    }


    public void QuitFollowSequence(bool setToStandardPosition)
    {
        if (setToStandardPosition)
        {
            Camera.main.transform.position  = _standardPosition;
            Camera.main.orthographicSize    = _standardZoom;
        }
        else // Only, if the camera is movable. Turned off yet.
        {
            Camera.main.transform.position  = _lastSavedCameraPosition;
            Camera.main.orthographicSize    = _lastSavedCameraZoom;
        }

        LevelData.CurrentTurnState           = LevelData.TurnState.AIMING;
    }



#if UNITY_EDITOR
    public void MoveCamera(Vector3 movement)
    {
        Vector3 newPosition = Camera.main.transform.position + (movement * _scrollSpeed);

        if (newPosition.x >= _minPositionX
          && newPosition.x <= _maxPositionX)
        {
            Camera.main.transform.position = newPosition;
        }
    }


    public void SetZoom(float newZoom)
    {
        float zoom = Camera.main.orthographicSize + (newZoom * _zoomIntensity);

        if (zoom >= _minZoom
          && zoom <= _maxZoom)
        {
            Camera.main.orthographicSize = zoom;

            // Reset the position of the camera based on the zoom
            Camera.main.transform.position = new Vector3(Camera.main.transform.position.x,
                                                         (zoom - _minZoom) * 0.6f,
                                                         -10);
        }
    }

#endif

}
