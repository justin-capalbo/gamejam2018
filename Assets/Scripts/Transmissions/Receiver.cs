using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(Repository))]
public class Receiver : MonoBehaviour
{
    //Transmissions I am allowed to receive
    [SerializeField]
    private List<TransmissionType> ValidTransmissions = new List<TransmissionType>();

    public Repository Repository;

    public void Awake()
    {
        Repository = GetComponent<Repository>();
    }

    //Determine if I am allowed to receive this transmission
    public bool CanReceive(TransmissionType t)
    {
        return ValidTransmissions.Contains(t);
    }

    //If I already have this transmission, don't receive it
    public bool TryReceive(TransmissionType t)
    {
        if (Repository.Contains(t))
            return false;

        Repository.Add(t);
        return true;
    }
}

