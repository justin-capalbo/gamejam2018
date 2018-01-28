using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour {

    [SerializeField]
    private GameObject projectile;
    [SerializeField]
    private float fireRate = 40f;
    [SerializeField]
    private float pSpeed = 4f;
    [SerializeField]
    private float turnSpeed = 10f;

    private float hori = 0;
    private float vert = 1;
    
    // Use this for initialization
    void Start () {
        InvokeRepeating("Fire",fireRate * Time.deltaTime,fireRate * Time.deltaTime);
    }
	
	// Update is called once per frame
	void Update () {  
        Move();
	}

    void Fire()
    {
        var p = Instantiate(projectile, transform.position, transform.rotation);
        p.GetComponent<Rigidbody2D>().velocity = new Vector2(hori, vert).normalized * pSpeed;
    }

    void Move()
    {

        if (Input.GetAxis("Horizontal") != 0)
        {
            hori = Input.GetAxis("Horizontal");
        }
        if (Input.GetAxis("Vertical") != 0)
        {
            vert = Input.GetAxis("Vertical");
        }
                
        print(hori + " , " + vert);

        var targetAngle = (Mathf.Atan2(vert,hori) * Mathf.Rad2Deg) - 90;
        transform.rotation = Quaternion.Slerp(transform.rotation,
                                                Quaternion.Euler(0,0,targetAngle),
                                                turnSpeed * Time.deltaTime);

    }

    void Jump()
    {

    }
}


//float targetAngle = Mathf.Atan2(moveDirection.y,moveDirection.x) * Mathf.Rad2Deg;
//transform.rotation = 
//  Quaternion.Slerp(transform.rotation,
//                    Quaternion.Euler( 0, 0, targetAngle ), 
//                    turnSpeed* Time.deltaTime );