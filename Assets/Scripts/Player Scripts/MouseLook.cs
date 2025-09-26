using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private Transform playerRoot, lookRoot;
    [SerializeField] private bool invert;
    [SerializeField] private bool can_Unlock = true;
    [SerializeField] private float sensivity = 5f;
    [SerializeField] private Vector2 default_Look_Limits = new Vector2(-70f, 80f);

    // Mobil joystick (kamera)
    public FixedJoystick lookJoystick;

    private Vector2 look_Angles;

    void Start()
    {
#if UNITY_ANDROID || UNITY_IOS
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
#else
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
#endif
    }

    void Update()
    {
        LockAndUnlockCursor();
        LookAround();
    }

    void LockAndUnlockCursor()
    {
#if !UNITY_ANDROID && !UNITY_IOS
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
#endif
    }

    void LookAround()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (lookJoystick == null) return;

        float h = lookJoystick.Horizontal * sensivity;
        float v = lookJoystick.Vertical * sensivity;

        look_Angles.y += h;
        look_Angles.x += (invert ? v : -v);
        look_Angles.x = Mathf.Clamp(look_Angles.x, default_Look_Limits.x, default_Look_Limits.y);

        lookRoot.localRotation = Quaternion.Euler(look_Angles.x, 0f, 0f);
        playerRoot.localRotation = Quaternion.Euler(0f, look_Angles.y, 0f);
#else
        float mouseX = Input.GetAxis("Mouse X") * sensivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensivity * (invert ? 1f : -1f);

        look_Angles.y += mouseX;
        look_Angles.x += mouseY;
        look_Angles.x = Mathf.Clamp(look_Angles.x, default_Look_Limits.x, default_Look_Limits.y);

        lookRoot.localRotation = Quaternion.Euler(look_Angles.x, 0f, 0f);
        playerRoot.localRotation = Quaternion.Euler(0f, look_Angles.y, 0f);
#endif
    }
}
