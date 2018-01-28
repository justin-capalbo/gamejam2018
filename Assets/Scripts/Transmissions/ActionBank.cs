using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Receiver))]
public class ActionBank : MonoBehaviour
{
    //Transmissions I currently have
    [SerializeField]
    private List<TransmissionType> currentTransmissions = new List<TransmissionType>();

    //Transmissions I am allowed to receive
    [SerializeField]
    private List<TransmissionType> ValidTransmissions = new List<TransmissionType>();

    public int MaxTotalActions = 1;


    public GameObject recallEffect;


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
        // Spawn a feedback object
        if (recallEffect)
            Instantiate(recallEffect,transform.position,Quaternion.identity);
        return currentTransmissions.Remove(t);
    }

    public TransmissionType GetFirst()
    {
        return currentTransmissions[0];
    }

    public int Count()
    {
        return currentTransmissions.Count;
    }

    public bool Full()
    {
        return Count() >= MaxTotalActions;
    }
}
