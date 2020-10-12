using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun_script_enemy : MonoBehaviour
{
    [SerializeField]
    private Enemy_AI_1 enemy_Script;
    private bool Alert;

    private GameObject Player;
    private Transform Muzzle;
    private GameObject muzzleFlash;

    private Transform aimTransform;
    private Vector3 mousePos;
    private Vector3 handPosR, handPosL;
    private GameObject aim;
    private GameObject gun;
    private Animator anim_gun;
    private bool LeftHandOn;// RightHandOn;
    private float nextShootTime;

    private GameObject Bullet;
    [Header("Gun")]
    public float Gun_Spread_angle = 10;
    public float shoot_distance = 5f;


    [Header("Bullet")]
    public float Bullet_Speed = 10;
    public float Bullet_Duration = 5f;
    public float fireRate = 1f;

    private void Awake()
    {
        aimTransform = transform.Find("Weapon_Position");
        handPosR = new Vector3(aimTransform.localPosition.x, aimTransform.localPosition.y, aimTransform.localPosition.z);
        handPosL = new Vector3(-aimTransform.localPosition.x, aimTransform.localPosition.y, aimTransform.localPosition.z);
        aim = this.transform.GetChild(0).gameObject;
        gun = aim.transform.GetChild(0).gameObject;
        Muzzle = gun.transform.GetChild(0).gameObject.transform;
        muzzleFlash = Muzzle.transform.GetChild(0).gameObject;
        Bullet = gun.transform.GetChild(1).gameObject;

        anim_gun = gun.GetComponent<Animator>();

        //RightHandOn = true;
        LeftHandOn = false;
    }

    private void Start()
    {
        enemy_Script = GetComponent<Enemy_AI_1>();
        //nextShootTime = 2f;
    }

    private void Update()
    {
        Alert = enemy_Script.Alert;
        Player = enemy_Script.player;
        if (Alert == true)
        {
            aimingAt();
            shootAt();
        }
    }

    void aimingAt()
    {
        Vector3 playerPos = Player.transform.position;
        Vector3 aimDirection = (playerPos - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        aimTransform.eulerAngles = new Vector3(0, 0, angle);
        Muzzle.eulerAngles = new Vector3(0, 0, angle);

        if ((Player.transform.position.x - transform.position.x) < 0f)
        {
            //RightHandOn = false;
            LeftHandOn = true;
        }
        else
        {
            //RightHandOn = true;
            LeftHandOn = false;
        }

        Vector3 aimLocalScale = Vector3.one;
        Vector3 handFix = aimTransform.position;
        if (LeftHandOn == false)
        {
            aimLocalScale.y = +1f;
            handFix = handPosR;
        }
        else
        {
            aimLocalScale.y = -1f;
            handFix = handPosL;
        }

        aimTransform.localScale = aimLocalScale;
        aimTransform.localPosition = handFix;
    }

    void shootAt()
    {
        if (Vector3.Distance(transform.position, Player.transform.position) < shoot_distance)
        {
            //Debug.Log("enemy can shoot!!!");

            //Vector3 angle = new Vector3( 0, Random.Range(-Gun_Spread_angle, Gun_Spread_angle), 0);

            //fireRate = 0.5f;
            //anim_gun.SetBool("Shoot");
            if (Time.time > nextShootTime)
            {
                anim_gun.SetTrigger("Shoot");
                Vector3 angle = new Vector3(0, 0, 0);
                GameObject bullet_Shot = Instantiate(Bullet, Muzzle.position, Muzzle.rotation);
                GameObject muzzle_flash = Instantiate(muzzleFlash, Muzzle.position, Muzzle.rotation);
                

                SpriteRenderer sr_Bullet = bullet_Shot.GetComponent<SpriteRenderer>();
                SpriteRenderer sprt_muzzle_flash = muzzle_flash.GetComponent<SpriteRenderer>();

                sr_Bullet.enabled = !sr_Bullet.enabled;
                sprt_muzzle_flash.enabled = !sprt_muzzle_flash.enabled;

                Rigidbody2D rb_Bullet = bullet_Shot.GetComponent<Rigidbody2D>();
                CircleCollider2D cCol2D = bullet_Shot.AddComponent<CircleCollider2D>();
                rb_Bullet.AddForce((Muzzle.right + (angle * Mathf.Deg2Rad)) * Bullet_Speed, ForceMode2D.Impulse);

                nextShootTime = Time.time + fireRate;

                Destroy(bullet_Shot, Bullet_Duration);
                Destroy(muzzle_flash, 0.1f);
                
            }

        }
    }
}
