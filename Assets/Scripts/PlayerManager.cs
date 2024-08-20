using System;
using System.Collections.Generic;
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
    public GameObject attackChargeVFX;
    public AudioSource attackActionSFX;
    public ParticleSystem attackActionVFX;
    public GameObject[] getHitVFXArray;
    public AudioSource getHitSFX;
    public GameObject deathVFX;
    public GameObject growVFX;
    public GameObject dodgeIndicator;

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
    private List<int> attackActionIndexHistory;
    private int getHitIndex;
    private int attackHitIndex;
    private float dodgeTimer;
    //
    void Awake()
    {
        playerHead.DOShakeRotation(3.5f, 8, 3).SetLoops(-1, LoopType.Yoyo);
        attackActionIndexHistory = new();
        dodgeTimer = 5f;
    }
    //
    public void UpdatePlayerAction(InputCache inputCache, float waterBaseLevel, float totalMass, int phase)
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
        GravityMove(waterBaseLevel, phase);
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
    public void GrowReaction(float growRatio)
    {
        growVFX.transform.localScale = Vector3.one * (0.25f + growRatio);
        growVFX.SetActive(false);
        growVFX.SetActive(true);
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
    public void GravityMove(float waterBaseLevel, int phase)
    {
        if (playerTransform.position.y > 0f)
        {
            _finalPosVector += 8f * Vector3.down;
        }
        else if (playerTransform.position.y < waterBaseLevel * 0.95f)
        {
            if (phase > 2)
            {
                _finalPosVector += 0.4f * Vector3.down;
            }
            else
            {
                _finalPosVector += 0.1f * Vector3.down;
            }
        }
        else if (playerTransform.position.y < waterBaseLevel * 0.75f)
        {
            if (phase > 2)
            {
                _finalPosVector += 0.6f * Vector3.down;
            }
            else
            {
                _finalPosVector += 0.2f * Vector3.down;
            }
        }
        else if (playerTransform.position.y < waterBaseLevel * 0.45f)
        {
            if (phase > 2)
            {
                _finalPosVector += 0.8f * Vector3.down;
            }
            else
            {
                _finalPosVector += 0.3f * Vector3.down;
            }
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
        // Outside Circle
        if (playerTransform.position.x * playerTransform.position.x + playerTransform.position.z * playerTransform.position.z > 16000f)
        {
            _additivePosVector += (_moveInputVector - playerTransform.position * 5f).normalized * 4f;
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
        dodgeTimer += Time.deltaTime;
        if (dodgeTimer > 2.25f)
        {
            dodgeIndicator.SetActive(true);
            if (inputCache.rightClickInput)
            {
                if (inputCache.moveInput.sqrMagnitude > 0.5)
                {
                    inputCache.rightClickInput = false;
                    _additivePosVector += _moveInputVector * 14f;
                    dodgeTimer = 0f;
                    dodgeIndicator.transform.DOScale(0.01f, 0.25f).OnComplete(() =>
                    {
                        dodgeIndicator.transform.DOScale(0.1f, 2f).SetEase(Ease.InOutSine);
                    });
                }
                else
                {
                    inputCache.rightClickInput = false;
                    _additivePosVector += -(-playerTransform.up * 0.25f + playerTransform.forward) * 14f;
                    dodgeTimer = 0f;
                    dodgeIndicator.transform.DOScale(0.01f, 0.25f).OnComplete(() =>
                    {
                        dodgeIndicator.transform.DOScale(0.1f, 2f).SetEase(Ease.InOutSine);
                    });
                }
            }
        }
    }
    public void AttackAction(InputCache inputCache)
    {
        if (inputCache.leftClickInput && !attackCharging && !attackActionToken)
        {
            attackChargeTimer = 0f;
            attackCharging = true;
            attackChargeSFX.Play();
            attackChargeVFX.SetActive(false);
            attackChargeVFX.SetActive(true);
        }
        else
        {
            if (attackCharging)
            {
                attackChargeTimer += Time.deltaTime;
                playerHead.localScale = Vector3.one * (1f + Mathf.Clamp(attackChargeTimer, 0f, 2.5f) * 0.25f);
                //
                if (!inputCache.leftClickInput)
                {
                    attackChargeSFX.Stop();
                    attackChargeVFX.SetActive(false);
                    attackActionSFX.Play();
                    attackActionVFX.gameObject.SetActive(true);
                    attackActionVFX.Play();
                    if (attackChargeTimer > 0.25f)
                    {
                        _additivePosVector += playerTransform.forward * Mathf.Lerp(6f, 20f, attackChargeTimer * 0.5f);
                        attackActionVFX.transform.localScale = Vector3.one * Mathf.Lerp(0.5f, 1.5f, attackChargeTimer * 0.5f);
                        attackCharging = false;
                        attackActionIndexHistory.Clear();
                        attackActionToken = true;
                        attackActionTimer = 0f;
                        playerHead.DOScale(Vector3.one, 0.5f).SetEase(Ease.InOutBounce);
                    }
                    else
                    {
                        _additivePosVector += playerTransform.forward * 6f;
                        attackActionVFX.transform.localScale = Vector3.one * 0.5f;
                        attackCharging = false;
                        attackActionIndexHistory.Clear();
                        attackActionToken = true;
                        attackActionTimer = 0f;
                        playerHead.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutSine);
                    }
                }
            }
        }
        if (attackActionToken)
        {
            attackActionHit = Physics.SphereCastAll(playerTransform.position, 0.5f * playerTransform.localScale.x, playerTransform.forward, 0.4f, enemyLayer);
            if (attackActionHit != null)
            {
                if (attackActionHit.Length > 0)
                {
                    for (int i = 0; i < attackActionHit.Length; i++)
                    {
                        attackHitIndex = int.Parse(attackActionHit[i].transform.name.Split('-')[1]);
                        if (!attackActionIndexHistory.Contains((attackHitIndex)))
                        {
                            Debug.Log("Hit on " + attackActionHit[i].transform.name);
                            attackActionIndexHistory.Add(attackHitIndex);
                            GameManager.Instance.OnEnemyHit(attackHitIndex, Mathf.Lerp(0.75f, 1.8f, attackChargeTimer * 0.5f));
                        }
                    }
                }
            }
            attackActionTimer += Time.deltaTime;
            if (attackActionTimer > 0.75f)
            {
                attackActionToken = false;
                attackActionVFX.Stop();
                attackActionIndexHistory.Clear();
                attackActionTimer = 0f;
            }
        }
    }
}