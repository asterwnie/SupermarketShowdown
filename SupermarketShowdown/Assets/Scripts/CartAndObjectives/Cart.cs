using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cart : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "PickupItem")
        {
            if(!GameManager.instance.isChildEatInstantly && other.GetComponent<PickupItem>().isObjective)
            {
                Debug.Log("Item added to cart!");
                GameManager.instance.collectedItems.Add(other.gameObject);
                other.gameObject.SetActive(false);
            }
                
            // spawn some particles
        }
    }
}
