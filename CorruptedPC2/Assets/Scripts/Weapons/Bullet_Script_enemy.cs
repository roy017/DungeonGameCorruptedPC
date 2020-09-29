using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Script_enemy : MonoBehaviour
{
    private GameObject hit_Effect;

    private void Start()
    {
        hit_Effect = this.transform.GetChild(0).gameObject;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        Vector3 dir = collision.contacts[0].point - (Vector2)transform.position;
        GameObject effect = Instantiate(hit_Effect, collision.contacts[0].point + (Vector2)(dir * -1.5f), collision.gameObject.transform.rotation);

        SpriteRenderer sprt_effect = effect.GetComponent<SpriteRenderer>();
        sprt_effect.enabled = !sprt_effect.enabled;

        Destroy(gameObject);
        Destroy(effect, .4f);
    }
}
