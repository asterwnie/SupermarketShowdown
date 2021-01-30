//Michelle Nie
// NOTE THIS IS ONLY FOR 2D (6/18) AND HAS NOT BEEN ADJUSTED FOR 3D YET

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicCamera3D : MonoBehaviour
{
    /* Purpose of this script:
     * Smoothly positions the camera SMOOTHLY between two objects
     * Resizes the camera to appropriately accomodate for both objects
     */

    [SerializeField] private GameObject player; // player
    public GameObject focusObj; // obj to focus

    public bool inInteraction = false; // if the player is interacting with something that requires a DOF change, this overrides normal camera focus.

    //keep X and Z the same for isometric
    public float cameraOffsetX = -10f;
    public float cameraOffsetY = 10f;
    public float cameraOffsetZ = -10f;

    float scrollSensitivity = 2f;
    public float scrollValue;

    public Vector3 minCameraOffset = new Vector3(-4f, 4f, -3f);
    public Vector3 maxCameraOffset = new Vector3(-20f, 20f, -20f);

    public float lerpSpeed = 1f;

    private float distanceBetweenPlayers; //distance between player and focus

    private void Start()
    {
        //find player
        player = GameObject.FindGameObjectWithTag("Player");
        // reset focus
        focusObj = null;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!inInteraction) { 
            if (focusObj != null)
            {
                CalculateDistanceBetweenPlayers();
                CameraFollow2Players();
            }
            else
            {
                // if nothing is focused, do normal camera movement
                CameraFollowPlayer();
            }
        }

        float combinedScrollValue = Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity + scrollValue;
        ScrollCameraWithWheel(combinedScrollValue);
    }

    void ScrollCameraWithWheel(float scrollValue)
    {
        // scroll in/out camera

        // x
        if (cameraOffsetX <= minCameraOffset.x && cameraOffsetX >= maxCameraOffset.x)
        {
            cameraOffsetX += scrollValue;
        }
        else if(cameraOffsetX > minCameraOffset.x)
        {
            cameraOffsetX = minCameraOffset.x;
        }
        else if(cameraOffsetX < maxCameraOffset.x)
        {
            cameraOffsetX = maxCameraOffset.x;
        }

        // y
        if (cameraOffsetY >= minCameraOffset.y && cameraOffsetY <= maxCameraOffset.y)
        {
            cameraOffsetY -= scrollValue;
        }
        else if (cameraOffsetY < minCameraOffset.y)
        {
            cameraOffsetY = minCameraOffset.y;
        }
        else if (cameraOffsetY > maxCameraOffset.y)
        {
            cameraOffsetY = maxCameraOffset.y;
        }

        // z
        if (cameraOffsetZ <= minCameraOffset.z && cameraOffsetZ >= maxCameraOffset.z)
        {
            cameraOffsetZ += scrollValue;
        }
        else if (cameraOffsetZ > minCameraOffset.z)
        {
            cameraOffsetZ = minCameraOffset.z;
        }
        else if (cameraOffsetZ < maxCameraOffset.z)
        {
            cameraOffsetZ = maxCameraOffset.z;
        }
    }

    void CameraFollowPlayer()
    {
        // lerp towards player
        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, new Vector3(player.transform.position.x + cameraOffsetX, player.transform.position.y + cameraOffsetY, player.transform.position.z + cameraOffsetZ), Time.deltaTime * lerpSpeed);
    }

    void CalculateDistanceBetweenPlayers()
    {
        //Collect positions of players
        Vector3 playerPos = player.transform.position;
        Vector3 focusPos = focusObj.transform.position;

        //Get distance in Vector3
        Vector3 displacement = focusPos - playerPos;

        //Use Pythagorean theorem to calculate linear distance
        distanceBetweenPlayers = Mathf.Sqrt(Mathf.Pow(displacement.x, 2) + Mathf.Pow(displacement.z, 2)); //square root of (x^2 + y^2)

        // Uncomment for debug
        //Debug.Log(distanceBetweenPlayers);
    }

    void CameraFollow2Players()
    {
        Vector3 distanceBetweenPlayersV3 = GetVector3DistanceBetweenPlayers();

        //update position to be the midpoint between the two players----------
        if (distanceBetweenPlayers > 0)   //positive = player1 on the right
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, new Vector3((player.transform.position.x - distanceBetweenPlayersV3.x / 2) + cameraOffsetX, player.transform.position.y + cameraOffsetY, (player.transform.position.z - distanceBetweenPlayersV3.z / 2) + cameraOffsetZ), Time.deltaTime * lerpSpeed / 3);
        else                                                    //negative = player2 on the right
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, new Vector3((focusObj.transform.position.x - distanceBetweenPlayersV3.x / 2) + cameraOffsetX, focusObj.transform.position.y + cameraOffsetY, (focusObj.transform.position.z - distanceBetweenPlayersV3.z / 2) + cameraOffsetZ), Time.deltaTime * lerpSpeed / 3);
    }

    Vector3 GetVector3DistanceBetweenPlayers()
    {
        //Collect positions of players
        Vector3 playerPos = player.transform.position;
        Vector3 focusPos = focusObj.transform.position;

        //Get distance in Vector3
        Vector3 displacement = playerPos - focusPos;
        return displacement;

    }
}
