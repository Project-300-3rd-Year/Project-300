using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraRotation : MonoBehaviour
{

    //Components.
    [SerializeField] private Transform PlayerBody;
    private PlayerMovement playerMovement;

    private Camera playerCamera;
    private Vector3 cameraOrigin;
    float headBobCounter;
    float xIntensity;
    float yIntensity;

    private float horizontalRotationInput { get { return Input.GetAxisRaw("Mouse X"); } }
    private float verticalRotationInput { get { return Input.GetAxisRaw("Mouse Y"); } set { } }

    [Header("Rotation Speed")]
    [SerializeField] private float mouseSensitivity;
    private float defaultMouseSensitivity;
    public float currentHorizontalRotationSpeed { get { return Mathf.Abs(horizontalRotationInput * mouseSensitivity); } }
    public float currentVerticalRotationSpeed { get { return Mathf.Abs(verticalRotationInput * mouseSensitivity); } }

    [Header("Looking Up and Down")]
    [SerializeField] private float VerticalRotation = 0;
    [SerializeField] private float MinXAxisClampValue = -90;
    [SerializeField] private float MaxXAxisClampValue = 90;

    private void Awake()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    void Start()
    {
        defaultMouseSensitivity = mouseSensitivity;

        playerCamera = GetComponent<Camera>();
        cameraOrigin = playerCamera.transform.localPosition; 

        Cursor.lockState = CursorLockMode.Locked;
        playerCamera = Camera.main;
        cameraOrigin = playerCamera.transform.localPosition;
    }

    void Update()
    {
        headBobCounter += Time.deltaTime;
        print(playerCamera.name);
        RotateOnPlayerInput();
        HeadBob(headBobCounter, xIntensity, yIntensity);
    }

    private void RotateOnPlayerInput()
    {
        //Get rotation amount for looking down and up and clamp it.
        VerticalRotation += verticalRotationInput * mouseSensitivity;
        ClampVerticalRotation();

        //Rotate player camera and player.
        transform.localRotation = Quaternion.Euler(-VerticalRotation, 0f, 0f);
        PlayerBody.Rotate(Vector3.up * horizontalRotationInput * mouseSensitivity);
    }
    private void ClampVerticalRotation()
    {
        VerticalRotation = Mathf.Clamp(VerticalRotation, MinXAxisClampValue, MaxXAxisClampValue);

        if (VerticalRotation >= MaxXAxisClampValue || VerticalRotation <= MinXAxisClampValue)
            verticalRotationInput = 0;
    }

    public void DisableRotation() => mouseSensitivity = 0;
    public void EnableRotation() => mouseSensitivity = defaultMouseSensitivity;

    public void HeadBob(float p_z, float p_x_intensity, float p_y_intensity)
    {
        playerCamera.transform.localPosition = new Vector3(Mathf.Cos(p_z * 6) * p_x_intensity, Mathf.Sin(p_z * 2) * p_y_intensity, 0);
    }

    public void SetBobbingIntensity(float x, float y)
    {
        xIntensity = x;
        yIntensity = y;
    }
}
