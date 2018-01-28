using System;
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
    public float hMovementSpeed;
    public float vMovementSpeed;

    [Header("Jump Parameters")]
    public float jumpHeight;
    private bool isJumping;

    private Vector2 jumpOriginPoint;

    private float originalGravity;

    public Transform parentTransform;
    public float returnSpeed;

    private float currentHSpeed, currentVSpeed;

    protected BasicMovementController playerMovement;

    private void Start()
    {
        originalGravity = basicMovementController.currentParameters.gravity;
        GravityActive(false);

        playerMovement = GameObject.FindWithTag("Player").GetComponent<BasicMovementController>();
    }


    private void HorizontalMovement(float horizontalMove)
    {

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

        currentHSpeed = normalizedHorizontalSpeed * hMovementSpeed;
        basicMovementController.SetHorizontalForce(currentHSpeed);

        if (!playerMovement)
            playerMovement = GameObject.FindWithTag("Player").GetComponent<BasicMovementController>();

        if (playerMovement.standingOn == this.gameObject &&
            playerMovement.basicMovementState.isGrounded)
        {
            playerMovement.SetHorizontalForce(currentHSpeed);
        }
    }

    public void StopMovement()
    {
        basicMovementController.SetHorizontalForce(0);
        basicMovementController.SetVerticalForce(0);
    }

    /// <summary>
    /// Called at Update(), handles vertical movement
    /// </summary>
    private void VerticalMovement(float verticalMove)
    {

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

        currentVSpeed = normalizedVerticalSpeed * hMovementSpeed;
        basicMovementController.SetVerticalForce(currentVSpeed);
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
        jumpOriginPoint = parentTransform.position;
        isJumping = true;

        GravityActive(true);
        basicMovementController.CollisionsOff();
        basicMovementController.SetVerticalForce(Mathf.Sqrt(2f * jumpHeight * Mathf.Abs(basicMovementController.currentParameters.gravity)));

        while(basicMovementController.speed.y > 0)
        {
            yield return null;
        }
        
        StartCoroutine("SlowFall");
    }

    public IEnumerator SlowFall()
    {
        basicMovementController.SetVerticalForce(0);
        GravityActive(false);

        while (parentTransform.position.y > jumpOriginPoint.y)
        {
            parentTransform.position = Vector2.MoveTowards(parentTransform.position, jumpOriginPoint, returnSpeed);
            //parentTransform.position = new Vector2(parentTransform.position.x, Mathf.Lerp(parentTransform.position.y, jumpOriginPoint.y, .02f));
            //print("wat");
            yield return null;
        }

        basicMovementController.CollisionsOn();
        isJumping = false;

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
