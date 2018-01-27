using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PlatformController : MonoBehaviour, IJumper, IMover
{
    //reference to basic movement controller
    public BasicMovementController basicMovementController;

    public float hMovementSpeed;
    public float vMovementSpeed;
    
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

        basicMovementController.SetHorizontalForce(normalizedHorizontalSpeed * hMovementSpeed);
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

        basicMovementController.SetVerticalForce(normalizedVerticalSpeed * vMovementSpeed);
    }
    
    public void Move(float moveH, float moveV)
    {
        VerticalMovement(moveV);
        HorizontalMovement(moveH);
    }

    public void Jump(bool jumpPress, bool jumpRelease)
    {

    }
}
