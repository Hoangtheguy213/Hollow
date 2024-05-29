using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    // Start is called before the first frame update
    private float startPos;
    public GameObject cam;
    public float ParallaxEffect;

    void Start()
    {
        startPos = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = cam.transform.position.x * ParallaxEffect; //0= move with camera, 1= not move, 0,5 = half speed
        transform.position = new Vector3(startPos +distance, transform.position.y, transform.position.z);
    }
}
