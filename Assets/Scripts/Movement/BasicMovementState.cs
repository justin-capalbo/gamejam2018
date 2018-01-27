using UnityEngine;
using System.Collections;



/// <summary>
/// The various states you can use to check if your character is doing something at the current frame
/// </summary>
public class BasicMovementState 
{
	/// is the character colliding right ?
	public bool isCollidingRight { get; set; }
	/// is the character colliding left ?
	public bool isCollidingLeft { get; set; }
	/// is the character colliding with something above it ?
	public bool isCollidingAbove { get; set; }
	/// is the character colliding with something above it ?
	public bool isCollidingBelow { get; set; }
	/// is the character colliding with anything ?
	public bool hasCollisions { get { return isCollidingRight || isCollidingLeft || isCollidingAbove || isCollidingBelow; }}
	
	/// is the character moving down a slope ?
	public bool isMovingDownSlope { get; set; }
	/// is the character moving up a slope ?
	public bool isMovingUpSlope { get; set; }
	/// returns the slope the character is moving on angle
	public float slopeAngle { get; set; }
	/// returns true if the slope angle is ok to walk on
	public bool slopeAngleOK { get; set; }
	/// returns true if the character is standing on a moving platform
	public bool onAMovingPlatform { get; set; }
	
	/// Is the character grounded ? 
	public bool isGrounded { get { return isCollidingBelow; } }
	/// is the character falling right now ?
	public bool isFalling { get; set; }
	/// was the character grounded last frame ?
	public bool wasGroundedLastFrame { get ; set; }
	/// was the character grounded last frame ?
	public bool wasTouchingTheCeilingLastFrame { get ; set; }
	/// did the character just become grounded ?
	public bool justGotGrounded { get ; set;  }


  
    ///  <summary>
	///  Reset all collision states to false
	///  </summary>    
    public void reset()
	{
		isMovingUpSlope = false;
		isMovingDownSlope = false;
		isCollidingLeft = false;
		isCollidingRight = false;
		isCollidingAbove = false;
		slopeAngleOK = false;
	    justGotGrounded = false; 
		isFalling=true;
		slopeAngle = 0;
	}

    /// <summary>
    /// Serializes the collision states
    /// </summary>
    /// <returns>A <see cref="System.String"/> that represents the current collision states.</returns>
    public override string ToString ()
	{
		return string.Format("(controller: r:{0} l:{1} a:{2} b:{3} down-slope:{4} up-slope:{5} angle: {6}",
		                     isCollidingRight,
		                     isCollidingLeft,
		                     isCollidingAbove,
		                     isCollidingBelow,
		                     isMovingDownSlope,
		                     isMovingUpSlope,
		                     slopeAngle);
	}	
}