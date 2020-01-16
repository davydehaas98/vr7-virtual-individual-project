using UnityEngine;
using Valve.VR;

public class VRController : MonoBehaviour
{
    public float Gravity = 30.0f;
    public float Sensitivity = 0.1f;
    public float MaxSpeed = 1.0f;
    public float RotateIncrement = 90;

    public SteamVR_Action_Boolean RotatePress = null;
    public SteamVR_Action_Boolean MovePress = null;
    public SteamVR_Action_Vector2 MoveValue = null;

    private float Speed = 0.0f;

    private CharacterController CharacterController = null;
    private Transform CameraRig = null;
    private Transform Head = null;

    private void Awake()
    {
        CharacterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        CameraRig = SteamVR_Render.Top().origin;
        Head = SteamVR_Render.Top().head;
    }
    private void FixedUpdate()
    {
        //HandleHead();
        HandleHeight();
        CalculateMovement();
        //SnapRotation();
    }

    private void HandleHeight()
    {
        // Get the Head in local space
        float headHeight = Mathf.Clamp(Head.localPosition.y, 1, 2);
        CharacterController.height = headHeight;

        // Cut in half
        Vector3 newCenter = Vector3.zero;
        newCenter.y = CharacterController.height / 2;
        newCenter.y += CharacterController.skinWidth;

        // Move Capsule in local space
        newCenter.x = Head.localPosition.x;
        newCenter.z = Head.localPosition.z;

        // Rotate
        //newCenter = Quaternion.Euler(0, -transform.eulerAngles.y, 0) * newCenter;

        // Apply
        CharacterController.center = newCenter;
    }

    private void CalculateMovement()
    {
        // Get Movement orientation
        Quaternion orientation = CalculateOrientation();

        Vector3 movement = Vector3.zero;

        // If not moving
        if (MoveValue.axis.magnitude == 0)
        {
            Speed = 0;
        }
        print(MoveValue.axis.magnitude);
        // Add Speed
        Speed += MoveValue.axis.magnitude * Sensitivity;

        // Clamp the Speed within the MaxSpeed and -MaxSpeed values
        Speed = Mathf.Clamp(Speed, -MaxSpeed, MaxSpeed);

        // Orientation
        movement += orientation * (Speed * Vector3.forward);

        // Gravity
        movement.y -= Gravity * Time.deltaTime;

        CharacterController.Move(movement);
    }

    private Quaternion CalculateOrientation()
    {
        float rotation = Mathf.Atan2(MoveValue.axis.x, MoveValue.axis.y);
        rotation *= Mathf.Rad2Deg;

        Vector3 orientationEuler = new Vector3(0, Head.eulerAngles.y + rotation, 0);
        return Quaternion.Euler(orientationEuler);
    }

    private void HandleHead()
    {
        // Save the current position and rotation of the Head
        Vector3 oldPosition = CameraRig.position;
        Quaternion oldRotation = CameraRig.rotation;

        // Rotation
        transform.eulerAngles = new Vector3(0.0f, Head.rotation.eulerAngles.y, 0.0f);

        // Restore the position and rotation of the Head
        CameraRig.position = oldPosition;
        CameraRig.rotation = oldRotation;
    }

    private void SnapRotation()
    {
        float snapValue = 0.0f;

        if (RotatePress.GetStateDown(SteamVR_Input_Sources.LeftHand))
        {
            // Make SnapValue always negative
            snapValue = -Mathf.Abs(RotateIncrement);
        }

        if (RotatePress.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            // Make SnapValue always positive
            snapValue = Mathf.Abs(RotateIncrement);
        }

        transform.RotateAround(Head.position, Vector3.up, snapValue);
    }
}
