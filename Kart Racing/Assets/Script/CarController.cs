using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Rigidbody rb;
    public float maxSpeed;

    public float forwardAccel = 8f, reverseAccel = 4f;
    public float turnStrength = 100f;

    private float speedInput;
    private float turnInput;

    // Start is called before the first frame update
    void Start()
    {
        rb.transform.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        speedInput = 0f;
        if (Input.GetAxis("Vertical") > 0)
        {
            speedInput = Input.GetAxis("Vertical") * forwardAccel;
        }
        else if(Input.GetAxis("Vertical") < 0)
        {
             speedInput = Input.GetAxis("Vertical") * reverseAccel;
        }
        turnInput = Input.GetAxis("Horizontal");
        if (Input.GetAxis("Vertical") != 0)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f,turnInput * turnStrength 
            * Time.deltaTime * Mathf.Sign(speedInput) * (rb.velocity.magnitude/maxSpeed) ,0f));
        }


        transform.position = rb.position;
    }
    
    private void FixedUpdate() 
    {
        
        rb.AddForce(transform.forward * speedInput * 500f);

        if(rb.velocity.magnitude > maxSpeed){
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
        Debug.Log(rb.velocity.magnitude);
    }
}
