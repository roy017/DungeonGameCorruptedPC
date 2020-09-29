using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AI_1 : MonoBehaviour
{
    private Vector3 start_Pos;
    private Vector3 roam_Pos;
    private Animator anim;
    private SpriteRenderer sprt;
    private GameObject gun;
    private SpriteRenderer sprt_gun;
    private CircleCollider2D cCol2D;
    public GameObject player;
    private Rigidbody2D rb;
    private bool gotHit = false;
    private float gotHitTime;

    public float MIN_Roam_Dist = 10f;
    public float MAX_Roam_Dist = 50f;
    public float enemy_Speed = 1f;
    public bool Alert = false;
    private bool Aware = false;
    public float detection_Radius = 5f;
    public float stun_by_hit_time = 0.2f;


    // Start is called before the first frame update
    void Start()
    {
        start_Pos = transform.position;
        roam_Pos = roamPosFunc();
        anim = GetComponent<Animator>();
        sprt = GetComponent<SpriteRenderer>();
        GameObject aim = this.transform.GetChild(0).gameObject;
        gun = aim.transform.GetChild(0).gameObject;
        sprt_gun = gun.GetComponent<SpriteRenderer>();
        cCol2D = GetComponent<CircleCollider2D>();
        cCol2D.radius = detection_Radius;

        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        
        
        if(Vector3.Distance(roam_Pos, transform.position) < 0.2f && Alert == false)
        {
            roam_Pos = roamPosFunc();
        }
        
        

        if (Alert == false)
        {
            transform.position = Vector3.MoveTowards(transform.position, roam_Pos, enemy_Speed * Time.deltaTime);

            anim.SetFloat("Alert", 0f);

            // not sure if this is correct, got to test it
            anim.SetFloat("Horizontal", roam_Pos.x);
            anim.SetFloat("Vertical", roam_Pos.y);

            if ((roam_Pos.x - transform.position.x) < 0f)
                sprt.flipX = true;
            else
                sprt.flipX = false;
        }
        else
        {
            if (gotHit == true)
            {
                Debug.Log("Time: " + Time.time + " HitTime: " + (gotHitTime + stun_by_hit_time));
                rb.velocity = Vector3.zero;
                rb.angularVelocity = 0f;
                if (Time.time > (gotHitTime + stun_by_hit_time))
                {
                    gotHit = false;
                }
            }
            else
                if (Vector3.Distance(transform.position, player.transform.position) > 2f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, player.transform.position, enemy_Speed * Time.deltaTime);
                }
                
            if (Aware == false)
            {
                anim.SetFloat("Alert", 0.5f);
                sprt_gun.enabled = !sprt_gun.enabled;
                Aware = true;
            }
            else
                anim.SetFloat("Alert", 1f);
            // not sure if this is correct, got to test it
            anim.SetFloat("Horizontal", (player.transform.position.x - transform.position.x));
            anim.SetFloat("Vertical", (player.transform.position.y - transform.position.y));
            if ((player.transform.position.x - transform.position.x) < 0f)
            {
                sprt.flipX = true;
                //sprt_gun.flipX = true;
            }
            else
            {
                sprt.flipX = false;
                //sprt_gun.flipX = false;
            }

        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject.name);
        if(other.gameObject.tag == "Player")
        {
            Alert = true;
            player = other.gameObject;
            //Debug.Log("Found Player");
        }
    }

    private Vector3 RandDir()
    {
        Vector3 result;
        float x, y;
        x = Random.Range(-1f, 1f);
        y = Random.Range(-1f, 1f);
        result = new Vector3(x, y, 0).normalized;
        return result;
    }

    private Vector3 roamPosFunc()
    {
        return start_Pos + RandDir() * Random.Range(MIN_Roam_Dist, MAX_Roam_Dist);
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        PolygonCollider2D enemyCol = GetComponent<PolygonCollider2D>();
        if(collision.gameObject.tag == "Bullet_Player" || collision.gameObject.tag == "Player")
        {
            gotHit = true;
            gotHitTime = Time.time;
        }
        
    }
    
}
