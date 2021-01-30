using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectThemAll : MonoBehaviour
{
    public List<GameObject> itemsToCollect = new List<GameObject>();
    public GameObject ceiling;
    List<GameObject> itemsCollected = new List<GameObject>();
    
    // Update is called once per frame
    void Update()
    {
        if (itemsCollected.Count == itemsToCollect.Count)
        {
            Debug.Log("YOU WON BABEY!!");
            Destroy(ceiling);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (itemsToCollect.Contains(other.gameObject))
            itemsCollected.Add(other.gameObject);
    }
}
