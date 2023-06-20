using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum GravityDirection
    {
        right,
        left,
    }

    [SerializeField] private float forceSize;
    public float gravitySize;
    private Rigidbody2D rb;
    private Vector2 gravity;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SetPlayerGravity(GravityDirection.right);
    }
    private void Update()
    {
        rb.AddForce(gravity * Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = new Vector2(Mathf.Sign(gravity.x) * -1 * forceSize,0);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SetPlayerGravity(GravityDirection.right);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SetPlayerGravity(GravityDirection.left);
        }
    }
    public void SetPlayerGravity(GravityDirection gravityDirection)
    {
        if(gravityDirection == GravityDirection.right)
        {
            gravity = Vector2.right * gravitySize;
        }
        else if(gravityDirection == GravityDirection.left)
        {
            gravity = Vector2.left * gravitySize;
        }
    }
}
