using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Script : MonoBehaviour
{
    //private CircleCollider2D cCol2D;
    private void Start()
    {
        //cCol2D = this.GetComponent<CircleCollider2D>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Add in an effect that happens on collision
        /*
        if (collision.gameObject.tag == "Player")
        {
            BoxCollider2D bCol2D = collision.gameObject.GetComponent<BoxCollider2D>();
            Debug.Log("should ignore " + collision.collider.name + " and " + cCol2D.name);
            Physics2D.IgnoreCollision(collision.collider, cCol2D, true);
            Physics2D.IgnoreCollision(bCol2D, cCol2D, true);
        }*/
        Destroy(gameObject);
    }
}
