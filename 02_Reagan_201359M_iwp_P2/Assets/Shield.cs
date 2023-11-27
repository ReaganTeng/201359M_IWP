using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public float shieldtimer;

    SpriteRenderer sprite;
    //CircleCollider2D circleCollider;
    public bool shieldActive;
    public void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        //circleCollider = GetComponent<CircleCollider2D>();
        sprite.enabled = false;
        shieldActive = false;
       //circleCollider.enabled = false;
    }

    private void Update()
    {
        if(shieldtimer > 0)
        {
            shieldtimer -= Time.deltaTime;

            if(shieldtimer <= 0 )
            {
                sprite.enabled = false;
                shieldActive = false;
                //circleCollider.enabled = false;
            }
            else
            {
                sprite.enabled = true;
                shieldActive = true;
                //circleCollider.enabled = true;
            }
        }
    }


}
