using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunnlightRotation : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private bool isDaySycleActive;

    [SerializeField] private Light[] globalLight;

    // Update is called once per frame
    void Update()
    {
        if(isDaySycleActive)
            transform.Rotate(Vector3.right * (rotationSpeed * Time.deltaTime));
    }
}
