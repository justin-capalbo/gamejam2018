using UnityEngine;
using System.Collections;

/// <summary>
/// This persistent singleton handles the inputs and sends commands to the player
/// </summary>
public class PlayerInputManager: MonoBehaviour
{
    public AdvancedMovementController playerController;
    public AdvancedMovementController platformController;

    public IMover movingController;
    public IJumper jumpingController;

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
            JumpUp = Input.GetButtonUp("Jump"),
            Broadcast = Input.GetAxis("Broadcast")
        };
    }

	/// <summary>
	/// At update, we check the various commands and send them to the player.
	/// </summary>
	void Update()
	{

        if(controllingPlayer)
        {
            movingController = this.GetComponent<AdvancedMovementController>();
            jumpingController = this.GetComponent<AdvancedMovementController>();
        }
        else
        {
            movingController = platformController;
            jumpingController = platformController;
        }

        PlayerInputState input = GetInputState();

        movingController.Move(input.Horizontal, input.Vertical);
        jumpingController.Jump(input.JumpDown, input.JumpUp);

        if (input.Broadcast > 0)
        {

        }

    }

    class PlayerInputState
    {
        public float Horizontal { get; set; }
        public float Vertical { get; set; }
        public bool JumpDown { get; set; }
        public bool JumpUp { get; set; }
        public float Broadcast { get; set; }
    }
}