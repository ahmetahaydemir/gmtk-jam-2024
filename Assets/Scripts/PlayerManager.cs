using StylizedWater2;
using UnityEngine;
public class PlayerManager : MonoBehaviour
{
    public Transform playerTransform;
    public WaterObject waterObject;
    public float movementSpeed = 2f;
    public float sprintSpeed = 2f;
    public float verticalSpeed = 2f;
    public float rotationSpeed = 2f;
    public float additiveFadeSpeed = 2f;
    public Transform playerCamera;
    //
    private Vector3 inputRawDirection;
    private Vector3 _moveInputVector;
    private Vector3 _lookInputVector;
    private Vector3 _additivePosVector;
    private Vector3 _finalPosVector;
    private float attackChargeTimer;
    private bool attackCharging;
    private float attackActionTimer;
    private bool attackActionToken;
    //
    public void UpdatePlayerAction(InputCache inputCache)
    {
        // Input Control
        inputRawDirection.x = inputCache.moveInput.x;
        inputRawDirection.z = inputCache.moveInput.y;
        // Clamp input
        Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputCache.moveInput.x, 0f, inputCache.moveInput.y), 1f);
        // Calculate camera direction and rotation on the character plane
        Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(playerCamera.rotation * Vector3.forward, Vector3.up).normalized;
        if (cameraPlanarDirection.sqrMagnitude == 0f)
        {
            cameraPlanarDirection = Vector3.ProjectOnPlane(playerCamera.rotation * Vector3.up, Vector3.up).normalized;
        }
        Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Vector3.up);
        // Finalize Directed Input
        _moveInputVector = playerCamera.rotation * moveInputVector;
        _lookInputVector = cameraPlanarDirection;
        //
        _finalPosVector = Vector3.zero;
        //
        if (inputCache.sprintInput && playerTransform.position.y < 0)
        {
            _finalPosVector += sprintSpeed * _moveInputVector;
        }
        else
        {
            _finalPosVector += movementSpeed * _moveInputVector;
        }
        //
        if (inputCache.riseInput)
        {
            if (playerTransform.position.y < -0.1)
            {
                _finalPosVector += verticalSpeed * Vector3.up;
            }
            else if (playerTransform.position.y > -0.2 && playerTransform.position.y < 0)
            {
                _additivePosVector += (Vector3.up + _moveInputVector) * verticalSpeed;
            }
        }
        // Gravity
        if (playerTransform.position.y > 0f)
        {
            if (_finalPosVector.y > 0)
            {
                _finalPosVector += 4f * Vector3.down;
            }
            else
            {
                _finalPosVector += 8f * Vector3.down;
            }
        }
        else if (playerTransform.position.y < -5f)
        {
            _finalPosVector += 0.5f * Vector3.down;
        }
        else
        {
            _finalPosVector += 0.5f * Vector3.down;
        }
        //
        if (inputCache.dodgeInput)
        {
            inputCache.dodgeInput = false;
            _additivePosVector += -playerTransform.right * 8f;
        }
        //
        AttackAction(inputCache);
        //
        playerTransform.position += Time.deltaTime * (_finalPosVector + _additivePosVector);
        playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, playerCamera.rotation, rotationSpeed * Time.deltaTime);
        //
        _additivePosVector = Vector3.Slerp(_additivePosVector, Vector3.zero, Time.deltaTime * additiveFadeSpeed);
        Debug.DrawLine(playerTransform.position, playerTransform.position + _lookInputVector * 2f, Color.green);
        Debug.DrawLine(playerTransform.position, playerTransform.position + _moveInputVector * 2.5f, Color.red);
    }
    //
    public void AttackAction(InputCache inputCache)
    {
        if (inputCache.leftClickInput && !attackCharging && !attackActionToken)
        {
            attackChargeTimer = 0f;
            attackCharging = true;
        }
        else
        {
            if (attackCharging)
            {
                attackChargeTimer += Time.deltaTime;
                //
                if (!inputCache.leftClickInput)
                {
                    if (attackChargeTimer > 0.25f)
                    {
                        _additivePosVector += playerTransform.forward * Mathf.Lerp(6f, 18f, attackChargeTimer * 0.5f);
                        attackCharging = false;
                        attackActionToken = true;
                        attackActionTimer = 0f;
                    }
                    else
                    {
                        _additivePosVector += playerTransform.forward * 6f;
                        attackCharging = false;
                        attackActionToken = true;
                        attackActionTimer = 0f;
                    }
                }
            }
        }
        if (attackActionToken)
        {
            attackActionTimer += Time.deltaTime;
            if (attackActionTimer > 0.75f)
            {
                attackActionToken = false;
                attackActionTimer = 0f;
            }
        }
    }
}