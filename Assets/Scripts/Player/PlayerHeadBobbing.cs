using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeadBobbing : MonoBehaviour
{
    PlayerMovement playerMovement;

    [SerializeField] [Tooltip("Bobbing distance up and down")] float movementFloatY = 0.05f;
    [SerializeField] [Tooltip("Bobbing distance left and right")] float movementFloatX = 0.05f;
    [SerializeField] Transform player;
    [SerializeField] float leanSpeed = 10f;
    [SerializeField] float leanDistance = 1f;
    [SerializeField] float BobbingDuration = 2f;
    [Range(0, 1)] float movementFactor;
    [SerializeField] private float tiltAngle = -10f;
    private Vector3 cameraOrigin;
    float posDueToControl;

    void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();

        cameraOrigin = transform.localPosition;
    }

    void Update()
    {
        float cycles = Time.time / BobbingDuration;
        const float tau = Mathf.PI * 2f;
        float rawSineWave = Mathf.Sin(cycles * tau);
        float rawCosWave = Mathf.Cos(cycles * tau);

        movementFactor = rawSineWave / 2f + 0.5f;
        float offsetY = movementFloatY * playerMovement.Speed * movementFactor;

        movementFactor = rawCosWave / 2f + 0.5f;
        float offsetX = movementFloatX * playerMovement.Speed * movementFactor;

        transform.localPosition = new Vector3(
            offsetX + posDueToControl,
            cameraOrigin.y + offsetY,
            transform.localPosition.z);
    }
}
