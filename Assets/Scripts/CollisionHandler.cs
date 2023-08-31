using UnityEngine;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{
    int currentSceneIndex;

    [SerializeField] AudioClip crash;
    [SerializeField] AudioClip finish;

    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem collisionParticles;

    AudioSource audioSource;
    bool isTransitioning = false;
    public float volumeDecreaseAmount = 0.1f;

    void Start()
    {
        UpdateCurrentSceneIndex();
        audioSource = GetComponent<AudioSource>();
        //audioSource.volume -= volumeDecreaseAmount;
    }

    private void UpdateCurrentSceneIndex()
    {
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isTransitioning) { return; }

        switch (collision.gameObject.tag)
        {
            case "Start":

                break;
            case "Finish":
                CheckIfTransition();

                audioSource.PlayOneShot(finish);
                successParticles.Play();
                SetMovementDisabled();
                Invoke("LoadNextLevel", 1f);

                break;
            case "Fuel":

                break;
            default:
                CheckIfTransition();

                audioSource.PlayOneShot(crash);
                collisionParticles.Play();
                SetMovementDisabled();
                Invoke("ReloadLevel", 1f);
                break;
        }
    }

    private void CheckIfTransition()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
        isTransitioning = true;
        audioSource.Stop();
    }

    private void LoadNextLevel()
    {
        UpdateCurrentSceneIndex();
        currentSceneIndex += 1;

        if (currentSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            currentSceneIndex = 0; // reset to first level
        }

        SceneManager.LoadScene(currentSceneIndex);
    }

    private void SetMovementDisabled()
    {
        GetComponent<Movement>().enabled = false;
    }

    void ReloadLevel()
    {
        SceneManager.LoadScene(currentSceneIndex);
    }
}
