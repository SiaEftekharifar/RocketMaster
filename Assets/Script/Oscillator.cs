using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

[DisallowMultipleComponent]

public class Oscillator : MonoBehaviour
{

    [SerializeField] Vector3 movementVector;
    [SerializeField] float period=2f;

    Vector3 startingPos;
    Vector3 offSet;

    float movementFactor; 
   


    void Start() {
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update() {

        Movement();

    }

    private void Movement() {
        if (period <= Mathf.Epsilon) {return;}

        float cycle = Time.time / period;
        const float tau = Mathf.PI * 2;
        float rawSineWave = Mathf.Sin(cycle * tau);

        movementFactor = rawSineWave/2+0.5f;
        offSet = movementVector * movementFactor;
        transform.position = offSet + startingPos;
    }
}
