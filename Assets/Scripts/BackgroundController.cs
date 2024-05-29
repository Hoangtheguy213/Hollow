using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    // Start is called before the first frame update
    private float startPos, length;
    public GameObject cam;
    public float ParallaxEffect;

    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // tinh toan khoang cach cua background dua tren toc do di chuyen cua camera
        float distance = cam.transform.position.x * ParallaxEffect; //0= move with camera, 1= not move, 0,5 = half speed
        float movement = cam.transform.position.x * (1 - ParallaxEffect);  

        transform.position = new Vector3(startPos +distance, transform.position.y, transform.position.z);
        if (movement >startPos + length)
        {
            startPos += length;
        }
        else if(movement <startPos - length)
        {
            startPos -= length;
        }
    }
    
}
