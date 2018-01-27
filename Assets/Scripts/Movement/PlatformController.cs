﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

public class PlatformController : MonoBehaviour, IJumper, IMover
{
    //reference to basic movement controller
    public BasicMovementController basicMovementController;

    [Header("Movement Parameters")]
    public AdvancedMovementPermissions defaultPermissions;
    private AdvancedMovementPermissions currentPermissions;
    public float hMovementSpeed;
    public float vMovementSpeed;

    [Header("Jump Parameters")]
    public float jumpHeight;
    private bool isJumping;

    private Vector2 jumpOriginPoint;

    private float originalGravity;

    private void Start()
    {
        originalGravity = basicMovementController.currentParameters.gravity;
        currentPermissions = defaultPermissions;
        GravityActive(false);
    }


    private void HorizontalMovement(float horizontalMove)
    {
        if (!currentPermissions.hMoveEnabled) return;

        float normalizedHorizontalSpeed = 0;
        // If the value of the horizontal axis is positive, the character must face right.
        if (horizontalMove > 0.1f)
        {
            normalizedHorizontalSpeed = horizontalMove;
        }
        // If it's negative, then we're facing left
        else if (horizontalMove < -0.1f)
        {
            normalizedHorizontalSpeed = horizontalMove;
        }
        else
        {
            normalizedHorizontalSpeed = 0;
        }

        basicMovementController.SetHorizontalForce(normalizedHorizontalSpeed * hMovementSpeed);
    }

    /// <summary>
    /// Called at Update(), handles vertical movement
    /// </summary>
    private void VerticalMovement(float verticalMove)
    {
        if (!currentPermissions.vMoveEnabled) return;

        float normalizedVerticalSpeed = 0;

        // If the value of the horizontal axis is positive, the character must face right.
        if (verticalMove > 0.1f)
        {
            normalizedVerticalSpeed = verticalMove;
        }
        // If it's negative, then we're facing left
        else if (verticalMove < -0.1f)
        {
            normalizedVerticalSpeed = verticalMove;
        }
        else
        {
            normalizedVerticalSpeed = 0;
        }

        basicMovementController.SetVerticalForce(normalizedVerticalSpeed * vMovementSpeed);
    }
    
    public void Move(float moveH, float moveV)
    {
        VerticalMovement(moveV);
        HorizontalMovement(moveH);
    }

    public void Jump(bool jumpPress, bool jumpRelease)
    {
        if (isJumping) return;

        if (jumpPress)
        {
            StartCoroutine("JumpStart");
        }

        if(jumpRelease)
        {
            //JumpStop();
        }
    }

    private IEnumerator JumpStart()
    {
        print("jumping");
        jumpOriginPoint = transform.position;
        isJumping = true;

        GravityActive(true);
        currentPermissions.vMoveEnabled = false;
        basicMovementController.SetVerticalForce(Mathf.Sqrt(2f * jumpHeight * Mathf.Abs(basicMovementController.currentParameters.gravity)));

        while(basicMovementController.speed.y > 0)
        {
            yield return null;
        }
        
        StartCoroutine("SlowFall");
    }

    public IEnumerator SlowFall()
    {
        print("transform: " + transform.position.y);
        print("origin: " + jumpOriginPoint.y);

        while (transform.position.y > jumpOriginPoint.y)
        {
            print("wat");
            yield return null;
        }

        print("out of coroutine");
        basicMovementController.SetVerticalForce(0);
        GravityActive(false);
        isJumping = false;
        currentPermissions.vMoveEnabled = defaultPermissions.vMoveEnabled;

    }

    private void GravityActive(bool state)
    {
        if (state == true)
        {
            if (basicMovementController.currentParameters.gravity == 0)
            {
                basicMovementController.currentParameters.gravity = originalGravity;
            }
        }
        else
        {
            if (basicMovementController.currentParameters.gravity != 0)
                originalGravity = basicMovementController.currentParameters.gravity;
            basicMovementController.currentParameters.gravity = 0;
        }
    }

}
