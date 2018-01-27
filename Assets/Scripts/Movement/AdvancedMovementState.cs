using UnityEngine;
using System.Collections;

/// <summary>
/// The various states you can use to check if your character is doing something at the current frame
/// by Renaud Forestié
/// </summary>
public class AdvancedMovementState 
{

	/// can the character jump right now 
	public bool canJump{get;set;}	
	/// can the character dash right now ?
	public bool canDash{get;set;}
	/// can the character move freely right now ?
	public bool canMoveFreely{get;set;}
	/// can the character jetpack ?
	public bool canJetpack{ get; set; }
	/// the number of jumps left to the character
	public int numberOfJumpsLeft;

	/// true if the character is dashing right now
	public bool dashing{get;set;}
	/// true if the character is running right now
	public bool running{get;set;}
	/// true if the character is crouching right now
	public bool crouching{get;set;}
	/// true if the character was crouching during the previous frame
	public bool crouchingPreviously{get;set;}
	/// true if the character is clinging to a wall right now
	public bool wallClinging{get;set;}
	/// true if the character is jetpacking right now
	public bool jetpacking{get;set;}
	/// true if the character is dash diving right now
	public bool diving{get;set;}
	/// true if the character is colliding with a ladder
	public bool ladderColliding{get;set;}
	/// true if the character is colliding with the top of a ladder
	public bool ladderTopColliding{get;set;}
	/// true if the character is climbing on a ladder
	public bool ladderClimbing{get;set;}
	/// the current ladder climbing speed of the character
	public float ladderClimbingSpeed{get;set;}
	/// the remaining jetpack fuel duration (in seconds)
	public float jetpackFuelDurationLeft{get;set;}


	/// <summary>
	/// Initializes all states to their default value
	/// </summary>
	public void Initialize()
	{				
		canMoveFreely = true;
		canDash = true;;
		canJetpack = true;
		dashing = false;
		running = false;
		crouching = false;
		crouchingPreviously=false;
		wallClinging = false;
		jetpacking = false;
		diving = false;
		ladderClimbing=false;
		ladderColliding=false;
		ladderTopColliding=false;
		ladderClimbingSpeed=0f;
	}

}