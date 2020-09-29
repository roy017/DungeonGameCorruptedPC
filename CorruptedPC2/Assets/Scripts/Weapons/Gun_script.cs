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
    private Transform Muzzle;

    private Transform aimTransform;
    private Vector3 mousePos;
    private Vector3 handPosR, handPosL;
    private GameObject aim;
    private bool LeftHandOn, RightHandOn;

    private GameObject Bullet;
    [Header("Gun")]
    public float Gun_Spread_angle = 10;


    [Header("Bullet")]
    public float Bullet_Speed = 10;
    public float Bullet_Duration = 5f;
    
    // Start is called before the first frame update
    private void Awake()
    {
        aimTransform = transform.Find("Aim");
        handPosR = new Vector3(aimTransform.localPosition.x, aimTransform.localPosition.y, aimTransform.localPosition.z);
        handPosL = new Vector3(-aimTransform.localPosition.x, aimTransform.localPosition.y, aimTransform.localPosition.z);
        aim = this.transform.GetChild(0).gameObject;
        GameObject gun = aim.transform.GetChild(0).gameObject;
        Muzzle = gun.transform.GetChild(0).gameObject.transform;
        Bullet = gun.transform.GetChild(1).gameObject;

        RightHandOn = true;
        LeftHandOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        Aiming();
        Shooting();
    }

    private void Aiming()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        Vector3 aimDirection = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        aimTransform.eulerAngles = new Vector3(0, 0, angle);
        Muzzle.eulerAngles = new Vector3(0, 0, angle);

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

    private void Shooting()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("PewPew");
            //float angle = Random.Range(Gun_Spread_angle,Gun_Spread_angle);
            
            Vector3 angle = new Vector3( 0, Random.Range(-Gun_Spread_angle, Gun_Spread_angle), 0);
            GameObject bullet_Shot = Instantiate(Bullet, Muzzle.position, Muzzle.localRotation);
            SpriteRenderer sr_Bullet = bullet_Shot.GetComponent<SpriteRenderer>();
            sr_Bullet.enabled = !sr_Bullet.enabled;
            Rigidbody2D rb_Bullet = bullet_Shot.GetComponent<Rigidbody2D>();
            CircleCollider2D cCol2D = bullet_Shot.AddComponent<CircleCollider2D>();
            rb_Bullet.AddForce((Muzzle.right + (angle * Mathf.Deg2Rad)) * Bullet_Speed, ForceMode2D.Impulse);

            Destroy(bullet_Shot, Bullet_Duration);

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
}
