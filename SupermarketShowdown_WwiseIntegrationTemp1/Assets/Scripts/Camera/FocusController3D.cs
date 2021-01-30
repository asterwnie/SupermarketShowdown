using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusController3D : MonoBehaviour
{
    // FocusController3D
    // Attach this as a CHILD of any object you want highlighted.
    // Behavior: When the player is withihn the collision zone of this object, the camera will "focus" on this object's parent.

    DynamicCamera3D gameCamera;


    // Start is called before the first frame update
    void Start()
    {
        //retrieve main camera
        gameCamera = GameObject.Find("DynamicCamera3D").GetComponent<DynamicCamera3D>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag.Contains("Player"))
        {
            //Debug.Log("Player entered trigger zone of " + gameObject.GetComponentInParent<Transform>().gameObject.name);

            // sets its parent to the focus obj
            gameCamera.focusObj = gameObject.GetComponentInParent<Transform>().gameObject;
            //gameCamera.focusObj = gameObject.transform.parent;
        }
    }
    
    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag.Contains("Player"))
        {
            //Debug.Log("Player exited a trigger zone.");
            gameCamera.focusObj = null;
        }
    }
}
