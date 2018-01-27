using UnityEngine;
using System.Collections;

public class PlayerInputState
{
    public float Horizontal { get; set; }
    public float Vertical { get; set; }
    public bool JumpDown { get; set; }
    public bool JumpUp { get; set; }
}

/// <summary>
/// This persistent singleton handles the inputs and sends commands to the player
/// </summary>
public class PlayerInputManager: MonoBehaviour
{
    public AdvancedMovementController playerController;

    public AdvancedMovementController movingController;
    public AdvancedMovementController jumpingController;

    public ActionBank ActionBank;

    public bool controllingPlayer;

	/// <summary>
	/// We get the player from its tag.
	/// </summary>
	void Start()
	{
        movingController = this.GetComponent<AdvancedMovementController>();
        jumpingController = this.GetComponent<AdvancedMovementController>();
	}
    
    private PlayerInputState GetInputState()
    {
        return new PlayerInputState()
        {
            Horizontal = Input.GetAxis("Horizontal"),
            Vertical = Input.GetAxis("Vertical"),
            JumpDown = Input.GetButtonDown("Jump"),
            JumpUp = Input.GetButtonUp("Jump")
        };
    }

	/// <summary>
	/// At update, we check the various commands and send them to the player.
	/// </summary>
	void Update()
	{
        PlayerInputState input = GetInputState();

        movingController.SetHorizontalMove(input.Horizontal);
        movingController.SetVerticalMove(input.Vertical);

        if (input.JumpDown)
        {
            jumpingController.JumpStart();
        }

        if(input.JumpUp)
        {
            jumpingController.JumpStop();
        }
    }	
}