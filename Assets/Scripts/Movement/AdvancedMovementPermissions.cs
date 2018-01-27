using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// A list of permissions for your character. 
/// set them to false if you don't want the player to EVER be able to jump, dash, or any of the actions below
/// (you can set these at the start, or unlock them in your game)	
/// </summary>
[Serializable]
public class AdvancedMovementPermissions
{
	public bool runEnabled=true;
	public bool dashEnabled=true;
	public bool jumpEnabled=true;
	public bool crouchEnabled=true;
	public bool wallJumpEnabled=true;
	public bool wallClingingEnabled=true;
}

