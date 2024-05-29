using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float followSpeed = 0.1f;
    [SerializeField] private Vector3 offSet;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      
            transform.position = Vector3.Lerp(transform.position, Player.Instance.transform.position + offSet, followSpeed);
        
    }
}
