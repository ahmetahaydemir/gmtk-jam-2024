using StylizedWater2;
using UnityEngine;
public class PlayerManager : MonoBehaviour
{
    public Transform playerTransform;
    public WaterObject waterObject;
    public float movementSpeed = 2f;
    public float verticalSpeed = 2f;
    public float rotationSpeed = 2f;
    public Transform playerCamera;
    //
    private Vector3 inputRawDirection;
    private Vector3 _moveInputVector;
    private Vector3 _verticalInputVector;
    private Vector3 _lookInputVector;
    private Vector3 buoyancyNormal;
    //
    public void UpdatePlayerAction(InputCache inputCache)
    {
        // Input Control
        inputRawDirection.x = inputCache.moveInput.x;
        inputRawDirection.z = inputCache.moveInput.y;
        //
        // Clamp input
        Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputCache.moveInput.x, 0f, inputCache.moveInput.y), 1f);

        // Calculate camera direction and rotation on the character plane
        Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(playerCamera.rotation * Vector3.forward, Vector3.up).normalized;
        if (cameraPlanarDirection.sqrMagnitude == 0f)
        {
            cameraPlanarDirection = Vector3.ProjectOnPlane(playerCamera.rotation * Vector3.up, Vector3.up).normalized;
        }
        Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Vector3.up);
        _moveInputVector = cameraPlanarRotation * moveInputVector;
        _lookInputVector = cameraPlanarDirection;
        //
        buoyancyNormal = Vector3.up;
        Buoyancy.SampleWaves(playerTransform.position, waterObject.material, 0f, 0.1f, false, out buoyancyNormal);
        //
        _verticalInputVector = Vector3.zero;
        if (inputCache.riseInput)
        {
            _verticalInputVector = verticalSpeed * Vector3.up;
        }
        if (inputCache.diveInput)
        {
            _verticalInputVector = verticalSpeed * -Vector3.up;
        }
        //
        playerTransform.position += movementSpeed * Time.deltaTime * (_moveInputVector + _verticalInputVector);
        playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, cameraPlanarRotation, rotationSpeed * Time.deltaTime);
        //
        Debug.DrawLine(playerTransform.position, playerTransform.position + _lookInputVector * 2f, Color.green);
        Debug.DrawLine(playerTransform.position, playerTransform.position + _moveInputVector * 2.4f, Color.red);
        Debug.DrawLine(playerTransform.position, playerTransform.position + _verticalInputVector * 2.75f, Color.cyan);
        Debug.DrawLine(playerTransform.position, playerTransform.position + buoyancyNormal * 3f, Color.blue);
    }
}