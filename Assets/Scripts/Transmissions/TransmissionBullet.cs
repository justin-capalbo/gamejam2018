using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TransmissionBullet : MonoBehaviour
{
    public TransmissionType TransmissionType { get; set; }
    public Transmitter Sender { get; set; }

    public GameObject transmitEffect;

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
                // Spawn a feedback object
                Instantiate(transmitEffect,collision.transform.position,Quaternion.identity);

                Sender.SetForeignReceiver(TransmissionType, receiver);

                //Grant the appropriate action controller of the receive to the sender.
                if (TransmissionType == TransmissionType.Jump)
                    Sender.InputManager.JumpingController = (IJumper)receiver.gameObject.GetComponentInChildren(typeof(IJumper));

                if (TransmissionType == TransmissionType.Move)
                {
                    Sender.InputManager.MovingController.StopMovement();
                    Sender.InputManager.MovingController = (IMover)receiver.gameObject.GetComponentInChildren(typeof(IMover));
                }

                Destroy(gameObject);
            }
        }
    }
}
