using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using System;

public enum TransmissionType { Move, Jump }

[RequireComponent(typeof(ActionBank))]
[RequireComponent(typeof(Receiver))]
[RequireComponent(typeof(PlayerInputManager))]
public class Transmitter : MonoBehaviour
{
    public ActionBank MyActionBank { get; set; }
    public Receiver MyReceiver { get; set; }
    public PlayerInputManager InputManager { get; set; }

    public TransmissionBullet transmissionProjectile;
    public RecallBullet recallProjectile;

    public float TransmitCooldown = 1f;
    private float RemainingCD = 0f;

    public void Awake()
    {
        MyActionBank = GetComponent<ActionBank>();
        MyReceiver = GetComponent<Receiver>();
        InputManager = GetComponent<PlayerInputManager>();
    }

    //Only called if we are broadcasting
    public void HandleBroadcast(PlayerInputState input)
    {
        if (input.JumpDown && CanTransmit(TransmissionType.Jump))
        {
            //Instantiate a "Jump" Transmission 
            TransmissionBullet t = Instantiate(transmissionProjectile);
            t.TransmissionType = TransmissionType.Jump;
            t.Sender = this;
            t.GetComponent<Rigidbody2D>().velocity = getBulletVelocity();
            StartCoroutine(Cooldown());
        }

        if (input.TransmitMove && CanTransmit(TransmissionType.Move))
        {
            //Instantiate a "Move" Transmission 
            TransmissionBullet t = Instantiate(transmissionProjectile);
            t.TransmissionType = TransmissionType.Move;
            t.Sender = this;
            t.GetComponent<Rigidbody2D>().velocity = getBulletVelocity();
            StartCoroutine(Cooldown());
        }

    }

    public void HandleRecall(PlayerInputState input)
    {
        //Instantiate a "Recall Transmission"
        RecallBullet r = Instantiate(recallProjectile);
        r.OwningReceiver = MyReceiver;
        r.OwningTransmitter = this;
        r.GetComponent<Rigidbody2D>().velocity = getBulletVelocity();
        StartCoroutine(Cooldown());
    }

    private Vector2 getBulletVelocity()
    {
        return new Vector2(10f, 0f);
    }

    IEnumerator Cooldown()
    {
        RemainingCD = TransmitCooldown;
        while(RemainingCD > 0)
        {
            RemainingCD -= Time.deltaTime;
            yield return null;
        }
    }

    public bool CanTransmit(TransmissionType t)
    {
        return RemainingCD <= 0f && MyActionBank.HasAction(t);
    }

    public Receiver TryTransmit(TransmissionType t, Receiver targetReceiver)
    {
        //If we don't actually have the ability to transmit, do nothing.
        if (!MyActionBank.HasAction(t))
            return null;

        //If the receiver cannot receive the transmission, do nothing.
        if (!targetReceiver.CanReceive(t))
            return null;

        if (targetReceiver.ReceiveTransmission(t))
        {
            MyActionBank.RemoveAction(t);
        }
        return targetReceiver;
    }
}