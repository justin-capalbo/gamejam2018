using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TransmissionBullet : MonoBehaviour
{
    public TransmissionType TransmissionType { get; set; }
    public Transmitter Sender { get; set; }

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
        var receiver = collision.GetComponent<Receiver>();
        if (receiver)
        {
            if(Sender.TryTransmit(TransmissionType, receiver))
            {
                //Grant the appropriate action controller of the receive to the sender.
                if (TransmissionType == TransmissionType.Jump)
                    Sender.InputManager.JumpingController = (IJumper)receiver.gameObject.GetComponent(typeof(IJumper));

                if (TransmissionType == TransmissionType.Move)
                    Sender.InputManager.MovingController = (IMover)receiver.gameObject.GetComponent(typeof(IMover));

                Destroy(gameObject);
            }
        }
    }
}
