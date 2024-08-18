using DG.Tweening;
using UnityEngine;
public class CameraManager : MonoBehaviour
{
    public Vector2 frameOffset;
    public float distanceMin;
    public float distanceMax;
    public float angleMax;
    public float angleMin;
    public float rotationSpeed;
    public float DistanceMovementSharpness = 10f;
    public float FollowingSharpness = 10000f;
    public float RotationSharpness = 10000f;
    public float DistanceMovementSpeed = 5f;
    //
    private float _currentDistance;
    private float _targetVerticalAngle;
    private Vector3 _currentFollowPosition;
    private Quaternion rotationFromInput;
    private Quaternion planarRot;
    private Quaternion verticalRot;
    private Quaternion targetRotation;
    private Vector3 targetPosition;
    public Vector3 PlanarDirection { get; set; }
    public float TargetDistance { get; set; }
    public Transform PlayerTransform { get; set; }
    public Transform CameraTransform { get; set; }
    public bool HorizontalSway;
    public bool VerticalSway;
    //
    public void Initialize(Transform targetTransform, Transform cameraTransform)
    {
        PlayerTransform = targetTransform;
        CameraTransform = cameraTransform;
        _currentDistance = distanceMax;
        TargetDistance = _currentDistance;
        _targetVerticalAngle = 0f;
        PlanarDirection = Vector3.forward;
        rotationFromInput = Quaternion.identity;
        PlanarDirection = PlayerTransform.forward;
        _currentFollowPosition = PlayerTransform.position;
    }
    //
    public void UpdateCameraAction(InputCache inputCache, float waterBaseLevel, float totalMass)
    {
        distanceMax = 5f + totalMass * 0.175f;

        // Process Rotation - Direction
        if (VerticalSway)
        {
            rotationFromInput = Quaternion.Euler(PlayerTransform.up * (inputCache.lookInput.x * rotationSpeed));
            PlanarDirection = rotationFromInput * PlanarDirection;
            PlanarDirection = Vector3.Cross(PlayerTransform.up, Vector3.Cross(PlanarDirection, PlayerTransform.up));
        }
        else
        {
            rotationFromInput = Quaternion.Euler(Vector3.up * (inputCache.lookInput.x * rotationSpeed));
            PlanarDirection = rotationFromInput * PlanarDirection;
        }
        // Process Rotation - Rotation
        if (HorizontalSway)
        {
            planarRot = Quaternion.LookRotation(PlanarDirection, PlayerTransform.up);
        }
        else
        {
            planarRot = Quaternion.LookRotation(PlanarDirection, Vector3.up);
        }

        _targetVerticalAngle -= inputCache.lookInput.y * rotationSpeed;
        _targetVerticalAngle = Mathf.Clamp(_targetVerticalAngle, angleMin, angleMax);
        verticalRot = Quaternion.Euler(_targetVerticalAngle, 0, 0);
        targetRotation = Quaternion.Slerp(CameraTransform.rotation, planarRot * verticalRot, 1f - Mathf.Exp(-RotationSharpness * Time.deltaTime));

        // Apply rotation
        CameraTransform.rotation = targetRotation;

        // Process distance input
        TargetDistance += -inputCache.zoomInput * DistanceMovementSpeed;
        TargetDistance = Mathf.Clamp(TargetDistance, distanceMin, distanceMax);

        // Find the smoothed follow position
        _currentFollowPosition = Vector3.Lerp(_currentFollowPosition, PlayerTransform.position, 1f - Mathf.Exp(-FollowingSharpness * Time.deltaTime));

        // Zoom Distance With No Obstruction
        _currentDistance = Mathf.Lerp(_currentDistance, TargetDistance, 1 - Mathf.Exp(-DistanceMovementSharpness * Time.deltaTime));

        // Find the smoothed camera orbit position
        targetPosition = _currentFollowPosition - (targetRotation * Vector3.forward * _currentDistance);

        // Handle framing
        targetPosition += CameraTransform.right * frameOffset.x;
        targetPosition += CameraTransform.up * frameOffset.y;

        // Raise Above Ground
        if (targetPosition.y < waterBaseLevel)
        {
            targetPosition += (_currentFollowPosition - targetPosition) * (0.1f + ((targetPosition.y - waterBaseLevel) / (targetPosition.y - _currentFollowPosition.y)));
        }
        // Dip Below Surface
        // if (targetPosition.y > 1)
        // {
        //     targetPosition += (_currentFollowPosition - targetPosition) * (0.1f + ((targetPosition.y) / (targetPosition.y - _currentFollowPosition.y)));
        // }

        // Apply position
        CameraTransform.position = targetPosition;
    }
    //
    public void ShakeCamera()
    {
        CameraTransform.DOPunchRotation(Vector3.right * 5f, 0.75f, 6);
    }
}