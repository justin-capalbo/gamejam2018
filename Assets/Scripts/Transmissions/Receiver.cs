using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(ActionBank))]
public class Receiver : MonoBehaviour
{

    public ActionBank ActionBank;

    public void Awake()
    {
        ActionBank = GetComponent<ActionBank>();
    }

    //Determine if I am allowed to receive this transmission
    public bool CanReceive(TransmissionType t)
    {
        if (ActionBank.HasAction(t) || ActionBank.Full())
            return false;

        if (t == TransmissionType.Jump && gameObject.GetComponentInChildren(typeof(IJumper)) == null)
            return false;

        if (t == TransmissionType.Move && gameObject.GetComponentInChildren(typeof(IMover)) == null)
            return false;

        return true;
    }

    //If I already have this transmission, don't receive it
    public bool ReceiveTransmission(TransmissionType t)
    {
        ActionBank.AddAction(t);
        Animator anim = GetComponentInParent<Animator>();
        if (anim && !gameObject.CompareTag("Player"))
        {
            anim.speed *= 3f;
        }
        return true;
    }
}

