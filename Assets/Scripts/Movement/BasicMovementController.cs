using UnityEngine;
using System.Collections;

public class BasicMovementController : MonoBehaviour {

	/// the various states of our character
	public BasicMovementState basicMovementState { get; private set; }
	/// the initial parameters
	public BasicMovementParameters defaultParameters;
	/// the current parameters
	public BasicMovementParameters currentParameters{get{return overrideParameters ?? defaultParameters;}}

    /// the current velocity of the character
	public Vector2 speed { get { return _speed; } }

    [Space(10)]	
	[Header("Collision Masks")]
	/// The layer mask the platforms are on
	public LayerMask platformMask=0;
	/// The layer mask the moving platforms are on
	public LayerMask movingPlatformMask=0;
	/// The layer mask the one way platforms are on
	public LayerMask edgeColliderPlatformMask=0;
	/// gives you the object the character is standing on
	public GameObject standingOn { get; private set; }	
	

	[Space(10)]	
	[Header("Raycasting")]
	public int numberOfHorizontalRays = 8;
	public int numberOfVerticalRays = 8;	
	public float rayOffset=0.05f; 

	public Vector3 colliderCenter {get
		{
			Vector3 colliderCenter = Vector3.Scale(transform.localScale, boxCollider.offset);
			return colliderCenter;
		}}
	public Vector3 colliderPosition {get
		{
			Vector3 colliderPosition = transform.position + colliderCenter;
			return colliderPosition;
		}}
	public Vector3 colliderSize {get
		{
			Vector3 colliderSize = Vector3.Scale(transform.localScale, boxCollider.size);
			return colliderSize;
		}}
	public Vector3 bottomPosition {get
		{
			Vector3 colliderBottom = new Vector3(colliderPosition.x,colliderPosition.y - (colliderSize.y / 2),colliderPosition.z);
			return colliderBottom;
		}}



    // private local variables
    private Vector2 _speed;
    private BasicMovementParameters overrideParameters;
	private Vector2 externalForce;
	private float fallSlowFactor;
	private Vector2 newPosition;
	private BoxCollider2D boxCollider;
	private Transform positionTransform;
	private Rect rayBoundsRectangle;
	private float movingPlatformsCurrentGravity;
	private LayerMask platformMaskSave;


	private const float largeValue=500000f;
	private const float smallValue=0.0001f;
	private const float obstacleHeightTolerance=0.05f;
	private const float movingPlatformsGravity=-150;






    /// <summary>
	/// Initialization
	/// </summary>
	public void Awake()
    {
        positionTransform = this.gameObject.transform;
		boxCollider = (BoxCollider2D)GetComponent<BoxCollider2D>();
		basicMovementState = new BasicMovementState();
		
		// we add the edge collider platform and moving platform masks to our initial platform mask so they can be walked on	
		platformMaskSave = platformMask;	
		platformMask |= edgeColliderPlatformMask;
		platformMask |= movingPlatformMask;
		
		basicMovementState.reset();
		SetRaysParameters();
	}

	/// <summary>
	/// Use this to add force to the character
	/// </summary>
	/// <param name="force">Force to add to the character.</param>
	public void AddForce(Vector2 force)
	{
		_speed += force;	
		externalForce += force;
	}
	
	/// <summary>
	///  use this to set the horizontal force applied to the character
	/// </summary>
	/// <param name="x">The x value of the velocity.</param>
	public void AddHorizontalForce(float x)
	{
		_speed.x += x;
		externalForce.x += x;
	}
	
	/// <summary>
	///  use this to set the vertical force applied to the character
	/// </summary>
	/// <param name="y">The y value of the velocity.</param>
	public void AddVerticalForce(float y)
	{
		_speed.y += y;
		externalForce.y += y;
	}
	
	/// <summary>
	/// Use this to set the force applied to the character
	/// </summary>
	/// <param name="force">Force to apply to the character.</param>
	public void SetForce(Vector2 force)
	{
		_speed = force;
		externalForce = force;	
	}
	
	/// <summary>
	///  use this to set the horizontal force applied to the character
	/// </summary>
	/// <param name="x">The x value of the velocity.</param>
	public void SetHorizontalForce (float x)
	{
		_speed.x = x;
		externalForce.x = x;
	}
	
	/// <summary>
	///  use this to set the vertical force applied to the character
	/// </summary>
	/// <param name="y">The y value of the velocity.</param>
	public void SetVerticalForce (float y)
	{
		_speed.y = y;
		externalForce.y = y;
	}

    /// <summary>
    /// Creates a rectangle with the boxcollider's size for ease of use and draws debug lines along the different raycast origin axis
    /// </summary>	
    public void SetRaysParameters()
    {
        rayBoundsRectangle = new Rect(boxCollider.bounds.min.x,
                                       boxCollider.bounds.min.y,
                                       boxCollider.bounds.size.x,
                                       boxCollider.bounds.size.y);

        Debug.DrawLine(new Vector2(rayBoundsRectangle.center.x, rayBoundsRectangle.yMin), new Vector2(rayBoundsRectangle.center.x, rayBoundsRectangle.yMax));
        Debug.DrawLine(new Vector2(rayBoundsRectangle.xMin, rayBoundsRectangle.center.y), new Vector2(rayBoundsRectangle.xMax, rayBoundsRectangle.center.y));
    }

    /// <summary>
    /// Disables the collisions for the specified duration
    /// </summary>
    /// <param name="duration">the duration for which the collisions must be disabled</param>
    public IEnumerator DisableCollisions(float duration)
    {
        // we turn the collisions off
        CollisionsOff();
        // we wait for a few seconds
        yield return new WaitForSeconds(duration);
        // we turn them on again
        CollisionsOn();
    }

    public void ResetMovingPlatformsGravity()
    {
        movingPlatformsCurrentGravity = 0f;
    }

    public void CollisionsOn()
    {
        platformMask = platformMaskSave;
        platformMask |= edgeColliderPlatformMask;
        platformMask |= movingPlatformMask;
    }

    public void CollisionsOff()
    {
        platformMask = 0;
    }

    public void SlowFall(float factor)
    {
        fallSlowFactor = factor;
    }

    /// <summary>
    /// Every frame, we apply the gravity to our character, then check using raycasts if an object's been hit, and modify its new position 
    /// accordingly. When all the checks have been done, we apply that new position. 
    /// </summary>
    private void LateUpdate()
    {
        _speed.y += (currentParameters.gravity + movingPlatformsCurrentGravity) * Time.deltaTime;
		
		if (fallSlowFactor!=0)
		{
			_speed.y*=fallSlowFactor;
		}
		
		newPosition=speed * Time.deltaTime;
		
		basicMovementState.wasGroundedLastFrame = basicMovementState.isCollidingBelow;
		basicMovementState.wasTouchingTheCeilingLastFrame = basicMovementState.isCollidingAbove;
		basicMovementState.reset(); 
		
		SetRaysParameters();

        //Detect collisions Horizontal
        float moveDirX = 1;
        if ((_speed.x < 0) || (externalForce.x < 0))
            moveDirX = -1;
        CastRaysToTheSides(moveDirX);
        //If not moving, cast in both directions
        if (_speed.x == 0)
            CastRaysToTheSides(moveDirX * -1);
        
		CastRaysBelow();	
		CastRaysAbove();
		
		positionTransform.Translate(newPosition,Space.World);
		
		SetRaysParameters();
		
		// we compute the new speed
		if (Time.deltaTime > 0) 
		{
			_speed = newPosition / Time.deltaTime;		
		}
		
		
		externalForce.x=0;
		externalForce.y=0;
		
		// we make sure the velocity doesn't exceed the MaxVelocity specified in the parameters
		Mathf.Clamp(_speed.x,-currentParameters.maxVelocity.x,currentParameters.maxVelocity.x);
		Mathf.Clamp(_speed.y,-currentParameters.maxVelocity.y,currentParameters.maxVelocity.y);
		
		// we change states depending on the outcome of the movement
		if (!basicMovementState.wasGroundedLastFrame && basicMovementState.isCollidingBelow) 
		{
			basicMovementState.justGotGrounded = true;
		}
	}


	/// <summary>
	/// Casts rays to the sides of the character, from its center axis.
	/// If we hit a wall/slope, we check its angle and move or not according to it.
	/// </summary>
	private void CastRaysToTheSides(float dir) 
	{
        /* float movementDirection=1;	
		if ((_speed.x < 0) || (externalForce.x<0))
			movementDirection = -1; */
        float movementDirection = dir; 
		
		float horizontalRayLength = Mathf.Abs(_speed.x*Time.deltaTime) + rayBoundsRectangle.width/2 + rayOffset*2;
		
		Vector2 horizontalRayCastFromBottom=new Vector2(rayBoundsRectangle.center.x,
		                                                rayBoundsRectangle.yMin+obstacleHeightTolerance);										
		Vector2 horizontalRayCastToTop=new Vector2(	rayBoundsRectangle.center.x,
		                                           rayBoundsRectangle.yMax-obstacleHeightTolerance);				
		
		RaycastHit2D[] hitsStorage = new RaycastHit2D[numberOfHorizontalRays];	
		
		for (int i=0; i<numberOfHorizontalRays;i++)
		{	
			Vector2 rayOriginPoint = Vector2.Lerp(horizontalRayCastFromBottom,horizontalRayCastToTop,(float)i/(float)(numberOfHorizontalRays-1));
			
			if ( basicMovementState.wasGroundedLastFrame && i == 0 )			
				hitsStorage[i] = BasicMovementController.RayCast (rayOriginPoint,movementDirection*Vector2.right,horizontalRayLength,platformMask,true,Color.red);	
			else
				hitsStorage[i] = BasicMovementController.RayCast (rayOriginPoint,movementDirection*Vector2.right,horizontalRayLength,platformMask & ~edgeColliderPlatformMask,true,Color.red);			
			
			if (hitsStorage[i].distance >0)
			{						
				float hitAngle = Mathf.Abs(Vector2.Angle(hitsStorage[i].normal, Vector2.up));								
				
				if (hitAngle > currentParameters.maximumSlopeAngle)
				{														
					if (movementDirection < 0)		
						basicMovementState.isCollidingLeft=true;
					else
						basicMovementState.isCollidingRight=true;						
					
					basicMovementState.slopeAngleOK=false;
					
					if (movementDirection<=0)
					{
						newPosition.x = -Mathf.Abs(hitsStorage[i].point.x - horizontalRayCastFromBottom.x) 
							+ rayBoundsRectangle.width/2 
								+ rayOffset*2;
					}
					else
					{						
						newPosition.x = Mathf.Abs(hitsStorage[i].point.x - horizontalRayCastFromBottom.x) 
							- rayBoundsRectangle.width/2 
								- rayOffset*2;						
					}					
					
					_speed = new Vector2(0, _speed.y);
					break;
				}
			}						
		}
		
	}
	
	/// <summary>
	/// Every frame, we cast a number of rays below our character to check for platform collisions
	/// </summary>
	private void CastRaysBelow()
	{
		if (newPosition.y < -smallValue)
		{
			basicMovementState.isFalling=true;
		}
		else
		{
			basicMovementState.isFalling = false;
		}
		
		if ((currentParameters.gravity > 0) && (!basicMovementState.isFalling))
			return;
		
		float rayLength = rayBoundsRectangle.height/2 + rayOffset ; 		
		if (newPosition.y<0)
		{
			rayLength+=Mathf.Abs(newPosition.y);
		}
		
		
		Vector2 verticalRayCastFromLeft=new Vector2(rayBoundsRectangle.xMin+newPosition.x,
		                                            rayBoundsRectangle.center.y+rayOffset);	
		Vector2 verticalRayCastToRight=new Vector2(	rayBoundsRectangle.xMax+newPosition.x,
		                                           rayBoundsRectangle.center.y+rayOffset);					
		
		RaycastHit2D[] hitsStorage = new RaycastHit2D[numberOfVerticalRays];
		float smallestDistance=largeValue; 
		int smallestDistanceIndex=0; 						
		bool hitConnected=false; 		
		
		for (int i=0; i<numberOfVerticalRays;i++)
		{			
			Vector2 rayOriginPoint = Vector2.Lerp(verticalRayCastFromLeft,verticalRayCastToRight,(float)i/(float)(numberOfVerticalRays-1));
			
			if ((newPosition.y>0) && (!basicMovementState.wasGroundedLastFrame))
				hitsStorage[i] = BasicMovementController.RayCast (rayOriginPoint,-Vector2.up,rayLength,platformMask & ~edgeColliderPlatformMask,true,Color.blue);	
			else
				hitsStorage[i] = BasicMovementController.RayCast (rayOriginPoint,-Vector2.up,rayLength,platformMask,true,Color.blue);					
			
			if ((Mathf.Abs(hitsStorage[smallestDistanceIndex].point.y - verticalRayCastFromLeft.y)) <  smallValue)
			{
				break;
			}		
			
			if (hitsStorage[i])
			{
				hitConnected=true;
				if (hitsStorage[i].distance<smallestDistance)
				{
					smallestDistanceIndex=i;
					smallestDistance = hitsStorage[i].distance;
				}
			}								
		}
		if (hitConnected)
		{
			// if the character is jumping onto a (1-way) platform but not high enough, we do nothing
			if (!basicMovementState.wasGroundedLastFrame && smallestDistance<rayBoundsRectangle.size.y/2)
			{
				basicMovementState.isCollidingBelow=false;
				return;
			}
			
			basicMovementState.isFalling=false;			
			basicMovementState.isCollidingBelow=true;
			
			newPosition.y = -Mathf.Abs(hitsStorage[smallestDistanceIndex].point.y - verticalRayCastFromLeft.y) 
				+ rayBoundsRectangle.height/2 
					+ rayOffset;
			
			if (externalForce.y>0)
			{
				newPosition.y += _speed.y * Time.deltaTime;
				basicMovementState.isCollidingBelow = false;
			}
			
			if (!basicMovementState.wasGroundedLastFrame && _speed.y>0)
			{
				newPosition.y += _speed.y * Time.deltaTime;
			}
			
			
			
			if (Mathf.Abs(newPosition.y)<smallValue)
				newPosition.y = 0;
			    
			// we check if the character is standing on a moving platform
			standingOn=hitsStorage[smallestDistanceIndex].collider.gameObject;

			/*IMover movingPlatform = (IMover)hitsStorage[smallestDistanceIndex].collider.GetComponentInChildren(typeof(IMover));
			basicMovementState.onAMovingPlatform=false;
			if (movingPlatform!=null)
			{
				movingPlatformsCurrentGravity=movingPlatformsGravity;
                basicMovementState.onAMovingPlatform=true;
				positionTransform.Translate(movingPlatform.GetSpeed()*Time.deltaTime);
				newPosition.y = 0;					
			}
			else
			{
				movingPlatformsCurrentGravity=0;
			}*/
		}
		else
		{
			movingPlatformsCurrentGravity=0;
			basicMovementState.isCollidingBelow=false;
		}	
		
		
	}
	
	/// <summary>
	/// If we're in the air and moving up, we cast rays above the character's head to check for collisions
	/// </summary>
	private void CastRaysAbove()
	{
		
		if (basicMovementState.isGrounded)
			return;
		
		float rayLength = basicMovementState.isGrounded?rayOffset : newPosition.y*Time.deltaTime;
		rayLength+=rayBoundsRectangle.height/2;
		
		bool hitConnected=false; 
		
		Vector2 verticalRayCastStart=new Vector2(rayBoundsRectangle.xMin+newPosition.x,
		                                         rayBoundsRectangle.center.y);	
		Vector2 verticalRayCastEnd=new Vector2(	rayBoundsRectangle.xMax+newPosition.x,
		                                       rayBoundsRectangle.center.y);	
		
		RaycastHit2D[] hitsStorage = new RaycastHit2D[numberOfVerticalRays];
		float smallestDistance=largeValue; 
		
		for (int i=0; i<numberOfVerticalRays;i++)
		{							
			Vector2 rayOriginPoint = Vector2.Lerp(verticalRayCastStart,verticalRayCastEnd,(float)i/(float)(numberOfVerticalRays-1));
			hitsStorage[i] = BasicMovementController.RayCast (rayOriginPoint,Vector2.up,rayLength,platformMask & ~edgeColliderPlatformMask,true,Color.green);	
			
			
			if (hitsStorage[i])
			{
				hitConnected=true;
				if (hitsStorage[i].distance<smallestDistance)
				{
					smallestDistance = hitsStorage[i].distance;
				}
			}	
			
		}	
		
		if (hitConnected)
		{
			_speed.y=0;
			newPosition.y = smallestDistance - rayBoundsRectangle.height/2   ;
			
			basicMovementState.isCollidingAbove=true;
			
			if (!basicMovementState.wasTouchingTheCeilingLastFrame)
			{
				newPosition.x=0;
				_speed = new Vector2(0, _speed.y);
			}
		}	
	}

    /// <summary>
    /// Draws a debug ray and does the actual raycast
    /// </summary>
    /// <returns>The cast.</returns>
    /// <param name="rayOriginPoint">Ray origin point.</param>
    /// <param name="rayDirection">Ray direction.</param>
    /// <param name="rayDistance">Ray distance.</param>
    /// <param name="mask">Mask.</param>
    /// <param name="debug">If set to <c>true</c> debug.</param>
    /// <param name="color">Color.</param>
    private static RaycastHit2D RayCast(Vector2 rayOriginPoint, Vector2 rayDirection, float rayDistance, LayerMask mask, bool debug, Color color)
    {
        Debug.DrawRay(rayOriginPoint, rayDirection * rayDistance, color);
        return Physics2D.Raycast(rayOriginPoint, rayDirection, rayDistance, mask);
    }


}