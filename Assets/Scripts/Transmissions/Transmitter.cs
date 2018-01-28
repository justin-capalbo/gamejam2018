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

    private Dictionary<TransmissionType, Receiver> foreignReceivers = new Dictionary<TransmissionType, Receiver>();

    public TransmissionBullet transmissionProjectile;

    public float TransmitCooldown = 1f;
    private float RemainingCD = 0f;

    [SerializeField]
    private float bulletSpeed = 10f; //<<OAKWOOD ADDED>>
    [SerializeField]
    private GameObject tPointer; //<<OAKWOOD ADDED>>

    private AudioSource transmittingSound;  //<<OAKWOOD ADDED>> 

    public void Awake()
    {
        MyActionBank = GetComponent<ActionBank>();
        MyReceiver = GetComponent<Receiver>();
        InputManager = GetComponent<PlayerInputManager>();
        transmittingSound = GetComponent<AudioSource>();
    }
        
    //Only called if we are broadcasting
    public void HandleBroadcast(PlayerInputState input, bool broadcasting) 
    {
        if (!broadcasting)
        {
            if (tPointer.activeSelf)
                tPointer.SetActive(false);
            transmittingSound.Stop();
            return;
        }

        if (!transmittingSound.isPlaying) //<<OAKWOOD ADDED>> 
            transmittingSound.Play();        
        
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


        Vector2 targetVector = new Vector2(input.Horizontal, input.Vertical).normalized * bulletSpeed;
        if (input.JumpDown && CanTransmit(TransmissionType.Jump) && targetVector != new Vector2(0,0))
            ShootTransmissionBullet(TransmissionType.Jump, targetVector,tPointer.transform.rotation);

        if (input.TransmitMove && CanTransmit(TransmissionType.Move) && targetVector != new Vector2(0,0)) 
            ShootTransmissionBullet(TransmissionType.Move, targetVector,tPointer.transform.rotation);

    }

    public void ShootTransmissionBullet(TransmissionType type, Vector2 targetVector, Quaternion rot)
    {
        // Acquire vector for shot
        TransmissionBullet t = Instantiate(transmissionProjectile, gameObject.transform.position, rot);
        t.TransmissionType = type;
        t.Sender = this;
        t.GetComponent<Rigidbody2D>().velocity = targetVector;
        StartCoroutine(Cooldown());
        tPointer.SetActive(false);
        transmittingSound.Stop();
    }

    public void HandleRecall(PlayerInputState input, bool recalling)
    {
        if (!recalling)
            return;

        Receiver foreignReceiver = null;
        if (CanRecall(TransmissionType.Jump, input.JumpDown))
            foreignReceiver = foreignReceivers[TransmissionType.Jump];
        if (CanRecall(TransmissionType.Move, input.TransmitMove))
            foreignReceiver = foreignReceivers[TransmissionType.Move];

        if (foreignReceiver)
        {
            if (foreignReceiver.ActionBank.Count() > 0)
            {
                //Pull the transmission from the foreign receiver
                TransmissionType type = foreignReceiver.ActionBank.GetFirst();
                if (MyReceiver.CanReceive(type) && MyReceiver.ReceiveTransmission(type))
                {
                    foreignReceiver.ActionBank.RemoveAction(type);
                    foreignReceivers.Remove(type);

                    //Grant the appropriate action controller of the receive to the sender.
                    if (type == TransmissionType.Jump)
                        InputManager.JumpingController = InputManager.PlayerMovementController;

                    if (type == TransmissionType.Move)
                        InputManager.MovingController = InputManager.PlayerMovementController;

                }
            }
        }
    }

    private bool CanRecall(TransmissionType t, bool inputFlag)
    {
        return inputFlag && foreignReceivers.ContainsKey(t);
    }

    public void SetForeignReceiver(TransmissionType t, Receiver r)
    {
        foreignReceivers.Add(t, r);
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