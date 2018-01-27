using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// The various parameters related to the AdvancedMovement class.
/// </summary>
[Serializable]
public class AdvancedMovementParameters 
{
	[Header("Control Type")]
	/// If set to true, acceleration / deceleration will take place when moving / stopping
	public bool smoothMovement=true;
	
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
	public float movementSpeed = 8f;
	/// the speed of the character when it's crouching
	public float crouchSpeed = 4f;
	/// the speed of the character when it's walking
	public float walkSpeed = 8f;
	/// the speed of the character when it's running
	public float runSpeed = 16f;
	/// the speed of the character when climbing a ladder
	public float ladderSpeed = 2f;
	
	[Space(10)]	
	[Header("Dash")]
	/// the duration of dash (in seconds)
	public float dashDuration = 0.15f;
	/// the force of the dash
	public float dashForce = 5f;	
	/// the duration of the cooldown between 2 dashes (in seconds)
	public float dashCooldown = 2f;	
	
	[Space(10)]	
	[Header("Walljump")]
	/// the force of a walljump
	public float wallJumpForce = 3f;
	/// the slow factor when wall clinging
	public float wallClingingSlowFactor=0.6f;

}
