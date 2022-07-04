using UnityEngine;

/// <summary>
/// This class represents the script for the camera movement.
/// </summary>
public class CameraMovement : Singleton<CameraMovement>
{
    [SerializeField]
    private float CameraZ = 0;

    [SerializeField]
    private float CameraSpeed = 1.0f;

    [SerializeField]
    private float ZoomSpeed = 1.0f;

    [SerializeField]
    private float ZoomTime = 0.25f;

    [SerializeField]
    private float MaxZoomInFactor = 1.0f;

    [SerializeField]
    private float MaxZoomOutFactor = 1.0f;

    [SerializeField]
    private int MouseButtonMovementTrigger = 0;

    private bool IsRunning;
    private float MinZoom, NormalZoom, MaxZoom, TargetZoom, RemainingZoomTime, InitialTouchZoom, InitialTargetZoomAtTouchStart;
    private float LeftCameraLimit, RightCameraLimit, BottomCameraLimit, TopCameraLimit;

    private Vector3 OuterTopLeftPosition, OuterBottomRightPosition, InnerTopLeftPosition, InnerBottomRightPosition;
    private Vector3 MouseMovementStartPoint;

    private bool Zooming, TouchZooming;
    private Vector3 ZoomingMousePosition;

    public Vector3 CurrentPosition => this.transform.position;

    private Vector3 _PanStartPosition;
    public Vector3 PanStartPosition => _PanStartPosition;

    /// <summary>
    /// This method is called before the first frame update.
    /// </summary>
    private void Start()
    {
        Debug.Log(string.Format("Class '{0}', Method '{1}': {2}", nameof(CameraMovement), nameof(Start), "Entering method..."));
    }

    /// <summary>
    /// This method is called once per frame.
    /// </summary>
    private void Update()
    {
        if (IsRunning)
        {
            ZoomCamera();
            MoveCamera();
            LimitCamera();
        }
    }

    /// <summary>
    /// Zooms the camera.
    /// </summary>
    private void ZoomCamera()
    {
        if (Input.GetKeyDown(KeyCode.Mouse2) && Input.touchCount == 0)
        {
            // if scrollwheel clicked => reset zoom and camera position
            ResetCameraZoomAndPosition();
            this.Zooming = false;
            this.TouchZooming = false;
            return;
        }

        if (!GameManager.GetInstance().IsRotatingSelection)
        {
            // if scrollwheel is scrolled instead => zoom in or out
            float updatedTargetZoom = TargetZoom - Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed;
            updatedTargetZoom = Mathf.Clamp(updatedTargetZoom, MinZoom, MaxZoom);
            if (!Mathf.Approximately(updatedTargetZoom, TargetZoom))
            {
                TargetZoom = updatedTargetZoom;
                RemainingZoomTime = ZoomTime;

                // Zoom to mouse-position instead of center
                this.Zooming = true;
                this.MouseMovementStartPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                this.ZoomingMousePosition = Input.mousePosition;
            }
            else
            {
                // No Zooming needed
                this.Zooming = false;
            }
        }

        if (RemainingZoomTime > Time.unscaledDeltaTime)
        {
            RemainingZoomTime -= Time.unscaledDeltaTime;
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, TargetZoom, 1 - RemainingZoomTime / ZoomTime);

            // Still zooming to mouse-position
            this.Zooming = true;
        }
        else
        {
            Camera.main.orthographicSize = TargetZoom;
            RemainingZoomTime = 0;

            // Zooming has finished
            this.Zooming = false;
        }

        // If device has touch support
        if (Input.touchCount == 2)
        {
            var touch0 = Input.GetTouch(0).position;
            var touch1 = Input.GetTouch(1).position;
            float distance = Vector2.Distance(touch0, touch1);

            if (this.TouchZooming)
            {
                Camera.main.orthographicSize = this.TargetZoom = this.InitialTargetZoomAtTouchStart * (this.InitialTouchZoom / distance);
            }
            else
            {
                this.TouchZooming = true;
                this.InitialTouchZoom = distance;
                this.InitialTargetZoomAtTouchStart = this.TargetZoom;
            }
        }
        else
        {
            this.TouchZooming = false;
        }
    }

    /// <summary>
    /// Resets the zoom and the position of the main camera back to the center.
    /// </summary>
    private void ResetCameraZoomAndPosition()
    {
        Camera.main.orthographicSize = TargetZoom = NormalZoom;
        Camera.main.transform.position = new Vector3
        {
            x = InnerTopLeftPosition.x + (NormalZoom * Screen.width) / Screen.height,
            y = InnerTopLeftPosition.y - NormalZoom,
            z = CameraZ,
        };
    }

    /// <summary>
    /// Moves the camera.
    /// </summary>
    private void MoveCamera()
    {
        // movement with mouse
        if (Input.GetMouseButtonDown(MouseButtonMovementTrigger))
        {
            this.MouseMovementStartPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            this._PanStartPosition = this.transform.position;
        }

        if (Input.GetMouseButton(MouseButtonMovementTrigger))
        {
            Camera.main.transform.position += MouseMovementStartPoint - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (this.Zooming)  // We dont want to update zoom-target-point to mouse-position. Instead we want to zoom to original mouse-position
        {
            Camera.main.transform.position += MouseMovementStartPoint - Camera.main.ScreenToWorldPoint(ZoomingMousePosition);
        }

        // movement with keyboard arrow keys
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Camera.main.transform.Translate(Vector3.up * CameraSpeed * Time.unscaledDeltaTime);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Camera.main.transform.Translate(Vector3.left * CameraSpeed * Time.unscaledDeltaTime);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            Camera.main.transform.Translate(Vector3.down * CameraSpeed * Time.unscaledDeltaTime);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            Camera.main.transform.Translate(Vector3.right * CameraSpeed * Time.unscaledDeltaTime);
        }
    }

    /// <summary>
    /// Limits the camera to calculated boundaries.
    /// Should be called last in the Update() method.
    /// </summary>
    private void LimitCamera()
    {
        LeftCameraLimit = OuterTopLeftPosition.x + Camera.main.orthographicSize * Camera.main.aspect;
        TopCameraLimit = OuterTopLeftPosition.y - Camera.main.orthographicSize;
        RightCameraLimit = OuterBottomRightPosition.x - Camera.main.orthographicSize * Camera.main.aspect;
        BottomCameraLimit = OuterBottomRightPosition.y + Camera.main.orthographicSize;
        Camera.main.transform.position = new Vector3
        {
            x = Mathf.Clamp(transform.position.x, LeftCameraLimit < RightCameraLimit ? LeftCameraLimit : RightCameraLimit, LeftCameraLimit > RightCameraLimit ? LeftCameraLimit : RightCameraLimit),
            y = Mathf.Clamp(transform.position.y, TopCameraLimit < BottomCameraLimit ? TopCameraLimit : BottomCameraLimit, TopCameraLimit > BottomCameraLimit ? TopCameraLimit : BottomCameraLimit),
            z = CameraZ
        };
    }

    /// <summary>
    /// This method must be called after building the level to pass the top left and bottom right corner of the map.
    /// These values are used to determine the boundaries for the camera movement and zooming.
    /// </summary>
    /// <param name="outerTopLeftPosition">A 3D vector determining the top left corner of the map including the border.</param>
    /// <param name="outerBottomRightPosition">A 3D vector determining the bottom right corner of the map including the border.</param>
    /// <param name="innerTopLeftPosition">A 3D vector determining the top left corner of the map excluding the border.</param>
    /// <param name="innerBottomRightPosition">A 3D vector determining the bottom right corner of the map excluding the border.</param>
    public void SetCameraLimits(Vector3 outerTopLeftPosition, Vector3 outerBottomRightPosition, Vector3 innerTopLeftPosition, Vector3 innerBottomRightPosition)
    {
        OuterTopLeftPosition = outerTopLeftPosition;
        OuterBottomRightPosition = outerBottomRightPosition;
        InnerTopLeftPosition = innerTopLeftPosition;
        InnerBottomRightPosition = innerBottomRightPosition;

        // set zoom based on map size
        float neededCameraSizeforWidth = (InnerBottomRightPosition.x - InnerTopLeftPosition.x) / (Camera.main.aspect * 2); // horizontal width
        float neededCameraSizeforHeigth = (InnerTopLeftPosition.y - InnerBottomRightPosition.y) / 2; // vertical height
        NormalZoom = TargetZoom = neededCameraSizeforWidth > neededCameraSizeforHeigth ? neededCameraSizeforWidth : neededCameraSizeforHeigth; // choose the greater one
        MinZoom = NormalZoom / MaxZoomInFactor;
        MaxZoom = NormalZoom * MaxZoomOutFactor;
        ResetCameraZoomAndPosition();
    }

    /// <summary>
    /// Sets a flag determining whether the Update() method is enabled or disabled.
    /// </summary>
    /// <param name="isRunning"></param>
    public void SetIsRunningFlag(bool isRunning)
    {
        IsRunning = isRunning;
    }

    /// <summary>
    /// This method draws some lines for debugging purposes, which are only visible in the debugger.
    /// </summary>
    private void OnDrawGizmos()
    {
        // visualize camera pivot boundaries
        Gizmos.color = Color.black;
        Gizmos.DrawLine(new Vector2(LeftCameraLimit, TopCameraLimit), new Vector2(RightCameraLimit, TopCameraLimit));
        Gizmos.DrawLine(new Vector2(RightCameraLimit, TopCameraLimit), new Vector2(RightCameraLimit, BottomCameraLimit));
        Gizmos.color = Color.white;
        Gizmos.DrawLine(new Vector2(RightCameraLimit, BottomCameraLimit), new Vector2(LeftCameraLimit, BottomCameraLimit));
        Gizmos.DrawLine(new Vector2(LeftCameraLimit, BottomCameraLimit), new Vector2(LeftCameraLimit, TopCameraLimit));

        // visualize special vectors
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(OuterTopLeftPosition, 0.2f);
        Gizmos.DrawSphere(OuterBottomRightPosition, 0.2f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(InnerTopLeftPosition, 0.2f);
        Gizmos.DrawSphere(InnerBottomRightPosition, 0.2f);
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.2f);
    }
}
