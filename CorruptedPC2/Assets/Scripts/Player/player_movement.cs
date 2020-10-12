using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_movement : MonoBehaviour
{
    public float movement_speed = 5f;
    public Rigidbody2D rb;
    public Animator anim;
    public SpriteRenderer sprt;
    public Camera cam;

    public bool ActionMode = false;

    private Vector2 movement;
    private Vector2 mousePos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float x, y;   
        x = Input.GetAxisRaw("Horizontal");    
        y = Input.GetAxisRaw("Vertical");
        movement = new Vector2(x, y);
        movement = Vector2.ClampMagnitude(movement, 1);
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * movement_speed * Time.fixedDeltaTime);
        Vector2 lookDir = mousePos - rb.position;

        // actionMode false = direction of player is controlled by "WASD", actionMode true = direction of player is controlled by position of mouse
        if (ActionMode == false)
        {
            if (movement != Vector2.zero)
            {
                if (movement.x >= 0f)
                    sprt.flipX = false;
                else
                    sprt.flipX = true;
                anim.SetFloat("Horizontal", movement.x);
                anim.SetFloat("Vertical", movement.y);
            }
        }
        else
        {
            if (lookDir != Vector2.zero)
            {
                if (lookDir.x >= 0f)
                    sprt.flipX = false;
                else
                    sprt.flipX = true;
                anim.SetFloat("Horizontal", lookDir.x);
                anim.SetFloat("Vertical", lookDir.y);
            }
        }
        anim.SetFloat("Speed", movement.sqrMagnitude);
    }
}
