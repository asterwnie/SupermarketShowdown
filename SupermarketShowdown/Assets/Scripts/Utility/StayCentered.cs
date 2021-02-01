using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayCentered : MonoBehaviour
{
    public Transform targetTransform;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(targetTransform.position.x, transform.position.y, targetTransform.position.z);
        transform.rotation = targetTransform.rotation;
    }
}
