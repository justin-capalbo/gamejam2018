﻿using UnityEngine;
using System.Collections;

/// <summary>
/// The various states you can use to check if your character is doing something at the current frame
/// by Renaud Forestié
/// </summary>
public class AdvancedMovementState 
{

	/// can the character jump right now 
	public bool canJump{get;set;}	
	/// can the character move freely right now ?
	public bool canMoveFreely{get;set;}

	public int numberOfJumpsLeft;


	/// <summary>
	/// Initializes all states to their default value
	/// </summary>
	public void Initialize()
	{				
		canMoveFreely = true;
	}

}