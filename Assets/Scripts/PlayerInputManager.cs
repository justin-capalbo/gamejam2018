using UnityEngine;
using System.Collections;

/// <summary>
/// This persistent singleton handles the inputs and sends commands to the player
/// </summary>
public class PlayerInputManager: MonoBehaviour
{
	private static AdvancedMovementController advancedMovementController;
	private static BasicMovementController basicMovementController;
	
	/// <summary>
	/// We get the player from its tag.
	/// </summary>
	void Start()
	{
		basicMovementController = GetComponent<BasicMovementController>();
		advancedMovementController = GetComponent<AdvancedMovementController>();
	}
    
	/// <summary>
	/// At update, we check the various commands and send them to the player.
	/// </summary>
	void Update()
	{		
		advancedMovementController.SetHorizontalMove(Input.GetAxis ("Horizontal"));
		advancedMovementController.SetVerticalMove(Input.GetAxis ("Vertical"));
		
		if ((Input.GetButtonDown ("Run") || Input.GetButton ("Run"))) 
		{
			advancedMovementController.RunStart();		
		}
		
		if (Input.GetButtonUp ("Run")) 
		{
			advancedMovementController.RunStop();	
		}
		
		if (Input.GetButtonDown ("Jump")) 
		{
     		advancedMovementController.JumpStart();
		}
		
		if (Input.GetButtonUp("Jump"))
		{
			advancedMovementController.JumpStop();
		}
		
		if (Input.GetButtonDown ("Dash"))
		{
			advancedMovementController.Dash();
		}
	}	
}