﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killzone : MonoBehaviour {

    private GameController gameController;

    private void Start()
    {
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerInputManager>())
        {
            gameController.RestartLevel();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireMesh(new Mesh());
    }
}
