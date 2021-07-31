using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;

    private Transform viewOwner;

    private const float MaxTurnY = 90.0f;
    private const float MinTurnY = -90.0f; 

    private float rotY = 0.0f;
    void Start()
    {
        viewOwner = transform.parent;
    }

    void Update()
    {
        float rotX = Input.GetAxis("Mouse X") * rotationSpeed;

        rotY -= Input.GetAxis("Mouse Y") * rotationSpeed;
        rotY = Mathf.Clamp(rotY, MinTurnY, MaxTurnY);


        viewOwner.eulerAngles += new Vector3(viewOwner.eulerAngles.x, rotX, 0);

        transform.eulerAngles = new Vector3(rotY, transform.eulerAngles.y, 0);
    }
}