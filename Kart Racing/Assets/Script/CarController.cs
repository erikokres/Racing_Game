using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Rigidbody rb;
    public float maxSpeed;

    public float forwardAccel = 8f, reverseAccel = 4f;
    public float turnStrength = 100f;

    public Transform groundRayPoint, groundRayPoint2;
    public LayerMask whatIsGround;
    public float groundRayLength = 0.75f;
    public float gravityMod =10f;

    public Transform leftFrontWheel, rightFrontWheel;
    public float maxWheelTurn = 25f;
    public ParticleSystem[] dustTrail;
    public float maxEmission = 25f, emissionFadeSpeed = 20f;
    private float emissionRate;

    private float speedInput;
    private float turnInput;
    private bool grounded;
    private float dragOnGround;


    // Start is called before the first frame update
    void Start()
    {
        rb.transform.parent = null;

        dragOnGround = rb.drag;

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
        if (grounded && Input.GetAxis("Vertical") != 0)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f,turnInput * turnStrength 
            * Time.deltaTime * Mathf.Sign(speedInput) * (rb.velocity.magnitude/maxSpeed) ,0f));
        }

        //turning the wheels
        leftFrontWheel.localRotation = Quaternion.Euler(leftFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn) - 180, leftFrontWheel.localRotation.eulerAngles.z);
        rightFrontWheel.localRotation = Quaternion.Euler(rightFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn), rightFrontWheel.localRotation.eulerAngles.z);


        transform.position = rb.position;

        //Controll particles emission
        emissionRate = Mathf.MoveTowards(emissionRate, 0f, emissionFadeSpeed * Time.deltaTime);

        if(grounded && (Mathf.Abs(turnInput) > 0.5f || (rb.velocity.magnitude < maxSpeed * 0.5f && rb.velocity.magnitude != 0))){
            emissionRate = maxEmission;
        }

        if(rb.velocity.magnitude <= 0.5f){
            emissionRate = 0;
        }

        for (int i = 0; i < dustTrail.Length; i++)
        {
            var emissionModule = dustTrail[i].emission;

            emissionModule.rateOverTime = emissionRate;
        }
    }
    
    private void FixedUpdate() 
    {

        grounded = false;

        RaycastHit hit;
        Vector3 normalTarget = Vector3.zero;

        if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            grounded = true;

            normalTarget = hit.normal;
        }

        //when on ground rotate to the normal
        if(grounded){
            transform.rotation = Quaternion.FromToRotation(transform.up, normalTarget)*transform.rotation;
        }

        if(Physics.Raycast(groundRayPoint2.position, -transform.up, out hit, groundRayLength, whatIsGround)){
            grounded = true;

            normalTarget = (normalTarget + hit.normal) /2f;
        }
        

        //accelerate the car
        if(grounded)
        {
            rb.AddForce(transform.forward * speedInput * 500f);
            rb.drag = dragOnGround;
        }else
        {
            rb.drag = 0.1f;

            rb.AddForce(-Vector3.up * gravityMod *100f);
        }

        if(rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        
    }
}
