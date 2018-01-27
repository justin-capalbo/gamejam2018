using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Receiver))]
public class Repository : MonoBehaviour
{
    //Transmissions I currently have
    [SerializeField]
    private List<TransmissionType> currentTransmissions = new List<TransmissionType>();

    public bool Contains(TransmissionType t)
    {
        return currentTransmissions.Contains(t);
    }

    public void Add(TransmissionType t)
    {
        currentTransmissions.Add(t);
    }

    public bool Remove(TransmissionType t)
    {
        return currentTransmissions.Remove(t);
    }
}
