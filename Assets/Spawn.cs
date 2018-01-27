using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour {

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private float yOffset = 0.5f;

	// Use this for initialization
	void Start () {
        Instantiate(player,new Vector3(transform.position.x,transform.position.y+yOffset,0f),Quaternion.identity);		
	}
}
