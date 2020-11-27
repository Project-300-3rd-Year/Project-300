using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraRotation : MonoBehaviour
{
    //Components.
    [SerializeField] private Transform PlayerBody;
    private PlayerMovement playerMovement;
    private Camera playerCamera;

    [SerializeField] private bool CanRotate;

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

        Cursor.lockState = CursorLockMode.Locked;
        playerCamera = Camera.main;
    }

    void Update()
    {
        if(CanRotate)
            RotateOnPlayerInput();
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

    public void SetRotation(float rotation)
    {
        VerticalRotation = rotation;
       // transform.localRotation = Quaternion.Euler(-rotation, 0f, 0f);
    }

    public void DisableRotation() => CanRotate = false;
    public void EnableRotation()
    {
        CanRotate = true;
        print("hello");
    } 
}
