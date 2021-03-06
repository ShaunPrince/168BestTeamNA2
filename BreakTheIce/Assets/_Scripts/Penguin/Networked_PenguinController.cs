﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Networked_PenguinController : MonoBehaviour
{

    public float moveSpeedModifier;
    public float jumpForce;

    public Ray groundCheckRay;
    public Rigidbody _rb;

    private float deltaX;
    private float deltaZ;
    public bool isGrounded;

    // rotation controller
    public GameObject PenguinRotation;
    // To stop duplicate moves from being sent (if player is not chnaging pos)
    private Vector3 prevPos;
    // Start is called before the first frame update
    void Start()
    {
        isGrounded = true;
        prevPos = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // only do normal penguin movement if player is the penguin
        if (GameManager.Instance.playerType == PlayerType.Penguin)
        {
            CheckGrounded();
            GetMovement();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }

    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.playerType == PlayerType.Penguin)
        {
            ApplyMovement();
        }
    }

    private void GetMovement()
    {
        deltaX = Input.GetAxisRaw("Horizontal");
        deltaZ = Input.GetAxisRaw("Vertical");

    }

    private void CheckGrounded()
    {
        if (Physics.Raycast(this.transform.position, Vector3.down, .51f, ~(1 << 10)))
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
        if (isGrounded)
        {
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
    }

    private void ApplyMovement()
    {
        if (isGrounded)
            _rb.AddForce(new Vector3(deltaX, 0, deltaZ) * moveSpeedModifier, ForceMode.Acceleration);
        else
            _rb.AddForce(new Vector3(deltaX, 0, deltaZ) * moveSpeedModifier * 0.25f, ForceMode.Acceleration);

        //Fun Rotation!
        if (deltaZ != 0)
            PenguinRotation.transform.Rotate(new Vector3(0, PenguinRotation.transform.rotation.y / deltaZ * 10, 0));
        else
            PenguinRotation.transform.Rotate(new Vector3(0, deltaX * 5, 0));

        // check if the player has moved or they are stationary
        if (prevPos != this.transform.position)
        {
            // send movement to server
            Client.Instance.SendPenguinMove(this.transform.position.x, this.transform.position.y, this.transform.position.z);

            // since player has moved, update prev location
            prevPos = this.transform.position;
        }
        // else if hasn't moved, do not send position
        
    }
}
