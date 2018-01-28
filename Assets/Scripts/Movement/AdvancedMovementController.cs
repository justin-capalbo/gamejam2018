using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BasicMovementController))]
public class AdvancedMovementController : MonoBehaviour, IMover, IJumper {

	/// the various states of the character
	public AdvancedMovementState advancedMovementState { get; private set; }
	/// the default parameters of the character
	public AdvancedMovementParameters defaultParameters;	
	/// the current behavior parameters (they can be overridden at times)
	public AdvancedMovementParameters currentParameters{get{return overrideParameters ?? defaultParameters;}}
	/// the permissions associated to the character
	public AdvancedMovementPermissions movementPermissions ;

    public AudioClip jumpSound;

    /// is true if the character can jump
    public bool jumpAuthorized 
	{ 
		get 
		{ 
			if ((currentParameters.jumpRestrictions == AdvancedMovementParameters.JumpBehavior.CanJumpAnywhere) ||  (currentParameters.jumpRestrictions == AdvancedMovementParameters.JumpBehavior.CanJumpAnywhereAnyNumberOfTimes) )
				return true;
			
			if (currentParameters.jumpRestrictions == AdvancedMovementParameters.JumpBehavior.CanJumpOnGround)
				return basicMovementController.basicMovementState.isGrounded;
			
			return false; 
		}
	}

	//reference to basic movement controller
	protected BasicMovementController basicMovementController;
	// storage for overriding behavior parameters
	private AdvancedMovementParameters overrideParameters;
	//animator
	private Animator animatorReference;
	// storage for original gravity and timer
	private float originalGravity;

	// the current normalized horizontal speed
	private float normalizedHorizontalSpeed;
    private float normalizedVerticalSpeed;

	// pressure timed jumps
	private float jumpButtonPressTime = 0;
	private bool jumpButtonPressed=false;
	private bool jumpButtonReleased=false;

	// input axis
	private float horizontalMove;
	private float verticalMove;

    private bool isFacingRight = true;


    /// <summary>
    ///Initializes this instance of the character
    /// </summary>
    void Awake()
	{		
		advancedMovementState = new AdvancedMovementState();	
		basicMovementController = GetComponent<BasicMovementController>();
        currentParameters.hMovementSpeed = currentParameters.walkSpeed;

        //_sceneCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();

    }

	/// <summary>
	/// Further initialization
	/// </summary>
    public virtual void Start()
	{
		// we get the animator
		animatorReference = this.gameObject.GetComponent<Animator>();

		originalGravity = basicMovementController.currentParameters.gravity;
        isFacingRight = transform.localScale.x > 0;

        // we initialize all the controller's states with their default values.
        advancedMovementState.Initialize();
		advancedMovementState.numberOfJumpsLeft=currentParameters.numberOfJumps;
		
		advancedMovementState.canJump=true;		
	}

	/// <summary>
	/// This is called every frame.
	/// </summary>
	protected virtual void Update()
	{        		
		// we send our various states to the animator.		
		UpdateAnimator ();

        //make sure gravity is active
		GravityActive(true);
			
		// we handle horizontal and vertical movement				
		HorizontalMovement();
		VerticalMovement();			

		// if the character can jump we handle press time controlled jumps
		if (jumpAuthorized)
		{				
			// If the user releases the jump button and the character is jumping up and enough time since the initial jump has passed, then we make it stop jumping by applying a force down.
			if ( (jumpButtonPressTime!=0) 
				&& (Time.time - jumpButtonPressTime >= currentParameters.jumpMinimumAirTime) 
			&& (basicMovementController.speed.y > Mathf.Sqrt(Mathf.Abs(basicMovementController.currentParameters.gravity))) 
				&& (jumpButtonReleased)
				&& (!jumpButtonPressed))
			{
				jumpButtonReleased=false;	
				if (currentParameters.jumpIsProportionalToThePressTime)					
				basicMovementController.AddForce(new Vector2(0,12 * -Mathf.Abs(basicMovementController.currentParameters.gravity) * Time.deltaTime ));			
			}
		}			

	}

	/// <summary>
	/// This is called once per frame, after Update();
	/// </summary>
	void LateUpdate()
	{
		// if the character became grounded this frame, we reset the doubleJump flag so he can doubleJump again
		if (basicMovementController.basicMovementState.justGotGrounded)
		{
			advancedMovementState.numberOfJumpsLeft=currentParameters.numberOfJumps;		
		}		
	}

    /** MOVEMENT **/

    public void Move(float moveH, float moveV)
    {
        if (!movementPermissions.hMoveEnabled) return;

        SetHorizontalMove(moveH);
        SetVerticalMove(moveV);
    }

    /// <summary>
    /// Sets the horizontal move value.
    /// </summary>
    /// <param name="value">Horizontal move value, between -1 and 1 - positive : will move to the right, negative : will move left </param>
    public void SetHorizontalMove(float value)
	{
		horizontalMove=value;
	}
	
	/// <summary>
	/// Sets the vertical move value.
	/// </summary>
	/// <param name="value">Vertical move value, between -1 and 1
	public void SetVerticalMove(float value)
	{
		verticalMove=value;
	}
    
    /// <summary>
	 /// Called at Update(), handles horizontal movement
	 /// </summary>
	private void HorizontalMovement()
	{		
		// if movement is prevented, we exit and do nothing
		if (!advancedMovementState.canMoveFreely) 
		{
			return;		
		}

        if (!movementPermissions.hMoveEnabled) return;

		// If the value of the horizontal axis is positive, the character must face right.
		if (horizontalMove>0.1f)
		{
			normalizedHorizontalSpeed = horizontalMove;
            animatorReference.SetBool("isMoving", true);

            if (!isFacingRight)
            {
                Flip();
            }
        }		
		// If it's negative, then we're facing left
		else if (horizontalMove<-0.1f)
		{
			normalizedHorizontalSpeed = horizontalMove;
            animatorReference.SetBool("isMoving", true);

            if (isFacingRight)
            {
                Flip();
            }
        }
		else
		{
			normalizedHorizontalSpeed=0;
            animatorReference.SetBool("isMoving", false);
        }
	
	    basicMovementController.SetHorizontalForce (normalizedHorizontalSpeed * currentParameters.hMovementSpeed);
		
	}
    
	/// <summary>
	/// Called at Update(), handles vertical movement
	/// </summary>
	private void VerticalMovement()
	{		
		// Manages the ground touching effect
		if (basicMovementController.basicMovementState.justGotGrounded)
		{
            print("just got grounded");
			//handle touch ground effects	
		}

        animatorReference.SetBool("isFalling", false);
        if (basicMovementController.speed.y < 0 && !basicMovementController.basicMovementState.isGrounded)
        {
            animatorReference.SetBool("isJumping", false);
            animatorReference.SetBool("isFalling", true);
        }
		
		// if the character is not in a position where it can move freely, we do nothing.
		if (!advancedMovementState.canMoveFreely) 
		{
			return;
		}

        if (!movementPermissions.vMoveEnabled) return;

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

        basicMovementController.SetVerticalForce(normalizedVerticalSpeed * currentParameters.vMovementSpeed);
    }

    private void StopMovement()
    {
        basicMovementController.SetHorizontalForce(0);
        basicMovementController.SetVerticalForce(0);
        animatorReference.SetBool("isMoving", false);
    }

    /** JUMPING **/

    public bool CanJump()
    {
        //<<OAKWOOD ADDED>>
        if (!advancedMovementState.canMoveFreely)
            return false;

        // if the Jump action is enabled in the permissions, we continue, if not we do nothing. If the player is dead, we do nothing.
        if (!movementPermissions.jumpEnabled || !jumpAuthorized)
            return false;

        // we check if the character can jump without conflicting with another action
        if ((basicMovementController.basicMovementState.isGrounded
            || advancedMovementState.numberOfJumpsLeft > 0) &&
            !advancedMovementState.broadcasting &&
            !advancedMovementState.recalling)
        {
            advancedMovementState.canJump = true;
        }
        else
        {
            advancedMovementState.canJump = false;
        }

        // if the player can't jump, we do nothing. 
        if ((!advancedMovementState.canJump) && !(currentParameters.jumpRestrictions == AdvancedMovementParameters.JumpBehavior.CanJumpAnywhereAnyNumberOfTimes))
            return false;

        return true;
    }

    /// <summary>
    /// Causes the character to start jumping.
    /// </summary>
    public void StartJump()
	{
        if (!advancedMovementState.canMoveFreely) return;

		// if the character is standing on a one way platform and is also pressing the down button,
		if (verticalMove<0 && basicMovementController.basicMovementState.isGrounded)
		{
			if (basicMovementController.standingOn.layer==LayerMask.NameToLayer("OneWayPlatforms"))
			{
				// we make it fall down below the platform by moving it just below the platform
				basicMovementController.transform.position=new Vector2(transform.position.x,transform.position.y-0.1f);
				// we turn the boxcollider off for a few milliseconds, so the character doesn't get stuck mid platform
				StartCoroutine(basicMovementController.DisableCollisions(0.3f));
				basicMovementController.ResetMovingPlatformsGravity();
				return;
			}
		}
		
		// if the character is standing on a moving platform and not pressing the down button,
		if (verticalMove>=0 && basicMovementController.basicMovementState.isGrounded)
		{
			if (basicMovementController.standingOn.layer
			     ==LayerMask.NameToLayer("MovingPlatforms"))
			{
				// we turn the boxcollider off for a few milliseconds, so the character doesn't get stuck mid air
				StartCoroutine(basicMovementController.DisableCollisions(0.3f));
				basicMovementController.ResetMovingPlatformsGravity();
			}
		}
		
		// we decrease the number of jumps left
		advancedMovementState.numberOfJumpsLeft=advancedMovementState.numberOfJumpsLeft-1;
		advancedMovementState.canMoveFreely=true;
		GravityActive(true);
		
		jumpButtonPressTime=Time.time;
		jumpButtonPressed=true;
		jumpButtonReleased=false;
        animatorReference.SetBool("isJumping", true);
		basicMovementController.SetVerticalForce(Mathf.Sqrt( 2f * currentParameters.jumpHeight * Mathf.Abs(basicMovementController.currentParameters.gravity) ));	
        AudioSource.PlayClipAtPoint(jumpSound,transform.position);
	}
	
	/// <summary>
	/// Causes the character to stop jumping.
	/// </summary>
	public void StopJump()
	{
		jumpButtonPressed=false;
		jumpButtonReleased=true;
	}

    protected virtual void Flip()
 	{
 		// Flips the character horizontally
 		transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
 		isFacingRight = transform.localScale.x > 0;
 
 	}

/// <summary>
/// Activates or desactivates the gravity for this character only.
/// </summary>
/// <param name="state">If set to <c>true</c>, activates the gravity. If set to <c>false</c>, turns it off.</param>
private void GravityActive(bool state)
	{
		if (state==true)
		{
			if (basicMovementController.currentParameters.gravity==0)
			{
				basicMovementController.currentParameters.gravity = originalGravity;
			}
		}
		else
		{
			if (basicMovementController.currentParameters.gravity!=0)
				originalGravity = basicMovementController.currentParameters.gravity;
			basicMovementController.currentParameters.gravity = 0;
		}
	}

	/// <summary>
	/// This is called at Update() and sets each of the animators parameters to their corresponding State values
	/// </summary>
	private void UpdateAnimator()
	{
        //AdvancedMovementController.UpdateAnimatorBool(animatorReference, "Grounded", basicMovementController.basicMovementState.isGrounded);
        //AdvancedMovementController.UpdateAnimatorFloat(animatorReference, "Speed", Mathf.Abs(basicMovementController.speed.x));
        //AdvancedMovementController.UpdateAnimatorFloat(animatorReference, "vSpeed", basicMovementController.speed.y);
        //AdvancedMovementController.UpdateAnimatorBool(animatorReference, "Running", advancedMovementState.running);
        //AdvancedMovementController.UpdateAnimatorBool(animatorReference, "Dashing", advancedMovementState.dashing);
        //AdvancedMovementController.UpdateAnimatorBool(animatorReference, "Crouching", advancedMovementState.crouching);
        //AdvancedMovementController.UpdateAnimatorBool(animatorReference, "WallClinging", advancedMovementState.wallClinging);
        //AdvancedMovementController.UpdateAnimatorBool(animatorReference, "Diving", advancedMovementState.diving);
        //AdvancedMovementController.UpdateAnimatorBool(animatorReference, "LadderClimbing", advancedMovementState.ladderClimbing);
        //AdvancedMovementController.UpdateAnimatorFloat(animatorReference, "LadderClimbingSpeed", advancedMovementState.ladderClimbingSpeed);
    }

    public void Broadcast(float broadcast)
    {
        if ((broadcast > 0) && (basicMovementController.basicMovementState.isGrounded) && (movementPermissions.broadcastEnabled))
        {
            StopMovement(); //<<OAKWOOD ADDED>>
            advancedMovementState.broadcasting = true;
            currentParameters.hMovementSpeed = currentParameters.broadcastWalkSpeed;
            movementPermissions.jumpEnabled = false;
            animatorReference.SetBool("isBroadcasting", true);
        }
        else
        {
            currentParameters.hMovementSpeed = currentParameters.walkSpeed;
            advancedMovementState.broadcasting = false;
            movementPermissions.jumpEnabled = true;
            animatorReference.SetBool("isBroadcasting", false);
        }

        advancedMovementState.broadcastingPreviously = advancedMovementState.broadcasting; 
    }

    public void Recall(float recall)
    {
        if ((recall > 0) && (basicMovementController.basicMovementState.isGrounded) && (movementPermissions.broadcastEnabled))
        {
            advancedMovementState.canMoveFreely = false;
            StopMovement(); //<<OAKWOOD ADDED>>
            advancedMovementState.recalling = true;
            currentParameters.hMovementSpeed = currentParameters.broadcastWalkSpeed;
            movementPermissions.jumpEnabled = false;
        }
        else
        {
            advancedMovementState.canMoveFreely = true;
            currentParameters.hMovementSpeed = currentParameters.walkSpeed;
            advancedMovementState.recalling = false;
            movementPermissions.jumpEnabled = true;
            advancedMovementState.canJump = true;
        }

        advancedMovementState.recallingPreviously = advancedMovementState.recalling;
    }

    public void Jump(bool jumpPress, bool jumpRelease)
    {        
        if (!movementPermissions.jumpEnabled) return;

        if (jumpPress && CanJump()) {
            StartJump();            
        }

        if (jumpRelease) {
            StopJump();
        }
    }

    public static void UpdateAnimatorBool(Animator animator, string parameterName,bool value)
	{
		//if (animator.HasParameterOfType (parameterName, AnimatorControllerParameterType.Bool))
			animator.SetBool(parameterName,value);
	}
	public static void UpdateAnimatorFloat(Animator animator, string parameterName,float value)
	{
		//if (animator.HasParameterOfType (parameterName, AnimatorControllerParameterType.Float))
			animator.SetFloat(parameterName,value);
	}
	public static void UpdateAnimatorInteger(Animator animator, string parameterName,int value)
	{
		//if (animator.HasParameterOfType (parameterName, AnimatorControllerParameterType.Int))
			animator.SetInteger(parameterName,value);
	}

}
