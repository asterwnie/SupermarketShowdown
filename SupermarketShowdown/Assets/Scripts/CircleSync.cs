using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// code and shader from https://www.youtube.com/watch?v=S5gdvibmsV0

public class CircleSync : MonoBehaviour
{
    public static int posID = Shader.PropertyToID("_Position");
    public static int sizeID = Shader.PropertyToID("_Size");

    public List<Material> wallMaterial = new List<Material>();
    public Camera mainCamera;
    public LayerMask mask;

    public float maxCircleRadius = 0.5f;
    public float circleLerpValue = 2f;
    float currSizeValue = 0;

    private void Update()
    {
        var dir = mainCamera.transform.position - transform.position;
        var ray = new Ray(transform.position, dir.normalized);

        foreach(Material mat in wallMaterial)
        {
            if (Physics.Raycast(ray, 1000, mask))
            {
                currSizeValue = Mathf.Lerp(currSizeValue, maxCircleRadius, Time.deltaTime * circleLerpValue);
                mat.SetFloat(sizeID, currSizeValue);
            }
            else
            {
                currSizeValue = Mathf.Lerp(currSizeValue, 0, Time.deltaTime * circleLerpValue);
                mat.SetFloat(sizeID, currSizeValue);
            }


            var view = mainCamera.WorldToViewportPoint(transform.position);
            mat.SetVector(posID, view);
        }
        
    }
}
