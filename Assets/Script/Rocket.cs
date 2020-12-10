using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

    [SerializeField] float thrustLevel;
    [SerializeField] float rotationThrust;
    [SerializeField] float levelDelay;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip winSound;


    [SerializeField] ParticleSystem mainEngineEffect;
    [SerializeField] ParticleSystem deathEffect;
    [SerializeField] ParticleSystem winEffect;

    AudioSource audioSource;
    Rigidbody rocketRigidBody;

    enum State {
        Alive,
        Dying,
        Trancending
    }
    
    State state = State.Alive;

    int currentSceneIndex;

    bool collisionsAreEnabled = true;

    // Start is called before the first frame update
    void Start() {
        rocketRigidBody = GetComponent<Rigidbody>();

        audioSource = GetComponent<AudioSource>();

         currentSceneIndex =SceneManager.GetActiveScene().buildIndex;
    }

    // Update is called once per frame
    void Update() {

        if (!(state == State.Dying)) {

            RespondToThrustInput();
            RespondToRotateInput();
        }

        if (Debug.isDebugBuild) {
            RespondToDebugKey();
        }





    }

    private void RespondToDebugKey() {
        if (Input.GetKeyDown(KeyCode.L)) {
            LoadNextScene();
        }

        if (Input.GetKeyDown(KeyCode.C)) {
            collisionsAreEnabled = !collisionsAreEnabled;
        }

    }

    private void RespondToThrustInput() {

        if (Input.GetKey(KeyCode.Space)) {
            ApplyThrust();
        }
        else {
            audioSource.Stop();
            mainEngineEffect.Stop();
        }
    }

    private void ApplyThrust() {

        float thrustLevelThisFrame = thrustLevel * Time.deltaTime;
        rocketRigidBody.AddRelativeForce(thrustLevelThisFrame * Vector3.up);
        mainEngineEffect.Play();
        if (!audioSource.isPlaying) {

            audioSource.PlayOneShot(mainEngine);
        }
    }

    private void RespondToRotateInput() {
        ApplyRotation();
    }

    private void ApplyRotation() {

        float rotationThrustThisFrame = rotationThrust * Time.deltaTime;
        rocketRigidBody.angularVelocity = Vector3.zero;
        
        if (Input.GetKey(KeyCode.A)) {

            transform.Rotate(Vector3.forward * rotationThrustThisFrame);
        }
        else if (Input.GetKey(KeyCode.D)) {

            transform.Rotate(-Vector3.forward * rotationThrustThisFrame);
        }
    }

    void OnCollisionEnter(Collision collision) {

        if (!(state == State.Alive) || !collisionsAreEnabled) { return; }

        switch (collision.gameObject.tag) {

            case "Friendly":
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }


    private void StartSuccessSequence() {
        state = State.Trancending;
        audioSource.Stop();
        audioSource.PlayOneShot(winSound);
        winEffect.Play();
        Invoke("LoadNextScene", levelDelay);
    }

    private void StartDeathSequence() {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(deathSound);
        deathEffect.Play();
        Invoke("LoadFirstScene", levelDelay);
    }

    private void LoadNextScene() {

        int nextSceneIndex = ++currentSceneIndex;

        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings) {
            nextSceneIndex = 0;
        }
      
            SceneManager.LoadScene(nextSceneIndex);
        
    }

    private void LoadFirstScene() {
        SceneManager.LoadScene(currentSceneIndex);
        state = State.Alive;
    }

 
}
