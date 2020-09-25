using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Script : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Add in an effect that happens on collision
        Destroy(gameObject);
    }
}
