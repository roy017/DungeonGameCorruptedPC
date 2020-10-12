//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun_script : MonoBehaviour
{
    /*
    public event EventHandler<OnShootEventArgs> OnShoot;
    public class OnShootEventArgs : EventArgs
    {
        public Vector3 gunEndPointPosition;
        public Vector3 shootPosition;
    }
    */
    private GameObject Muzzle;
    private GameObject MuzzleFlash;

    private Transform aimTransform;
    private Vector3 mousePos;
    private Vector3 handPosR, handPosL;
    private GameObject aim;
    private bool LeftHandOn, RightHandOn;

    private GameObject Bullet;
    private GameObject Magazine; // also contains the position where the magazine will drop from.
    private GameObject BulletCase; //also contains the postion where the bulletcase appears from.
    private GameObject MagazinePos;
    private GameObject BulletCasePos;
    [Header("Gun")]
    public float Gun_Spread_angle = 10;
    public int Gun_MagSize = 6;
    public int Gun_TotCurBullets;
    public float Reloadtime = 0.4f;
    private float timeOfReload;
    private bool MustReload = false;

    [Header("Bullet")]
    public float Bullet_Speed = 10;
    public float Bullet_Duration = 5f;
    public bool effectsOn = false;
    public float lifetime_Props = 100f; // for magazines and bullet cases
    
    // Start is called before the first frame update
    private void Awake()
    {
        aimTransform = transform.Find("Aim");
        handPosR = new Vector3(aimTransform.localPosition.x, aimTransform.localPosition.y, aimTransform.localPosition.z);
        handPosL = new Vector3(-aimTransform.localPosition.x, aimTransform.localPosition.y, aimTransform.localPosition.z);
        aim = this.transform.GetChild(0).gameObject;
        GameObject gun = aim.transform.GetChild(0).gameObject;
        Muzzle = gun.transform.GetChild(0).gameObject;
        MuzzleFlash = Muzzle.transform.GetChild(0).gameObject;
        Bullet = gun.transform.GetChild(1).gameObject;
        MagazinePos = gun.transform.GetChild(3).gameObject;
        BulletCasePos = gun.transform.GetChild(4).gameObject;
        Magazine = MagazinePos.transform.GetChild(0).gameObject;
        BulletCase = BulletCasePos.transform.GetChild(0).gameObject;
        //Debug.Log(Magazine.gameObject.transform);


        RightHandOn = true;
        LeftHandOn = false;
    }

    private void Start()
    {
        Gun_TotCurBullets = Gun_MagSize;
    }

    // Update is called once per frame
    void Update()
    {
        Aiming();
        Shooting();
        Reload();
    }

    private void Aiming()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        Vector3 aimDirection = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        aimTransform.eulerAngles = new Vector3(0, 0, angle);
        Muzzle.transform.eulerAngles = new Vector3(0, 0, angle);

        Vector3 aimLocalScale = Vector3.one;
        Vector3 handFix = aimTransform.position;
        
        if( mousePos.x >= (aimTransform.position.x + (2 * handPosR.x)) && LeftHandOn == true)
        {
            LeftHandOn = false;
            RightHandOn = true;
        }
        if (mousePos.x <= (aimTransform.position.x + (2 * handPosL.x)) && RightHandOn == true)
        {
            LeftHandOn = true;
            RightHandOn = false;
        }

        //Debug.Log("Lefthand pos: " + (aimTransform.position.x + (2 * handPosL.x)) + "Righthand pos: " + aimTransform.position.x);
        if(LeftHandOn == false)
        {
            aimLocalScale.y = +1f;
            handFix = handPosR;
        }
        else
        {
            aimLocalScale.y = -1f;
            handFix = handPosL;
        }

        /* orignal flipping, keeping it for backup
        if(angle > 90 || angle < -90)
        {
            aimLocalScale.y = -1f;
            handFix = handPosL;
        }
        else
        {
            aimLocalScale.y = +1f;
            handFix = handPosR;
        }
        */
        aimTransform.localScale = aimLocalScale;
        aimTransform.localPosition = handFix;
    }

    //TODO look into the muzzleflash, could use improvement
    private void Shooting()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("PewPew");
            //float angle = Random.Range(Gun_Spread_angle,Gun_Spread_angle);
            if(Gun_TotCurBullets > 0)
            {
                Vector3 angle = new Vector3( 0, Random.Range(-Gun_Spread_angle, Gun_Spread_angle), 0);
                GameObject bullet_Shot = Instantiate(Bullet, Muzzle.transform.position, Muzzle.transform.rotation);
                GameObject muzzle_flash = Instantiate(MuzzleFlash, Muzzle.transform.position, Muzzle.transform.rotation);

                SpriteRenderer sr_Bullet = bullet_Shot.GetComponent<SpriteRenderer>();
                SpriteRenderer sprt_muzzle_flash = muzzle_flash.GetComponent<SpriteRenderer>();

                sr_Bullet.enabled = !sr_Bullet.enabled;
                sprt_muzzle_flash.enabled = !sprt_muzzle_flash.enabled;

                Rigidbody2D rb_Bullet = bullet_Shot.GetComponent<Rigidbody2D>();

                CircleCollider2D cCol2D = bullet_Shot.AddComponent<CircleCollider2D>();
                rb_Bullet.AddForce((Muzzle.transform.right + (angle * Mathf.Deg2Rad)) * Bullet_Speed, ForceMode2D.Impulse);

                CameraShake.Instance.ShakeCamera(1f,0.2f);
                //Debug.Log(rb_Magazine.velocity.);

                Destroy(bullet_Shot, Bullet_Duration);
                Destroy(muzzle_flash, 0.05f);
                Gun_TotCurBullets -= 1;
            }

            Vector3 angle1 = new Vector3(0, Random.Range(-Gun_Spread_angle, Gun_Spread_angle), 0);
            if (effectsOn == true)
                propsEffect(angle1);

            // shoot stuff;
                /*
                OnShoot?.Invoke(this, new OnShootEventArgs
                {
                    gunEndPointPosition = Muzzle.position,
                    shootPosition = mousePos,
                });
                */
        }
    }

    private void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            timeOfReload = Time.time;
            //Debug.Log("R pressed " + timeOfReload);
            MustReload = true; 
        }
        if(MustReload == true)
        {
            if (Time.time >= (timeOfReload + Reloadtime))
            {
                //Debug.Log("ReloadNow");
                Gun_TotCurBullets = Gun_MagSize;
                MustReload = false;
            }
        }

    }

    void propsEffect(Vector3 angle)
    {
        GameObject magazine = Instantiate(Magazine, MagazinePos.transform.position, MagazinePos.transform.rotation);
        GameObject bullet_case = Instantiate(BulletCase, BulletCasePos.transform.position, BulletCasePos.transform.rotation);

        SpriteRenderer sprt_magazine = magazine.GetComponent<SpriteRenderer>();
        SpriteRenderer sprt_bullet_case = bullet_case.GetComponent<SpriteRenderer>();

        sprt_magazine.enabled = !sprt_magazine.enabled;
        sprt_bullet_case.enabled = !sprt_bullet_case.enabled;

        Rigidbody2D rb_Magazine = magazine.GetComponent<Rigidbody2D>();
        Rigidbody2D rb_BulletCase = bullet_case.GetComponent<Rigidbody2D>();

        rb_Magazine.AddForce((-MagazinePos.transform.up + (angle * Mathf.Deg2Rad)), ForceMode2D.Impulse);
        rb_BulletCase.AddForce((BulletCasePos.transform.up + (angle * Mathf.Deg2Rad)), ForceMode2D.Impulse);

        Destroy(magazine, lifetime_Props);
        Destroy(bullet_case, lifetime_Props);
    }
}
