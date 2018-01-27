using UnityEngine;
using System.Collections;

public class AdvancedMovementController : MonoBehaviour {



	/// the various states of the character
	public AdvancedMovementState advancedMovementState { get; private set; }
	/// the default parameters of the character
	public AdvancedMovementParameters defaultParameters;	
	/// the current behavior parameters (they can be overridden at times)
	public AdvancedMovementParameters currentParameters{get{return overrideParameters ?? defaultParameters;}}
	/// the permissions associated to the character
	public AdvancedMovementPermissions movementPermissions ;


	/// is true if the character can jump
	public bool jumpAuthorized 
	{ 
		get 
		{ 
			if ( (currentParameters.jumpRestrictions == AdvancedMovementParameters.JumpBehavior.CanJumpAnywhere) ||  (currentParameters.jumpRestrictions == AdvancedMovementParameters.JumpBehavior.CanJumpAnywhereAnyNumberOfTimes) )
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
	// true if the player is facing right
	private bool isFacingRight=true;
	// storage for original gravity and timer
	private float originalGravity;

	// the current normalized horizontal speed
	private float normalizedHorizontalSpeed;

	// pressure timed jumps
	private float jumpButtonPressTime = 0;
	private bool jumpButtonPressed=false;
	private bool jumpButtonReleased=false;

	// input axis
	private float horizontalMove;
	private float verticalMove;





	/// <summary>
	///Initializes this instance of the character
	/// </summary>
	void Awake()
	{		
		advancedMovementState = new AdvancedMovementState();	
		basicMovementController = GetComponent<BasicMovementController>();

        //_sceneCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();

    }

	/// <summary>
	/// Further initialization
	/// </summary>
    public virtual void Start()
	{
		// we get the animator
		animatorReference = this.gameObject.GetComponent<Animator>();

		// if the width of the character is positive, then it is facing right.
		isFacingRight = transform.localScale.x > 0;
		
		originalGravity = basicMovementController.currentParameters.gravity;
		
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
			
			
		// ladder climbing and wall clinging
		ClimbLadder();
		WallClinging ();
			
		// If the character is dashing, we cancel the gravity
		if (advancedMovementState.dashing) 
		{
			GravityActive(false);
			basicMovementController.SetVerticalForce(0);
		}	

		// if the character can jump we handle press time controlled jumps
		if (jumpAuthorized)
		{				
			// If the user releases the jump button and the character is jumping up and enough time since the initial jump has passed, then we make it stop jumping by applying a force down.
			if ( (jumpButtonPressTime!=0) 
				&& (Time.time - jumpButtonPressTime >= currentParameters.jumpMinimumAirTime) 
			&& (basicMovementController.speed.y > Mathf.Sqrt(Mathf.Abs(basicMovementController.currentParameters.gravity))) 
				&& (jumpButtonReleased)
				&& (!jumpButtonPressed||advancedMovementState.jetpacking))
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
		
		// If the value of the horizontal axis is positive, the character must face right.
		if (horizontalMove>0.1f)
		{
			normalizedHorizontalSpeed = horizontalMove;

			if (!isFacingRight)
			{
				Flip();
			}
		}		
		// If it's negative, then we're facing left
		else if (horizontalMove<-0.1f)
		{
			normalizedHorizontalSpeed = horizontalMove;

			if (isFacingRight)
			{
				Flip();
			}
		}
		else
		{
			normalizedHorizontalSpeed=0;
		}
		
		// we pass the horizontal force that needs to be applied to the controller.
		var movementFactor = basicMovementController.basicMovementState.isGrounded ? basicMovementController.currentParameters.groundAcceleration : basicMovementController.currentParameters.airAcceleration;

		if (currentParameters.smoothMovement) 
		{
			basicMovementController.SetHorizontalForce (Mathf.Lerp (basicMovementController.speed.x, normalizedHorizontalSpeed * currentParameters.movementSpeed, Time.deltaTime * movementFactor));
		} 
		else 
		{
			basicMovementController.SetHorizontalForce (normalizedHorizontalSpeed * currentParameters.movementSpeed);
		}
	}


	/// <summary>
	/// Called at Update(), handles vertical movement
	/// </summary>
	private void VerticalMovement()
	{
		
		// Manages the ground touching effect
		if (basicMovementController.basicMovementState.justGotGrounded)
		{
			//handle touch ground effects	
		}
		
		// if the character is not in a position where it can move freely, we do nothing.
		if (!advancedMovementState.canMoveFreely) 
		{
			return;
		}
		
		// Crouch Detection : if the player is pressing "down" and if the character is grounded and the crouch action is enabled
		if ( (verticalMove<-0.1) && (basicMovementController.basicMovementState.isGrounded) && (movementPermissions.crouchEnabled) )
		{
			advancedMovementState.crouching = true;
			currentParameters.movementSpeed = currentParameters.crouchSpeed;
			advancedMovementState.running=false;
		}
		else
		{	

			if (!advancedMovementState.running)
			{
				currentParameters.movementSpeed = currentParameters.walkSpeed;
			}
			advancedMovementState.crouching = false;
			advancedMovementState.canJump=true;
				

		}
		
		if (advancedMovementState.crouchingPreviously != advancedMovementState.crouching)
		{
			Invoke ("RecalculateRays",Time.deltaTime*10);		
		}
		
		advancedMovementState.crouchingPreviously=advancedMovementState.crouching;
		
		
	}

	
	/// <summary>
	/// Use this method to force the controller to recalculate the rays, especially useful when the size of the character has changed.
	/// </summary>
	public void RecalculateRays()
	{
		basicMovementController.SetRaysParameters();
	}


	/// <summary>
	/// Causes the character to start running.
	/// </summary>
	public void RunStart()
	{		
		// if the Run action is enabled in the permissions, we continue, if not we do nothing
		if (!movementPermissions.runEnabled)
			return;

		// if the character is not in a position where it can move freely, we do nothing.
		if (!advancedMovementState.canMoveFreely)
			return;
		
		// if the player presses the run button and if we're on the ground and not crouching and we can move freely, 
		// then we change the movement speed in the controller's parameters.
		if (basicMovementController.basicMovementState.isGrounded && !advancedMovementState.crouching)
		{
			currentParameters.movementSpeed = currentParameters.runSpeed;
			advancedMovementState.running=true;
		}
	}
	
	/// <summary>
	/// Causes the character to stop running.
	/// </summary>
	public void RunStop()
	{
		// if the run button is released, we revert back to the walking speed.
		currentParameters.movementSpeed = currentParameters.walkSpeed;
		advancedMovementState.running=false;
	}

	/// <summary>
	/// Causes the character to start jumping.
	/// </summary>
	public void JumpStart()
	{
		
		// if the Jump action is enabled in the permissions, we continue, if not we do nothing. If the player is dead, we do nothing.
		if (!movementPermissions.jumpEnabled  || !jumpAuthorized)
			return;
		
		// we check if the character can jump without conflicting with another action
		if (basicMovementController.basicMovementState.isGrounded 
			|| advancedMovementState.ladderClimbing 
			|| advancedMovementState.wallClinging 
			|| advancedMovementState.numberOfJumpsLeft > 0)
		{
			advancedMovementState.canJump = true;
		} 
		else 
		{
			advancedMovementState.canJump = false;
		}
		
		// if the player can't jump, we do nothing. 
		if ( (!advancedMovementState.canJump) && !(currentParameters.jumpRestrictions==AdvancedMovementParameters.JumpBehavior.CanJumpAnywhereAnyNumberOfTimes) )
			return;
		
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
		advancedMovementState.ladderClimbing=false;
		advancedMovementState.canMoveFreely=true;
		GravityActive(true);
		
		jumpButtonPressTime=Time.time;
		jumpButtonPressed=true;
		jumpButtonReleased=false;
		
		basicMovementController.SetVerticalForce(Mathf.Sqrt( 2f * currentParameters.jumpHeight * Mathf.Abs(basicMovementController.currentParameters.gravity) ));
		
		// we play the jump sound
		//if (PlayerJumpSfx!=null)
		//	SoundManager.Instance.PlaySound(PlayerJumpSfx,transform.position);
		
		// wall jump
		float wallJumpDirection;
		if (advancedMovementState.wallClinging)
		{
			
			// If the character is colliding to the right with something (probably the wall)
			if (basicMovementController.basicMovementState.isCollidingRight)
			{
				wallJumpDirection=-1f;
			}
			else
			{					
				wallJumpDirection=1f;
			}
			basicMovementController.SetForce(new Vector2(wallJumpDirection*currentParameters.wallJumpForce,Mathf.Sqrt( 2f * currentParameters.jumpHeight * Mathf.Abs(basicMovementController.currentParameters.gravity) )));
			advancedMovementState.wallClinging=false;
		}	
		
	}
	
	/// <summary>
	/// Causes the character to stop jumping.
	/// </summary>
	public void JumpStop()
	{
		jumpButtonPressed=false;
		jumpButtonReleased=true;
	}

	/// <summary>
	/// Called at Update(), handles the climbing of ladders
	/// </summary>	
	void ClimbLadder()
	{
		// if the character is colliding with a ladder
		if (advancedMovementState.ladderColliding)
		{
			// if the player is pressing the up key and not yet climbing a ladder, and not colliding with the top platform and not jetpacking
			if (verticalMove>0.1 && !advancedMovementState.ladderClimbing && !advancedMovementState.ladderTopColliding  && !advancedMovementState.jetpacking)
			{			
				// then the character starts climbing
				advancedMovementState.ladderClimbing=true;
				basicMovementController.CollisionsOn();
				
				// it can't move freely anymore
				advancedMovementState.canMoveFreely=false;

				// we initialize the ladder climbing speed to zero
				advancedMovementState.ladderClimbingSpeed=0;
				// we make sure the controller won't move
				basicMovementController.SetHorizontalForce(0);
				basicMovementController.SetVerticalForce(0);
				// we disable the gravity
				GravityActive(false);
			}			
			
			// if the character is climbing the ladder (which means it previously connected with it)
			if (advancedMovementState.ladderClimbing)
			{

				// we disable the gravity
				GravityActive(false);
				
				if (!advancedMovementState.ladderTopColliding)
					basicMovementController.CollisionsOn();
				
				// we set the vertical force according to the ladder climbing speed
				basicMovementController.SetVerticalForce(verticalMove * currentParameters.ladderSpeed);
				// we set pass that speed to the climbing speed state.
				advancedMovementState.ladderClimbingSpeed=Mathf.Abs(verticalMove);				
			}
			
			if (!advancedMovementState.ladderTopColliding)
			{
				basicMovementController.CollisionsOn();
			}
			
			// if the character is grounded AND climbing
			if (advancedMovementState.ladderClimbing && basicMovementController.basicMovementState.isGrounded && !advancedMovementState.ladderTopColliding)
			{			
				// we make it stop climbing, it has reached the ground.
				advancedMovementState.ladderColliding=false;
				advancedMovementState.ladderClimbing=false;
				advancedMovementState.canMoveFreely=true;
				advancedMovementState.ladderClimbingSpeed=0;	
				GravityActive(true);			
			}			
		}
		
		// If the character is colliding with the top of the ladder and is pressing down and is not on the ladder yet and is standing on the ground, we make it go down.
		if (advancedMovementState.ladderTopColliding && verticalMove<-0.1 && !advancedMovementState.ladderClimbing && basicMovementController.basicMovementState.isGrounded)
		{			
			basicMovementController.CollisionsOff();
			// we force its position to be a bit lower
			transform.position=new Vector2(transform.position.x,transform.position.y-0.1f);
			// we initiate the climbing.
			advancedMovementState.ladderClimbing=true;
			advancedMovementState.canMoveFreely=false;
			advancedMovementState.ladderClimbingSpeed=0;			
			basicMovementController.SetHorizontalForce(0);
			basicMovementController.SetVerticalForce(0);
			GravityActive(false);
		}		
	}

	/// <summary>
	/// Causes the character to dash or dive (depending on the vertical movement at the start of the dash)
	/// </summary>
	public void Dash()
	{	
		// declarations	
		float _dashDirection;
		float _boostForce;
		
		// if the Dash action is enabled in the permissions, we continue, if not we do nothing
		if (!movementPermissions.dashEnabled )
			return;

		// if the character is not in a position where it can move freely, we do nothing.
		if (!advancedMovementState.canMoveFreely)
			return;
		
		
		// If the user presses the dash button and is not aiming down
		if (verticalMove>-0.8) 
		{	
			// if the character is allowed to dash
			if (advancedMovementState.canDash)
			{
				// we set its dashing state to true
				advancedMovementState.dashing=true;
				
				// depending on its direction, we calculate the dash parameters to apply				
				if (isFacingRight) { _dashDirection=1f; } else { _dashDirection = -1f; }

				_boostForce=_dashDirection*currentParameters.dashForce;
				advancedMovementState.canDash = false;

				// we launch the boost corountine with the right parameters
				StartCoroutine( Boost(currentParameters.dashDuration,_boostForce,0,"dash") );
			}			
		}
		// if the user presses the dash button and is aiming down
		if (verticalMove<-0.8) 
		{
			basicMovementController.CollisionsOn();
			// we start the dive coroutine
			StartCoroutine(Dive());
		}		
		
	}
	
	/// <summary>
	/// Coroutine used to move the player in a direction over time
	/// </summary>
	IEnumerator Boost(float boostDuration, float boostForceX, float boostForceY, string name) 
	{
		float time = 0f; 
		
		// for the whole duration of the boost
		while(boostDuration > time) 
		{
			// we add the force passed as a parameter
			if (boostForceX!=0)
			{
				basicMovementController.AddForce(new Vector2(boostForceX,0));
			}
			if (boostForceY!=0)
			{
				basicMovementController.AddForce(new Vector2(0,boostForceY));
			}
			time+=Time.deltaTime;
			// we keep looping for the duration of the boost
			yield return 0; 
		}
		// once the boost is complete, if we were dashing, we make it stop and start the dash cooldown
		if (name=="dash")
		{
			advancedMovementState.dashing=false;
			GravityActive(true);
			yield return new WaitForSeconds(currentParameters.dashCooldown); 
			advancedMovementState.canDash = true; 
		}	
		if (name=="wallJump")
		{
			// so far we do nothing, but you could use it to trigger a sound or an effect when walljumping
		}		
	}
	
	/// <summary>
	/// Coroutine used to make the player dive vertically
	/// </summary>
	IEnumerator Dive()
	{	
		// Shake parameters : intensity, duration (in seconds) and decay
		Vector3 ShakeParameters = new Vector3(1.5f,0.5f,1f);
		advancedMovementState.diving=true;

		// while the player is not grounded, we force it to go down fast
		while (!basicMovementController.basicMovementState.isGrounded)
		{
			basicMovementController.SetVerticalForce(-Mathf.Abs(basicMovementController.currentParameters.gravity)*2);
			yield return 0; //go to next frame
		}
		
		// once the player is grounded, we shake the camera, and restore the diving state to false
		//_sceneCamera.Shake(ShakeParameters);		
		advancedMovementState.diving=false;
	}

	/// <summary>
	/// Flips the character and its dependencies (jetpack for example) horizontally
	/// </summary>
	protected virtual void Flip()
	{
		// Flips the character horizontally
		transform.localScale = new Vector3(-transform.localScale.x,transform.localScale.y,transform.localScale.z);
		isFacingRight = transform.localScale.x > 0;

	}

	/// <summary>
	/// Makes the player stick to a wall when jumping
	/// </summary>
	private void WallClinging()
	{
		// if the wall clinging action is enabled in the permissions, we continue, if not we do nothing
		if (!movementPermissions.wallClingingEnabled)
			return;
		
		if (!basicMovementController.basicMovementState.isCollidingLeft && !basicMovementController.basicMovementState.isCollidingRight)
		{
			advancedMovementState.wallClinging=false;
		}
		
		// if the character is not in a position where it can move freely, we do nothing.
		if (!advancedMovementState.canMoveFreely)
			return;
		
		// if the character is in the air and touching a wall and moving in the opposite direction, then we slow its fall
		
		if((!basicMovementController.basicMovementState.isGrounded) && ( ( (basicMovementController.basicMovementState.isCollidingRight) && (horizontalMove>0.1f) )	|| 	( (basicMovementController.basicMovementState.isCollidingLeft) && (horizontalMove<-0.1f) )	))
		{
			if (basicMovementController.speed.y<0)
			{
				advancedMovementState.wallClinging=true;
				basicMovementController.SlowFall(currentParameters.wallClingingSlowFactor);
			}
		}
		else
		{
			advancedMovementState.wallClinging=false;
			basicMovementController.SlowFall(0f);
		}
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
		
		AdvancedMovementController.UpdateAnimatorBool(animatorReference,"Grounded",basicMovementController.basicMovementState.isGrounded);
        AdvancedMovementController.UpdateAnimatorFloat(animatorReference,"Speed",Mathf.Abs(basicMovementController.speed.x));
        AdvancedMovementController.UpdateAnimatorFloat(animatorReference,"vSpeed",basicMovementController.speed.y);
        AdvancedMovementController.UpdateAnimatorBool(animatorReference,"Running",advancedMovementState.running);
        AdvancedMovementController.UpdateAnimatorBool(animatorReference,"Dashing",advancedMovementState.dashing);
        AdvancedMovementController.UpdateAnimatorBool(animatorReference,"Crouching",advancedMovementState.crouching);
        AdvancedMovementController.UpdateAnimatorBool(animatorReference,"WallClinging",advancedMovementState.wallClinging);
        AdvancedMovementController.UpdateAnimatorBool(animatorReference,"Diving",advancedMovementState.diving);
        AdvancedMovementController.UpdateAnimatorBool(animatorReference, "LadderClimbing", advancedMovementState.ladderClimbing);
        AdvancedMovementController.UpdateAnimatorFloat(animatorReference, "LadderClimbingSpeed", advancedMovementState.ladderClimbingSpeed);
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
