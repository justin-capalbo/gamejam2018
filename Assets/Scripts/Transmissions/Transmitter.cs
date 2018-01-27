using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public enum TransmissionType { Move, Jump }

[RequireComponent(typeof(Repository))]
public class Transmitter : MonoBehaviour
{
    public Repository MyRepository;

    public void Awake()
    {
        MyRepository = GetComponent<Repository>();
    }

    public void Transmit(TransmissionType t, Receiver targetReceiver)
    {
        //If we don't actually have the ability to transmit, do nothing.
        if (!MyRepository.Contains(t))
            return;

        //If the receiver cannot receive the transmission, do nothing.
        if (!targetReceiver.CanReceive(t))
            return;

        if (targetReceiver.TryReceive(t))
        {
            MyRepository.Remove(t);
        }
    }
}