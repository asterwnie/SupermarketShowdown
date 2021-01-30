using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public LayerMask layerMask;
    public bool canMove;
    Vector3 inputMovement;
    public float moveSpeed;
    Vector3 pos, velocity;
    //float heightOffset = 1.8f;
    float rayDisplacement = 0.5f;
    public float fakeGravityIntensity = 5f;
    public float steepWalkingDiff = 0.5f; // how steep until the player is not allowed to walk?

    public bool isPushing;

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        canMove = true;
        isPushing = false;
        pos = transform.position;
        animator = gameObject.GetComponentInChildren<Animator>();
    }

    void Update()
    {
        velocity = (transform.position - pos) / Time.deltaTime;
        pos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveHorizontal();
       // AnimateWalking();
    }

    void MoveHorizontal()
    {
        if(canMove)// && !CanvasManager.isPaused)
        {
            // collect inputs
            inputMovement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            // calculate y difference
            if (canWalkTerrain(steepWalkingDiff))
            {
                // translate
                // rotate 45 degrees
                float rad45 = -45f * (Mathf.PI / 180f);
                Vector3 isoRotate = new Vector3(inputMovement.x * Mathf.Cos(rad45) - inputMovement.z * Mathf.Sin(rad45), inputMovement.y, inputMovement.x * Mathf.Sin(rad45) + inputMovement.z * Mathf.Cos(rad45));

                if(Input.GetKey(KeyCode.LeftShift))
                {
                    // running
                    //animator.SetBool("isSprinting", true); // do running anim if shift is down
                    transform.Translate(isoRotate * Time.deltaTime * moveSpeed * 3/2, Space.World);
                    //transform.Translate(inputMovement * Time.deltaTime * moveSpeed * 3 / 2, Space.World);
                }
                else
                {
                    // walking
                    //animator.SetBool("isSprinting", false); // turn off running is shift is not down
                    transform.Translate(isoRotate * Time.deltaTime * moveSpeed, Space.World);
                    //transform.Translate(inputMovement * Time.deltaTime * moveSpeed, Space.World);
                }
                
            }

            // rotate to look the appropriate direction
            if (inputMovement.x != 0 || inputMovement.z != 0)
            {
                transform.eulerAngles = Vector3.up * ((Mathf.Atan2(inputMovement.x, inputMovement.z) * Mathf.Rad2Deg) + 45f);
                //transform.eulerAngles = Vector3.up * ((Mathf.Atan2(inputMovement.x, inputMovement.z) * Mathf.Rad2Deg));
            }
                
        }
        else
        {
            inputMovement = Vector3.zero;

            //disable animations
            //animator.SetBool("isWalking", false);
            //animator.SetBool("isSprinting", false);
            //animator.SetBool("isPushing", false);
        }
        if(inputMovement.magnitude <= 0.01f)
            gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, CalculateYValueOfTerrain(), transform.position.z), Time.deltaTime * fakeGravityIntensity); // adjust y position
    }

    float CalculateYValueOfTerrain()
    {
        // cast a ray slightly in front of the player
        
        float yVal = 0f;

        RaycastHit hit;
        Ray ray = new Ray(gameObject.transform.position + new Vector3(0,2,0) + (gameObject.transform.forward * rayDisplacement), Vector3.down);
        //Debug.DrawLine(gameObject.transform.position + new Vector3(0, 2, 0) + (gameObject.transform.forward * rayDisplacement), gameObject.transform.position + (gameObject.transform.forward * rayDisplacement) + new Vector3(0, -10, 0));
        if (Physics.Raycast(ray, out hit, 100f, layerMask))
        {
            if (hit.collider != null && hit.transform.tag == "Terrain")
            {
                //Debug.Log("hit terrain. Y is " + hit.point.y);
                yVal = hit.point.y;
            }
        }
        return yVal;
    }

    bool canWalkTerrain(float maxDiff)
    {
        // cast a ray slightly in front of the player
        RaycastHit hit;
        Ray ray = new Ray(gameObject.transform.position + new Vector3(0, 4f, 0) + (gameObject.transform.forward * rayDisplacement*2), Vector3.down);
        //Debug.DrawLine(gameObject.transform.position + new Vector3(0, 4f, 0) + (gameObject.transform.forward * rayDisplacement*2), gameObject.transform.position + (gameObject.transform.forward * rayDisplacement) + new Vector3(0, -10, 0));
        if (Physics.Raycast(ray, out hit, 100f, layerMask))
        {
            if (hit.collider != null && hit.transform.tag == "Terrain")
            {
                //Debug.Log(hit.point.y - transform.position.y);
                if(hit.point.y - transform.position.y > maxDiff || hit.point.y - transform.position.y < -maxDiff) //if(Mathf.Abs(hit.point.y - transform.position.y) > maxDiff)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        return false;
    }

    void AnimateWalking()
    {
        if (canMove && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
            animator.SetBool("isWalking", true);
        else
            animator.SetBool("isWalking", false);

        animator.SetBool("isPushing", isPushing);
    }

    public void AnimatePickup()
    {
        StartCoroutine(PickupAnimation());
    }

    IEnumerator PickupAnimation()
    {
        animator.SetTrigger("triggerPickup");
        canMove = false;
        yield return new WaitForSeconds(1.1f);
        canMove = true;
    }
}
