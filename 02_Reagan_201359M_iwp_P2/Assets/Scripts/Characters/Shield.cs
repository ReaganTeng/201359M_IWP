using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Shield : MonoBehaviour
{

    SpriteRenderer sprite;
    //CircleCollider2D circleCollider;

    //public float shieldtimer;


    [HideInInspector]
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
        if (!shieldActive) {
            sprite.enabled = false;
            //Debug.Log("SHIELD NOT ACTIVE");
            //circleCollider.enabled = false;
        }           
        else
        {
            sprite.enabled = true;
            //circleCollider.enabled = true;
        }
        
    }


}
