using UnityEngine;

public class Movement : MonoBehaviour
{
    AudioSource audioSource;
    Rigidbody body;
    [SerializeField] float thrust = 1000f;
    [SerializeField] float rotationThrust = 400f;
    [SerializeField] AudioClip engineBooster;
    bool keepBoostRotation = false;

    [SerializeField] ParticleSystem mainBoosterParticles;
    [SerializeField] ParticleSystem leftBoosterParticles;
    [SerializeField] ParticleSystem rightBoosterParticles;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessThrust();
        ProcessRotation();
    }

    void ProcessThrust() { 
        if (Input.GetKey(KeyCode.W))
        {
            body.AddRelativeForce(Vector3.up * thrust * Time.deltaTime);
            HandleParticlesAndAudio();
        }
        else
        {
            audioSource.Stop();
            mainBoosterParticles.Stop();
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

    void ProcessRotation() {
        if (Input.GetKey(KeyCode.A))
        {
            CheckIfInGround();
            // goto left
            ApplyRotation(rotationThrust);
            if (!rightBoosterParticles.isPlaying)
            {
                rightBoosterParticles.Play();
            }
        } 
        else
        {
            rightBoosterParticles.Stop();
        }
        if (Input.GetKey(KeyCode.D))
        {
            CheckIfInGround();
            // goto right
            ApplyRotation(-rotationThrust);

            if (!leftBoosterParticles.isPlaying)
            { 
                leftBoosterParticles.Play();
            }
        }
        else
        {
            leftBoosterParticles.Stop();
        }
    }

    private void CheckIfInGround()
    {
        if (transform.rotation.z >= .69 || transform.rotation.z <= -.69 && transform.position.y < 1.2)
        {
            rotationThrust = 800;
            keepBoostRotation = true;
        } 
        else if (keepBoostRotation && 
            (
             (transform.rotation.z >= 0 && transform.rotation.z <= .10) || 
             (transform.rotation.z <= 0 && transform.rotation.z >= -.10)
            )
        ) 
        {
            keepBoostRotation = false;
        }
        else if (!keepBoostRotation)
        {
            rotationThrust = 200;
        }
        //Debug.Log("rotationThrust " + rotationThrust);
    }

    private void ApplyRotation(float rotation)
    {
        body.freezeRotation = true;
        transform.Rotate(Vector3.forward * rotation * Time.deltaTime);
        body.freezeRotation = false;
    }
}
