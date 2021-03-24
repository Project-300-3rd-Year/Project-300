using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraRotation : MonoBehaviour
{
    ////Components.
    //[SerializeField] private Transform PlayerBody;
    //private PlayerMovement playerMovement;
    //private Camera playerCamera;

    //[Header("Status")]
    //[SerializeField] private bool CanRotate;

    //private float horizontalRotationInput { get { return Input.GetAxisRaw("Mouse X"); } }
    //private float verticalRotationInput { get { return Input.GetAxisRaw("Mouse Y"); } set { } }

    //public float currentHorizontalRotationSpeed { get { return Mathf.Abs(horizontalRotationInput * Settings.cameraSensitivity); } }
    //public float currentVerticalRotationSpeed { get { return Mathf.Abs(verticalRotationInput * Settings.cameraSensitivity); } }

    //[Header("Looking Up and Down")]
    //[SerializeField] private float VerticalRotation = 0;
    //[SerializeField] private float MinXAxisClampValue = -90;
    //[SerializeField] private float MaxXAxisClampValue = 90;

    ////Start.
    //private void Awake() => playerMovement = GetComponentInParent<PlayerMovement>();
    //void Start()
    //{
    //    Cursor.lockState = CursorLockMode.Locked;
    //    playerCamera = Camera.main;

    //    GameManager.Instance.onGameEnd += DisableRotation;
    //}

    //void Update()
    //{
    //    if (Time.timeScale == 0)
    //        return;

    //    if (CanRotate)
    //        RotateOnPlayerInput();
    //}

    //private void RotateOnPlayerInput()
    //{
    //    //Get rotation amount for looking down and up and clamp it.
    //    VerticalRotation += verticalRotationInput * Settings.cameraSensitivity;
    //    ClampVerticalRotation();

    //    //Rotate player camera and player.
    //    transform.localRotation = Quaternion.Euler(-VerticalRotation, 0f, 0f);
    //    PlayerBody.Rotate(Vector3.up * horizontalRotationInput * Settings.cameraSensitivity);
    //}
    //private void ClampVerticalRotation()
    //{
    //    VerticalRotation = Mathf.Clamp(VerticalRotation, MinXAxisClampValue, MaxXAxisClampValue);

    //    if (VerticalRotation >= MaxXAxisClampValue || VerticalRotation <= MinXAxisClampValue)
    //        verticalRotationInput = 0;
    //}

    //public void SetRotation(float rotation) => VerticalRotation = rotation;
    //public void DisableRotation() => CanRotate = false;
    //public void EnableRotation() => CanRotate = true;



    //Components.
    [SerializeField] private Transform PlayerBody;
    [SerializeField] private float tiltAngle = -10f;
    [SerializeField] private float tiltSpeed = 10f;

    [Header("Status")]
    [SerializeField] private bool CanRotate;

    private Camera playerCamera;

    private float horizontalRotationInput { get { return Input.GetAxisRaw("Mouse X"); } }
    private float verticalRotationInput { get { return Input.GetAxisRaw("Mouse Y"); } set { } }

    public float currentHorizontalRotationSpeed { get { return Mathf.Abs(horizontalRotationInput * Settings.cameraSensitivity); } }
    public float currentVerticalRotationSpeed { get { return Mathf.Abs(verticalRotationInput * Settings.cameraSensitivity); } }

    [Header("Looking Up and Down")]
    [SerializeField] private float VerticalRotation = 0;
    [SerializeField] private float MinXAxisClampValue = -90;
    [SerializeField] private float MaxXAxisClampValue = 90;
    private void Awake() { }

    void Start()
    {
        playerCamera = GetComponent<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        playerCamera = Camera.main;

        GameManager.Instance.onGameEnd += DisableRotation;
    }

    void Update()
    {
        if (Time.timeScale == 0)
            return;

        if (CanRotate)
            RotateOnPlayerInput();

    }

    private void RotateOnPlayerInput()
    {
        //Get rotation amount for looking down and up and clamp it.
        VerticalRotation += verticalRotationInput * Settings.cameraSensitivity;
        ClampVerticalRotation();


        float leanThrow = Input.GetAxis("Lean");
        float angleDueToControl = leanThrow * tiltSpeed;
        Ray ray = new Ray(PlayerBody.position, transform.TransformDirection(Vector3.right * leanThrow));
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, 1.5f))
            transform.localRotation = Quaternion.Euler(-VerticalRotation, 0f, 0f);
        else
        {
            angleDueToControl = Mathf.Clamp(angleDueToControl, tiltAngle, -tiltAngle);
            transform.localRotation = Quaternion.Euler(-VerticalRotation, 0f, -angleDueToControl);
        }

        // Rotate player camera and player.
        transform.localRotation = Quaternion.Euler(-VerticalRotation, 0f, -angleDueToControl);
        PlayerBody.Rotate(Vector3.up * horizontalRotationInput * Settings.cameraSensitivity);
    }
    private void ClampVerticalRotation()
    {
        VerticalRotation = Mathf.Clamp(VerticalRotation, MinXAxisClampValue, MaxXAxisClampValue);

        if (VerticalRotation >= MaxXAxisClampValue || VerticalRotation <= MinXAxisClampValue)
            verticalRotationInput = 0;
    }

    public void SetRotation(float rotation) => VerticalRotation = rotation;
    public void DisableRotation() => CanRotate = false;
    public void EnableRotation() => CanRotate = true;
}

