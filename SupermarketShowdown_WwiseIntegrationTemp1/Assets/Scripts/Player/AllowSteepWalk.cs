using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllowSteepWalk : MonoBehaviour
{
    public float targetSlopeVal = 0.7f;
    float defaultSlopeVal;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        defaultSlopeVal = player.GetComponent<Movement>().steepWalkingDiff;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
            player.GetComponent<Movement>().steepWalkingDiff = targetSlopeVal;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            player.GetComponent<Movement>().steepWalkingDiff = defaultSlopeVal;
    }
}
