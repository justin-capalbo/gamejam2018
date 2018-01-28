using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    [SerializeField]
    private float maxLife = 10f;
    private float lifetime = 0f;


    private void Update()
    {
        lifetime += Time.deltaTime;
        if (lifetime >= maxLife)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerInputManager>())
        {
            Destroy(collision.gameObject);
        }

        Destroy(gameObject);
    }
}
