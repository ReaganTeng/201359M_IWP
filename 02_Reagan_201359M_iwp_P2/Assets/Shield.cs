using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Shield : MonoBehaviour
{

    SpriteRenderer sprite;
    //CircleCollider2D circleCollider;

    [HideInInspector]
    public float shieldtimer;
    public bool shieldActive;
    public void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        shieldtimer = 0;
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
