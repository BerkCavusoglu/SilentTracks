using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement: MonoBehaviour
{
    [Header("Player & Camera")]
    public Transform playerRoot;
    public Transform lookRoot;

    [Header("Joystick")]
    public FixedJoystick lookJoystick;   // Hareket
    public DynamicJoystick moveJoystick; // Bakış

    [Header("Movement Settings")]
    public float speed = 5f;
    public float sprintSpeed = 8f;
    public float jumpForce = 10f;
    public float gravity = 50f;
    public bool invertLook = false;

    [Header("Crouch Settings")]
    public float crouchSpeed = 2f;
    public float standHeight = 1.6f;
    public float crouchHeight = 1f;

    [Header("Look Settings")]
    public float sensitivity = 5f;
    public Vector2 lookLimits = new Vector2(-70f, 80f);

    private CharacterController characterController;
    private Vector3 moveDirection;
    private float verticalVelocity;
    private Vector2 lookAngles;
    [HideInInspector] public bool isSprinting = false;
    [HideInInspector] public bool isCrouching = false;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleLook();
        HandleMovement();
    }

    // Kamera kontrolü
    void HandleLook()
    {
        if (lookJoystick == null) return;

        float h = lookJoystick.Horizontal * sensitivity;
        float v = lookJoystick.Vertical * sensitivity;

        lookAngles.y += h;
        lookAngles.x += (invertLook ? v : -v);
        lookAngles.x = Mathf.Clamp(lookAngles.x, lookLimits.x, lookLimits.y);

        lookRoot.localRotation = Quaternion.Euler(lookAngles.x, 0f, 0f);
        playerRoot.localRotation = Quaternion.Euler(0f, lookAngles.y, 0f);
    }

    // Hareket kontrolü
    void HandleMovement()
    {
        float h = moveJoystick.Horizontal;
        float v = moveJoystick.Vertical;

        float currentSpeed = isSprinting ? sprintSpeed : (isCrouching ? crouchSpeed : speed);

        Vector3 move = playerRoot.forward * v + playerRoot.right * h;
        move.y = 0f;
        move = move.normalized * currentSpeed;

        ApplyGravity();
        characterController.Move((move + Vector3.up * verticalVelocity) * Time.deltaTime);

        if (move.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(move), 0.2f);
        }
    }

    //void ApplyGravity()
    //{
    //    if (characterController.isGrounded)
    //    {
    //        verticalVelocity = -1f;
    //    }
    //    else
    //    {
    //        verticalVelocity -= gravity * Time.deltaTime;
    //    }
    //}
    void ApplyGravity()
    {
        if (characterController.isGrounded)
        {
            // Yerdeyse ve yukarı zıplama tuşuna basılmadıysa
            if (verticalVelocity < 0f)
                verticalVelocity = -1f;
        }
        else
        {
            verticalVelocity -= gravity * Time.deltaTime;
        }
    }

    // UI Butonları için
    public void JumpButton()
    {
        if (characterController.isGrounded)
        {
            verticalVelocity = jumpForce;
        }
    }

    public void ToggleCrouch()
    {
        if (isCrouching)
        {
            lookRoot.localPosition = new Vector3(0f, standHeight, 0f);
            isCrouching = false;
        }
        else
        {
            lookRoot.localPosition = new Vector3(0f, crouchHeight, 0f);
            isCrouching = true;
        }
    }

    public void StartSprint() => isSprinting = true;
    public void StopSprint() => isSprinting = false;
}
