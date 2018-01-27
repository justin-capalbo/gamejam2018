using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// The various parameters related to the AdvancedMovement class.
/// </summary>
[Serializable]
public class AdvancedMovementParameters 
{
	[Header("Jump")]
	/// defines how high the character can jump
	public float jumpHeight = 3.025f;
	/// the minimum time in the air allowed when jumping - this is used for pressure controlled jumps
	public float jumpMinimumAirTime = 0.1f;
	/// the maximum number of jumps allowed (0 : no jump, 1 : normal jump, 2 : double jump, etc...)
	public int numberOfJumps=3;
	public enum JumpBehavior
	{
		CanJumpOnGround,
		CanJumpAnywhere,
		CantJump,
		CanJumpAnywhereAnyNumberOfTimes
	}
	/// basic rules for jumps : where can the player jump ?
	public JumpBehavior jumpRestrictions;
	/// if true, the jump duration/height will be proportional to the duration of the button's press
	public bool jumpIsProportionalToThePressTime=true;
	
	[Space(10)]	
	[Header("Speed")]
    /// basic movement speed
    public float walkSpeed = 8f;
    public float broadcastWalkSpeed = 0f;
    public float vMovementSpeed = 8f;

    public float hMovementSpeed { get; set; }

}
