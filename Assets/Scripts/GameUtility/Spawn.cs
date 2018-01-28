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
        GameObject newPlayer = Instantiate(player,new Vector3(transform.position.x,transform.position.y+yOffset,0f),Quaternion.identity);
        GameController.S.playerRef = newPlayer.GetComponent<AdvancedMovementController>();
	}
}
