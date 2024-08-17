using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    //
    private Vector3 moveVector;
    //
    void Update()
    {
        moveVector = Vector3.zero;
        //
        if (Keyboard.current.wKey.isPressed)
        {
            moveVector += Vector3.forward;
        }
        if (Keyboard.current.sKey.isPressed)
        {
            moveVector -= Vector3.forward;
        }
        if (Keyboard.current.aKey.isPressed)
        {
            moveVector -= Vector3.right;
        }
        if (Keyboard.current.dKey.isPressed)
        {
            moveVector += Vector3.right;
        }
        //
        transform.position += moveSpeed * Time.deltaTime * moveVector.normalized;
    }
}