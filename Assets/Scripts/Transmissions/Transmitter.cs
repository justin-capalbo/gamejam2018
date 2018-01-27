using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public enum TransmissionType { Move, Jump }

[RequireComponent(typeof(ActionBank))]
public class Transmitter : MonoBehaviour
{
    public ActionBank MyActionBank;

    public void Awake()
    {
        MyActionBank = GetComponent<ActionBank>();
    }

    public void Transmit(TransmissionType t, Receiver targetReceiver)
    {
        //If we don't actually have the ability to transmit, do nothing.
        if (!MyActionBank.Contains(t))
            return;

        //If the receiver cannot receive the transmission, do nothing.
        if (!targetReceiver.CanReceive(t))
            return;

        if (targetReceiver.TryReceive(t))
        {
            MyActionBank.Remove(t);
        }
    }
}