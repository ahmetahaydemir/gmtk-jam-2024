using System;
using DG.Tweening;
using UnityEngine;
public class PlayerManager : MonoBehaviour
{
    public Transform playerTransform;
    public float movementSpeed = 2f;
    public float sprintSpeed = 2f;
    public float verticalSpeed = 2f;
    public float rotationSpeed = 2f;
    public float additiveFadeSpeed = 2f;
    public Transform playerCamera;
    public Transform playerHead;
    public LayerMask enemyLayer;
    public AudioSource attackChargeSFX;
    public AudioSource attackActionSFX;
    public GameObject[] getHitVFXArray;
    public AudioSource getHitSFX;
    public GameObject deathVFX;
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
    private RaycastHit[] attackActionHit;
    private int getHitIndex;
    //
    void Awake()
    {
        playerHead.DOShakeRotation(3.5f, 8, 3).SetLoops(-1, LoopType.Yoyo);
    }
    //
    public void UpdatePlayerAction(InputCache inputCache, float waterBaseLevel, float totalMass)
    {
        playerTransform.localScale = Vector3.one * (0.5f + totalMass * 0.05f);

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
        InputMove(inputCache);
        GravityMove(waterBaseLevel);
        //
        RiseAction(inputCache);
        DodgeAction(inputCache);
        AttackAction(inputCache);
        //
        BoundaryMove(waterBaseLevel);
        //
        playerTransform.position += Time.deltaTime * (_finalPosVector + _additivePosVector);
        playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, playerCamera.rotation, rotationSpeed * Time.deltaTime);
        //
        _additivePosVector = Vector3.Slerp(_additivePosVector, Vector3.zero, Time.deltaTime * additiveFadeSpeed);
        Debug.DrawLine(playerTransform.position, playerTransform.position + _lookInputVector * 2f, Color.green);
        Debug.DrawLine(playerTransform.position, playerTransform.position + _moveInputVector * 2.5f, Color.red);
    }
    //
    public void GetHitReaction(EnemyData enemyData)
    {
        getHitVFXArray[getHitIndex].SetActive(false);
        getHitVFXArray[getHitIndex].SetActive(true);
        getHitIndex++;
        getHitIndex %= getHitVFXArray.Length;
        //
        getHitSFX.pitch = UnityEngine.Random.Range(0.75f, 1f);
        getHitSFX.volume = UnityEngine.Random.Range(0.75f, 1f);
        getHitSFX.Play();
    }
    public void DeathReaction(float waterBaseLevel)
    {
        deathVFX.SetActive(false);
        deathVFX.SetActive(true);
        //
        playerTransform.DOShakeRotation(3f, 45, 5);
        playerTransform.DOScale(0.25f, 3f).SetEase(Ease.OutBounce);
        playerTransform.DOMoveY(waterBaseLevel + 0.06f, Mathf.Abs(playerTransform.position.y - waterBaseLevel) * 0.75f).SetEase(Ease.InOutSine);
    }
    public void InputMove(InputCache inputCache)
    {
        if (inputCache.sprintInput)
        {
            _finalPosVector += sprintSpeed * _moveInputVector;
        }
        else
        {
            _finalPosVector += movementSpeed * _moveInputVector;
        }
    }
    public void GravityMove(float waterBaseLevel)
    {
        if (playerTransform.position.y > 0f)
        {
            _finalPosVector += 8f * Vector3.down;
        }
        else if (playerTransform.position.y < waterBaseLevel * 0.95f)
        {
            _finalPosVector += 0.1f * Vector3.down;
        }
        else if (playerTransform.position.y < waterBaseLevel * 0.75f)
        {
            _finalPosVector += 0.2f * Vector3.down;
        }
        else if (playerTransform.position.y < waterBaseLevel * 0.45f)
        {
            _finalPosVector += 0.3f * Vector3.down;
        }
        else
        {
            _finalPosVector += 0.4f * Vector3.down;
        }
    }
    public void BoundaryMove(float waterBaseLevel)
    {
        // Above Ground
        if (playerTransform.position.y < waterBaseLevel)
        {
            _additivePosVector += (_moveInputVector + Vector3.up * 2f).normalized * (waterBaseLevel - playerTransform.position.y);
        }
        // Below Surface
        if (playerTransform.position.y > -0.4)
        {
            _additivePosVector += (_moveInputVector + Vector3.down * 2f).normalized * (0.75f + playerTransform.position.y);
        }
    }
    public void RiseAction(InputCache inputCache)
    {
        if (inputCache.riseInput)
        {
            if (playerTransform.position.y < -0.51f)
            {
                _finalPosVector += verticalSpeed * Vector3.up;
            }
        }
    }
    public void DodgeAction(InputCache inputCache)
    {
        if (inputCache.dodgeInput)
        {
            inputCache.dodgeInput = false;
            _additivePosVector += -playerTransform.right * 8f;
        }
    }
    public void AttackAction(InputCache inputCache)
    {
        if (inputCache.leftClickInput && !attackCharging && !attackActionToken)
        {
            attackChargeTimer = 0f;
            attackCharging = true;
            attackChargeSFX.Play();
        }
        else
        {
            if (attackCharging)
            {
                attackChargeTimer += Time.deltaTime;
                playerHead.localScale = Vector3.one * (1f + Mathf.Clamp(attackChargeTimer, 0f, 2f) * 0.25f);
                //
                if (!inputCache.leftClickInput)
                {
                    attackChargeSFX.Stop();
                    attackActionSFX.Play();
                    if (attackChargeTimer > 0.25f)
                    {
                        _additivePosVector += playerTransform.forward * Mathf.Lerp(6f, 18f, attackChargeTimer * 0.5f);
                        attackCharging = false;
                        attackActionToken = true;
                        attackActionTimer = 0f;
                        playerHead.DOScale(Vector3.one, 0.5f).SetEase(Ease.InOutBounce);
                    }
                    else
                    {
                        _additivePosVector += playerTransform.forward * 6f;
                        attackCharging = false;
                        attackActionToken = true;
                        attackActionTimer = 0f;
                        playerHead.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutSine);
                    }
                }
            }
        }
        if (attackActionToken)
        {
            attackActionHit = Physics.SphereCastAll(playerTransform.position, 0.45f * playerTransform.localScale.x, playerTransform.forward, 0.45f, enemyLayer);
            if (attackActionHit != null)
            {
                if (attackActionHit.Length > 0)
                {
                    for (int i = 0; i < attackActionHit.Length; i++)
                    {
                        Debug.Log("Hit on " + attackActionHit[i].transform.name);
                        GameManager.Instance.OnEnemyHit(int.Parse(attackActionHit[i].transform.name.Split('-')[1]));
                    }
                }
            }
            attackActionTimer += Time.deltaTime;
            if (attackActionTimer > 0.75f)
            {
                attackActionToken = false;
                attackActionTimer = 0f;
            }
        }
    }
}