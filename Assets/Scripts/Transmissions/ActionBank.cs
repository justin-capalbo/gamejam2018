using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Receiver))]
public class ActionBank : MonoBehaviour
{
    //Transmissions I currently have
    [SerializeField]
    private List<TransmissionType> currentTransmissions = new List<TransmissionType>();

    public bool HasAction(TransmissionType t)
    {
        return currentTransmissions.Contains(t);
    }

    public void AddAction(TransmissionType t)
    {
        currentTransmissions.Add(t);
    }

    public bool RemoveAction(TransmissionType t)
    {
        return currentTransmissions.Remove(t);
    }
}
