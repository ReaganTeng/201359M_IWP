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


                // Compare squared distances to avoid using expensive square root
                if (distanceSquared >= cam.orthographicSize * 1.75f)
                {
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


        //foreach (GameObject GO in FindObjectsOfType<GameObject>())
        //{
        //    if (!GO.CompareTag("Item") && !GO.CompareTag("Enemy"))
        //    {
        //        continue;
        //    }


        //    Vector2 gopos = GO.transform.position;
        //    Vector2 camerapos = cam.transform.position;

        //    // Use sqrMagnitude for distance comparison as it's faster than Distance
        //    float distanceSquared = (gopos - camerapos).sqrMagnitude;
        //    float maxDistanceSquared = cam.orthographicSize * cam.orthographicSize;

        //    // Check if the object is active before making decisions
        //    if (GO.activeSelf)
        //    {
        //        // Compare squared distances to avoid using expensive square root
        //        if (distanceSquared >= maxDistanceSquared)
        //        {
        //            GO.SetActive(false);
        //        }
        //    }
        //    else
        //    {
        //        // If the object is already inactive and within the view, set it active
        //        if (distanceSquared < maxDistanceSquared)
        //        {
        //            GO.SetActive(true);
        //        }
        //    }
        //}
    }
}
