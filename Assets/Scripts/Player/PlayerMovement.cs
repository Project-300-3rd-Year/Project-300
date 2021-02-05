using System.Collections;
using UnityEngine;
using System.Runtime.InteropServices;


public class PlayerMovement : MonoBehaviour
{
    //Components.
    CharacterController characterController;

    //Properties.
    public float Speed { get { return movementThisFrame.magnitude * currentMovementSpeed * Time.deltaTime; } }
  
    [Header("Movement")]
    [SerializeField] private bool CanMove;
    [SerializeField] private float currentMovementSpeed = 0f;
    private float xMovementInput { get { return Input.GetAxisRaw("Horizontal"); } }
    private float zMovementInput { get { return Input.GetAxisRaw("Vertical"); } }
    private Vector3 movementThisFrame;
    private Vector3 velocity;

    public void DisableMovement() => CanMove = false;
    public void EnableMovement() => CanMove = true;

    [Header("Walking")]
    [SerializeField] private float maxWalkSpeed;

    [Header("Sprinting")]
    public bool IsSprinting;
    [SerializeField] private float sprintSpeed = 24f;
    [SerializeField] private float sprintTimer = 0;
    [SerializeField] private float maxSprintTime = 3;

    [SerializeField] private bool SprintIsInCooldown;
    [SerializeField] private float sprintCooldownTimer = 0;
    [SerializeField] private float sprintCooldownTime = 4;

    [Header("Jumping")]
    [SerializeField] private bool IsJumping;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 5f;

    [Header("Crouching")]
    [SerializeField] private bool IsCrouching;
    [SerializeField] private float normalHeight;
    [SerializeField] private float crouchHeight;
    [SerializeField] private float maxCrouchSpeed = 2;
    private Coroutine crouchCoroutine;

    [Header("Movement Speed Adjustment")]
    [Range(0,1)]
    [SerializeField] private float movementSpeedAdjustSpeed;
    [SerializeField] private float minStandingMovementSpeed;
    [SerializeField] private float minCrouchingMovementSpeed;
    private float MovementSpeedAdjustAmount { get { return Input.mouseScrollDelta.y * movementSpeedAdjustSpeed; } }

    //Getting caps lock state for movement speed adjusting.
    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
    public static extern short GetKeyState(int keyCode); 
    private bool CapsLockIsActive;

    [Header("Ground Checking")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private bool IsGrounded;

    [Header("Leaning")]
    [SerializeField] private bool IsLeaning;
    [SerializeField] private float leanRotateAmount;
    [SerializeField] private float leanMoveAmount;
    [SerializeField] private float leanSpeed;
    private Coroutine LeanCoroutine;
    private Quaternion defaultRotation;
    private Quaternion rotationAtStartOfLean;

    public Transform objectHoldPosition;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {  
        defaultRotation = transform.rotation;

        currentMovementSpeed = maxWalkSpeed;

        normalHeight = characterController.height;
        crouchHeight = characterController.height / 2;

        CapsLockIsActive = (((ushort)GetKeyState(0x14)) & 0xffff) != 0;
    }

    void Update()
    {
        if (CanMove)
        {
            CheckIfPlayerIsGrounded();
            MoveOnPlayerInput();
            JumpOnPlayerInput();
            CrouchOnPlayerInput();
            AdjustMovementSpeedOnPlayerInput();
        }
    }

    private void JumpOnPlayerInput()
    {
        //setting velocity to near zero
        if (IsGrounded && velocity.y < 0)
        {
            IsJumping = false;
            velocity.y = -2f;
        }
        //jumping
        if (Input.GetKeyDown("space") && IsGrounded)
        {
            IsJumping = true;
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            if(IsCrouching)
                StopCrouching();
        }

        //gravity
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
    private void MoveOnPlayerInput()
    {
        //Check if player tries to sprint.
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W) && sprintTimer <= maxSprintTime && !SprintIsInCooldown && !IsCrouching)
        {
            IsSprinting = true;
            currentMovementSpeed = sprintSpeed;
            sprintTimer += Time.deltaTime;
            if (sprintTimer >= maxSprintTime)
            {
                SprintIsInCooldown = true;
                currentMovementSpeed = maxWalkSpeed;
            }
        }
        else
        {
            IsSprinting = false;

            if (SprintIsInCooldown)
            {
                sprintCooldownTimer += Time.deltaTime;
                if (sprintCooldownTimer >= sprintCooldownTime)
                {
                    SprintIsInCooldown = false;
                    sprintCooldownTimer = 0;
                }
            }

            if (sprintTimer > 0)
            {
                sprintTimer -= Time.deltaTime;
                if (sprintTimer <= 0)
                    sprintTimer = 0;
            }
        }

        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            IsSprinting = false;
            if (!IsCrouching)
                currentMovementSpeed = maxWalkSpeed;
        }

        //Moving.
        movementThisFrame = transform.right * xMovementInput + transform.forward * zMovementInput;
        characterController.Move(movementThisFrame * currentMovementSpeed * Time.deltaTime);
    }

    private void CheckIfPlayerIsGrounded() => IsGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    private void CrouchOnPlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.C) && IsGrounded && !IsSprinting)
        {
            if (!IsCrouching)
            {
                Crouch();
            }
            else
            {
                StopCrouching();
            }
        }
    }
    private void Crouch()
    {
        IsCrouching = true;
        currentMovementSpeed = maxCrouchSpeed;

        if (crouchCoroutine != null)
            StopCoroutine(crouchCoroutine);

        crouchCoroutine = StartCoroutine(AdjustHeight(crouchHeight));
    }
    private void StopCrouching()
    {
        currentMovementSpeed = maxWalkSpeed;

        IsCrouching = false;

        if (crouchCoroutine != null)
            StopCoroutine(crouchCoroutine);

        crouchCoroutine = StartCoroutine(AdjustHeight(normalHeight));
    }


    private IEnumerator AdjustHeight(float targetHeight)
    {
        float percentageComplete = 0;

        while (percentageComplete <= 1)
        {
            characterController.height = Mathf.Lerp(characterController.height, targetHeight, percentageComplete);
            percentageComplete += maxCrouchSpeed * Time.deltaTime;
            yield return null;
        }
    }

    private void AdjustMovementSpeedOnPlayerInput()
    {
        if(MovementSpeedAdjustAmount != 0)
        {
            if (!IsSprinting)
            {
                currentMovementSpeed += MovementSpeedAdjustAmount;
                currentMovementSpeed = IsCrouching ? Mathf.Clamp(currentMovementSpeed, minCrouchingMovementSpeed, maxCrouchSpeed) 
                                                   : Mathf.Clamp(currentMovementSpeed, minStandingMovementSpeed, maxWalkSpeed);
            }
        }

        if(Input.GetKeyDown(KeyCode.CapsLock))
        {
            CapsLockIsActive = !CapsLockIsActive;

            if(CapsLockIsActive) //Slow
                currentMovementSpeed = IsCrouching ? minCrouchingMovementSpeed : minStandingMovementSpeed;
            else
                currentMovementSpeed = IsCrouching ? maxCrouchSpeed : maxWalkSpeed;
        }
    }

    //TEMPORARY TEST FOR COLLIDING WITH OTHER RIGIDBODIES - FIX UP LATER.
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //if (hit.rigidbody != null)
        //{
        //    print("added force");
        //    hit.rigidbody.AddForce(hit.moveDirection * 5f);
        //}
    }
}
