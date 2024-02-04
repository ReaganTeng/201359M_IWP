using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class cameraChecker : MonoBehaviour
{

    Camera cam;
    public List<GameObject> Parents;

    // Start is called before the first frame update
    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        
        foreach (GameObject parent in Parents)
        {
            foreach (MonoBehaviour childrenMonoBehaviour in parent.GetComponentsInChildren<MonoBehaviour>())
            {
                Vector2 gopos = childrenMonoBehaviour.gameObject.transform.position;
                Vector2 camerapos = cam.transform.position;
                float distanceSquared = Vector2.Distance(gopos, camerapos);
                Renderer renderer = childrenMonoBehaviour.gameObject.GetComponent<Renderer>();


                if(childrenMonoBehaviour.gameObject.GetComponent<Enemy>() != null
                    && childrenMonoBehaviour.gameObject.GetComponent<Enemy>().currentState != Enemy.EnemyState.IDLE)
                {
                    childrenMonoBehaviour.gameObject.GetComponent<Enemy>().currentState = Enemy.EnemyState.IDLE;
                }

                // Compare squared distances to avoid using expensive square root
                if (distanceSquared >= cam.orthographicSize * 2.0f)
                {
                    //if(childrenMonoBehaviour.gameObject.GetComponent<Enemy>())

                    if (childrenMonoBehaviour.enabled)
                    {
                        if (renderer != null)
                        { 
                            renderer.enabled = false;
                        }
                        childrenMonoBehaviour.enabled = false;

                    }
                }
                else
                {
                    if (!childrenMonoBehaviour.enabled)
                    {
                        if (renderer != null)
                        {
                            renderer.enabled = true;
                        }
                        childrenMonoBehaviour.enabled = true;
                    }
                }
            }
            
        }

    }
}
