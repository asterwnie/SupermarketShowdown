using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustGameobjectActive : MonoBehaviour
{
    public GameObject targetObject;

    public void SetGameObjectActive(bool state)
    {
        targetObject.SetActive(state);
    }
}
