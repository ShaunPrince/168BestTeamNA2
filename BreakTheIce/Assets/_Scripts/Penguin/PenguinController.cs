using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenguinController : MonoBehaviour
{
    
    public float moveSpeedModifier;
    public float jumpForce;

    public Ray groundCheckRay;
    public Rigidbody _rb;

    private float deltaX;
    private float deltaZ;
    public bool isGrounded;
    // Start is called before the first frame update
    void Start()
    {
        isGrounded = true;
    }

    // Update is called once per frame
    void Update()
    {
        CheckGrounded();
        GetMovement();
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void GetMovement()
    {
        deltaX = Input.GetAxisRaw("Horizontal");
        deltaZ = Input.GetAxisRaw("Vertical");
    }

    private void CheckGrounded()
    {
        if(Physics.Raycast(this.transform.position,Vector3.down,.51f, ~(1<<10)))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void Jump()
    {
        if(isGrounded)
        {
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
    }

    private void ApplyMovement()
    {
        _rb.AddForce(new Vector3(deltaX, 0, deltaZ) * moveSpeedModifier, ForceMode.Acceleration);
    }
}
