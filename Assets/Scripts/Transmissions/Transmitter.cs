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

    [SerializeField]
    private float bulletSpeed = 10f; //<<OAKWOOD ADDED>>
    [SerializeField]
    private GameObject tPointer; //<<OAKWOOD ADDED>>
        
    public void Awake()
    {
        MyActionBank = GetComponent<ActionBank>();
        MyReceiver = GetComponent<Receiver>();
        InputManager = GetComponent<PlayerInputManager>();
    }

    //Only called if we are broadcasting
    public void HandleBroadcast(PlayerInputState input) 
    {
        //<<OAKWOOD ADDED>> 
        // Handle targeting visual aid
        // Should disappear when player is not actively aiming with stick
        var buffer = 0.2f;

        if (Mathf.Abs(input.Horizontal) >= buffer || Mathf.Abs(input.Vertical) >= buffer)
        {
            tPointer.SetActive(true);
            var targetAngle = (Mathf.Atan2(input.Vertical,input.Horizontal) * Mathf.Rad2Deg) - 90;
            tPointer.transform.rotation = Quaternion.Slerp(tPointer.transform.rotation, Quaternion.Euler(0,0,targetAngle), 10f * Time.deltaTime);
        } 
        else
            tPointer.SetActive(false);

       

        // Acquire vector for shot
        var targetVector = new Vector2(input.Horizontal,input.Vertical).normalized * bulletSpeed;

        if (input.JumpDown && CanTransmit(TransmissionType.Jump) && targetVector != new Vector2(0,0)) //<<OAKWOOD ADDED>>
        {
            //Instantiate a "Jump" Transmission 
            TransmissionBullet t = Instantiate(transmissionProjectile,gameObject.transform.position,Quaternion.identity);
            t.TransmissionType = TransmissionType.Jump;
            t.Sender = this;
            t.GetComponent<Rigidbody2D>().velocity = targetVector;
            StartCoroutine(Cooldown());
            tPointer.SetActive(false);
        }

        if (input.TransmitMove && CanTransmit(TransmissionType.Move) && targetVector != new Vector2(0,0)) //<<OAKWOOD ADDED>>
        {
            //Instantiate a "Move" Transmission 
            TransmissionBullet t = Instantiate(transmissionProjectile,gameObject.transform.position,Quaternion.identity);
            t.TransmissionType = TransmissionType.Move;
            t.Sender = this;
            t.GetComponent<Rigidbody2D>().velocity = targetVector;
            StartCoroutine(Cooldown());
            tPointer.SetActive(false);
        }

    }

    public void HandleRecall(PlayerInputState input)
    {
        //Instantiate a "Recall Transmission"
        RecallBullet r = Instantiate(recallProjectile,gameObject.transform.position,Quaternion.identity);
        r.OwningReceiver = MyReceiver;
        r.OwningTransmitter = this;
        r.GetComponent<Rigidbody2D>().velocity = getBulletVelocity();
        StartCoroutine(Cooldown());
    }

    private Vector2 getBulletVelocity()
    {
        return new Vector2(20f, 0f);
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