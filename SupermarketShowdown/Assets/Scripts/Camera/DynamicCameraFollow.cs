using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCameraFollow : MonoBehaviour
{
    public GameObject player; // object to follows

    public Vector3 cameraPosOffset = new Vector3(0,1, -2.6f);
    public Vector3 cameraRotOffset = new Vector3(15,0,0);

    private void Start()
    {
        transform.eulerAngles = cameraRotOffset;
    }

    // Update is called once per frame
    void Update()
    {
        DynamicFollowPlayer();
    }

    void DynamicFollowPlayer()
    {
        // apply offsets + shifts
        transform.position = player.transform.position + (transform.forward *  cameraPosOffset.z) + (transform.up * cameraPosOffset.y);
        
    }
}
