using UnityEngine;
using System.Collections;

/// <summary>
/// This persistent singleton handles the inputs and sends commands to the player
/// </summary>
[RequireComponent(typeof(AdvancedMovementController))]
[RequireComponent(typeof(ActionBank))]
public class PlayerInputManager: MonoBehaviour
{
    [HideInInspector]
    public IMover MovingController { get; set; }
    [HideInInspector]
    public IJumper JumpingController { get; set; }

    [HideInInspector]
    public AdvancedMovementController PlayerMovementController;
    protected ActionBank ActionBank;
    protected Transmitter Transmitter;

	/// <summary>
	/// We get the player from its tag.
	/// </summary>
	void Start()
	{
        MovingController = GetComponent<AdvancedMovementController>();
        JumpingController = GetComponent<AdvancedMovementController>();
        PlayerMovementController = GetComponent<AdvancedMovementController>();
        Transmitter = GetComponent<Transmitter>();
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
            Broadcast = Input.GetAxis("Broadcast"),
            TransmitMove = Input.GetButtonDown("TransmitMove"),
            Recall = Input.GetAxis("Recall")            
        };
    }

	/// <summary>
	/// At update, we check the various commands and send them to the player.
	/// </summary>
	void Update()
	{
        PlayerInputState input = GetInputState();

        //Pass input to relevant controllers
        
        if (!PlayerMovementController.advancedMovementState.broadcasting)
        {
            MovingController.Move(input.Horizontal, input.Vertical);            

            if (!PlayerMovementController.advancedMovementState.recalling)
                JumpingController.Jump(input.JumpDown, input.JumpUp);
        }

        PlayerMovementController.Recall(input.Recall);
        Transmitter.HandleRecall(input, PlayerMovementController.advancedMovementState.recalling);

        PlayerMovementController.Broadcast(input.Broadcast);
        Transmitter.HandleBroadcast(input, PlayerMovementController.advancedMovementState.broadcasting);

        if(Input.GetButtonDown("Help"))
        {
            if(GameController.S.controlsText.enabled)
            {
                GameController.S.controlsText.enabled = false;
                GameController.S.helpText.enabled = true;
            }
            else
            {
                GameController.S.controlsText.enabled = true;
                GameController.S.helpText.enabled = false;
            }
        }
    }

}