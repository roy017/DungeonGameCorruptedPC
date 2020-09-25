using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AI_1 : MonoBehaviour
{
    private Vector3 start_Pos;
    private Vector3 roam_Pos;
    private Animator anim;
    private SpriteRenderer sprt;

    public float MIN_Roam_Dist = 10f;
    public float MAX_Roam_Dist = 50f;
    public float enemy_Speed = 1f;
    public bool Alert = false;
    private bool Aware = false;


    // Start is called before the first frame update
    void Start()
    {
        start_Pos = transform.position;
        roam_Pos = roamPosFunc();
        anim = GetComponent<Animator>();
        sprt = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        
        transform.position = Vector3.MoveTowards(transform.position, roam_Pos, enemy_Speed * Time.deltaTime);
        if(Vector3.Distance(roam_Pos, transform.position) < 0.2f)
        {
            roam_Pos = roamPosFunc();
        }

        anim.SetFloat("Horizontal", roam_Pos.x);
        anim.SetFloat("Vertical", roam_Pos.y);
        if((roam_Pos.x - transform.position.x) < 0f)
            sprt.flipX = true;
        else
            sprt.flipX = false;

        if (Alert == false)
        {
            anim.SetFloat("Alert", 0f);
        }
        else
        {
            if(Aware == false)
            {
                anim.SetFloat("Alert", 0.5f);
                Aware = true;
            }
            else
                anim.SetFloat("Alert", 1f);
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
}
