using System.Collections;
using UnityEngine;

public class MovementOniria : MonoBehaviour
{
    AudioSource audioSource;
    Rigidbody firstStageBody;
    Rigidbody secondStageBody;
    Rigidbody parachutesRigidBody;

    [SerializeField] float thrust = 250f;
    [SerializeField] AudioClip engineBooster;
    [SerializeField] ParticleSystem mainBoosterParticles;
    [SerializeField] ParticleSystem secondStageBoosterParticles;
    private bool launched = false;


    private MeshRenderer parachuteMeshRenderer;
    private GameObject firstStage;
    private GameObject secondStage;
    private GameObject parachutes;
    private FixedJoint fixedJoint;


    private float timer = 0.0f;
    private float targetTime = 5.0f; // Time in seconds

    private float firstStageMaxHeight = 0f;
    private float secondStageMaxHeight = 0f;

    private string engineState = "on";



    // ROTATION
    //private float rotationDuration = 2.0f; // Duration in seconds

    //private Quaternion targetRotation;
    //private Quaternion initialRotation;
    //private float startTime;
    private bool falling = false;

    // TRAJECTORY
    //private float initialLaunchForce = 20f;   // Initial force applied to the rocket
    //private float gravity = 9.81f;            // Gravity value
    [SerializeField] private Transform[] targets;
    private Transform target;

    void Start()
    {
        parachutes = transform.Find("Parachutes").gameObject;
        parachuteMeshRenderer = parachutes.GetComponent<MeshRenderer>();

        firstStage = transform.Find("FirstStage").gameObject;
        secondStage = transform.Find("SecondStage").gameObject;
        
        firstStageBody = firstStage.GetComponent<Rigidbody>();
        secondStageBody = secondStage.GetComponent<Rigidbody>();
        parachutesRigidBody = parachutes.GetComponent<Rigidbody>();
        
        audioSource = GetComponent<AudioSource>();

        fixedJoint = secondStage.GetComponent<FixedJoint>();

        // rotation
        //initialRotation = firstStage.transform.rotation;
        //targetRotation = initialRotation * Quaternion.Euler(180, 0, 0);
        //startTime = Time.time;

        // get a random target
        target = targets[Random.Range(0, targets.Length)];


        secondStageBoosterParticles.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessThrust();
    }

    void Rotate()
    {

        //float elapsedTime = Time.time - startTime;
        //float t = Mathf.Clamp01(elapsedTime / rotationDuration); // Normalize time between 0 and 1

        //firstStage.transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, t);

        parachuteMeshRenderer.enabled = true;

        parachutesRigidBody.drag = 10;
    }


    void ProcessThrust() {
        launched = true;
        timer += Time.deltaTime;

        if (timer <= targetTime)
        {

            ApplyThrust(firstStageBody);
            HandleParticlesAndAudio();
        }
        else
        {
            // start counting after fuel is finished

            if (engineState == "on")
            {
                Debug.Log("5 seconds have passed. Shut engines off, no more fuel!");

                engineState = "off";
                audioSource.Stop();
                mainBoosterParticles.Stop();

                secondStageBoosterParticles.Play();

                Destroy(fixedJoint);
                secondStageBody.drag = 3;

            }
            if (thrust > 10000)
            {
                thrust = 6000;
            }

            if (secondStage.transform.rotation.x != -90)
            {
                float maxAngle = -90;

                float targetAngle = Mathf.Lerp(secondStage.transform.rotation.eulerAngles.x, maxAngle, Time.deltaTime * .5f);
                secondStage.transform.rotation = Quaternion.Euler(targetAngle, 0f, 0f);

            }

            ApplyThrust(secondStageBody);

            StartCoroutine(WaitAndMakeFallSequence());
        }
        GetFirstStageMaxHeight();
        GetSecondStageMaxHeight();


    }

    private void GetSecondStageMaxHeight()
    {
        if (secondStage.transform.position.y > secondStageMaxHeight)
        {
            secondStageMaxHeight = secondStage.transform.position.y;
            Debug.Log("Second stage max height => " + secondStageMaxHeight);
        }
    }

    private void GetFirstStageMaxHeight()
    {
        if (firstStage.transform.position.y > firstStageMaxHeight)
        {
            firstStageMaxHeight = firstStage.transform.position.y;
            Debug.Log("First stage max height => " + secondStageMaxHeight);
        }
    }

    private void ApplyThrust(Rigidbody rb)
    {
        rb.AddForce(Vector3.up * thrust * Time.deltaTime);
    }

    private IEnumerator WaitAndMakeFallSequence()
    {
        yield return new WaitForSeconds(4f);
        if (firstStageBody.velocity.y <= 0 && !falling)
        {
            Rotate();
            falling = true;
        }
    }


    private void HandleParticlesAndAudio()
    {
        if (!mainBoosterParticles.isPlaying)
        {
            mainBoosterParticles.Play();
        }
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(engineBooster);
        }
    }


    private void FixedUpdate()
    {
        if (launched)
        {
            firstStageBody.constraints = RigidbodyConstraints.None;

            Vector3 toTarget = target.position - firstStage.transform.position;

            // Apply gravity
            Vector3 gravityForce = Vector3.down * Physics.gravity.magnitude * .001f;
            firstStageBody.AddForce(gravityForce, ForceMode.Acceleration);

            // Calculate direction and distance to the target
            Vector3 directionToTarget = toTarget.normalized;
            float distanceToTarget = toTarget.magnitude;

            float velocityIntensity = 1f;
            if (timer >= targetTime && velocityIntensity != 2f)
            {
                velocityIntensity = 2f;
            }
            // Calculate the rocket's velocity required to reach the target
            float timeToImpact = Mathf.Sqrt((velocityIntensity * distanceToTarget) / -Physics.gravity.y);
            Vector3 requiredVelocity = directionToTarget * (distanceToTarget / timeToImpact);

            // Adjust the rocket's velocity
            firstStageBody.velocity = requiredVelocity;


            // Rotate towards target
            Vector3 direction = (target.position - firstStage.transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Quaternion currentRotation = firstStageBody.rotation;

            float intensity = .2f;
            if (timer >= targetTime && intensity != .6f)
            {
                intensity = .7f;
            }
            float t = Mathf.Clamp01(intensity * Time.deltaTime);
            Quaternion newRotation = Quaternion.Slerp(currentRotation, targetRotation, t);

            firstStageBody.MoveRotation(newRotation);
            ApplyWindForce(firstStageBody);

            firstStageBody.constraints =
                RigidbodyConstraints.FreezeRotationX
                | RigidbodyConstraints.FreezeRotationZ
                | RigidbodyConstraints.FreezeRotationY
            ;

            parachutesRigidBody.drag += Time.deltaTime;
            // Check if the rocket is "close enough" to the target
            if (distanceToTarget < 1f)
            {
                firstStageBody.velocity = Vector3.zero;
                launched = false;
            }
        }
    }

    private static void ApplyWindForce(Rigidbody rb)
    {
        Vector3 windDirection = Random.insideUnitSphere.normalized;
        Vector3 windForce = windDirection * Random.Range(.5f, 1.5f);
        rb.AddForce(windForce, ForceMode.Force);
    }

    private void ApplyRotation(float rotation)
    {
        firstStageBody.freezeRotation = true;
        transform.Rotate(Vector3.forward * rotation * Time.deltaTime);
        firstStageBody.freezeRotation = false;
    }
}
