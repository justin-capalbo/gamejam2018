using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class RecallBullet : MonoBehaviour
{
    public Receiver OwningReceiver { get; set; }
    public Transmitter OwningTransmitter { get; set; }

    private float lifetime;
    public float maxLife = 10f;

    private void Update()
    {
        lifetime += Time.deltaTime;

        if (lifetime > maxLife)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var targetReceiver = collision.GetComponentInChildren<Receiver>();
        if (targetReceiver)
        {
            if (targetReceiver.ActionBank.Count() > 0)
            {
                //Give the transmission back to the sender
                TransmissionType type = targetReceiver.ActionBank.GetFirst();
                if (OwningReceiver.CanReceive(type) && OwningReceiver.ReceiveTransmission(type))
                {
                    targetReceiver.ActionBank.RemoveAction(type);

                    //Grant the appropriate action controller of the receive to the sender.
                    if (type == TransmissionType.Jump)
                        OwningTransmitter.InputManager.JumpingController = OwningTransmitter.InputManager.PlayerMovementController;
                    
                    if (type == TransmissionType.Move)
                        OwningTransmitter.InputManager.MovingController = OwningTransmitter.InputManager.PlayerMovementController; 

                    Destroy(gameObject);
                }
            }
        }
    }
}
