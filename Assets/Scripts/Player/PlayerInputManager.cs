using UnityEngine;
using System.Collections;

/// <summary>
/// This persistent singleton handles the inputs and sends commands to the player
/// </summary>
[RequireComponent(typeof(AdvancedMovementController))]
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(ActionBank))]
public class PlayerInputManager: MonoBehaviour
{
    public PlatformController platformController;

    protected IMover movingController;
    protected IJumper jumpingController;

    protected AdvancedMovementController playerMovementController;
    protected PlayerController playerController;
    protected ActionBank ActionBank;

    public bool controllingPlayer;

	/// <summary>
	/// We get the player from its tag.
	/// </summary>
	void Start()
	{
        movingController = GetComponent<AdvancedMovementController>();
        jumpingController = GetComponent<AdvancedMovementController>();
        playerMovementController = GetComponent<AdvancedMovementController>();
        playerController = GetComponent<PlayerController>();
        ActionBank = GetComponent<ActionBank>();
    }
    
    private PlayerInputState GetInputState()
    {
        return new PlayerInputState()
        {
            Horizontal = Input.GetAxis("Horizontal"),
            Vertical = Input.GetAxis("Vertical"),
            JumpDown = Input.GetButtonDown("Jump"),
            JumpUp = Input.GetButtonUp("Jump"),
            Broadcast = Input.GetAxis("Broadcast")
        };
    }

	/// <summary>
	/// At update, we check the various commands and send them to the player.
	/// </summary>
	void Update()
	{
        if (ActionBank.HasAction(TransmissionType.Jump))
            jumpingController = playerMovementController;
        else
            jumpingController = platformController;

        if (ActionBank.HasAction(TransmissionType.Move))
            movingController = playerMovementController;
        else
            movingController = platformController;

        PlayerInputState input = GetInputState();

        movingController.Move(input.Horizontal, input.Vertical);
        jumpingController.Jump(input.JumpDown, input.JumpUp);
        playerController.HandleInput(input);

    }

}