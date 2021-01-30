using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    float yaw;
    float pitch;
    public Vector2 pitchMinMax = new Vector2(-40, 85);
    public float rotationSmoothTime = .12f;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    public float sensitivity = 1;
    public bool invertControls;

    private void Update()
    {
        yaw += Input.GetAxis("Mouse X");
        if (invertControls)
            pitch += Input.GetAxis("Mouse Y");
        else
            pitch -= Input.GetAxis("Mouse Y");
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch * sensitivity, yaw * sensitivity), ref rotationSmoothVelocity, rotationSmoothTime);
        

        //Vector3 targetRotation = new Vector3(pitch * sensitivity, yaw * sensitivity);
        transform.eulerAngles = currentRotation;
    }
}
