using UnityEngine;

[RequireComponent(typeof(Transmitter))]
public class PlayerController : MonoBehaviour
{
    protected Transmitter transmitter;
    protected AdvancedMovementController advancedMovementController;

    private void Awake()
    {
        transmitter = GetComponent<Transmitter>();
    }

    public void HandleInput(PlayerInputState inputState)
    {

    }
}
