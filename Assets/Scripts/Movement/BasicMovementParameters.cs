using System;
using UnityEngine;



/// <summary>
/// Parameters for the BasicMovementController class.
/// This is where you define your slope limit, gravity, and speed dampening factors
/// </summary>
[Serializable]
public class BasicMovementParameters 
{
    /// Gravity
	public float gravity = -15;
    /// Speed factor on the ground
    public float groundAcceleration = 20f;
    /// Speed factor in the air
    public float airAcceleration = 5f;

    /// Maximum velocity for your character, to prevent it from moving too fast on a slope for example
    public Vector2 maxVelocity = new Vector2(200f, 200f);
	public int maxSpeed=200;

	/// Maximum angle (in degrees) the character can walk on
	[Range(0,90)]
	public float maximumSlopeAngle = 45;	
    	
	
}