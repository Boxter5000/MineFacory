using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public bool isGrounded;
    public bool isSprinting;

    private Transform cam;
    private World world;

    [Header("Character movement")]
    public float walkSpeed = 3f;
    public float sprintSpeed = 6f;
    public float jumpForce = 5f;
    public float gravity = -9.8f;
    public float lookSpeed = 5f;
    
    [Header("Character collision")]
    public float playerWidth = 0.15f;

    private float horizontal;
    private float vertical;
    private float mouseHorizontal;
    private float mouseVertical;
    private Vector3 velocity;
    private float verticalMomentum = 0;
    private bool jumpRequest;

    
    private void Start() {
        

        cam = GameObject.Find("Main Camera").transform;
        world = GameObject.Find("World").GetComponent<World>();

    }

    private void FixedUpdate() {
        
        CalculateVelocity();
        if (jumpRequest)
            Jump();

        transform.Rotate(Vector3.up * mouseHorizontal);
        cam.Rotate(Vector3.right * -mouseVertical);
        transform.Translate(velocity, Space.World);

    }

    private void Update() {

        GetPlayerInputs();

    }

    void Jump () {

        verticalMomentum = jumpForce;
        isGrounded = false;
        jumpRequest = false;

    }

    private void CalculateVelocity () {

        // Affect vertical momentum with gravity.
        if (verticalMomentum > gravity)
        {
            if (!isGrounded)
            {
                verticalMomentum += Time.fixedDeltaTime * gravity;
            }
            else
            {
                verticalMomentum = 0f;
            }
        }

        

        

        // if we're sprinting, use the sprint multiplier.
        if (isSprinting)
            velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * sprintSpeed;
        else
            velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * walkSpeed;

        // Apply vertical momentum (falling/jumping).
        velocity += Vector3.up * verticalMomentum * Time.fixedDeltaTime;

        if ((velocity.z > 0 && front) || (velocity.z < 0 && back))
            velocity.z = 0;
        if ((velocity.x > 0 && right) || (velocity.x < 0 && left))
            velocity.x = 0;



        if (Mathf.Abs(velocity.x)  > 0|| Mathf.Abs(velocity.z) > 0)
            checkDownSpeed(velocity.y - 0.1f);

        
        if (velocity.y < 0 )
            velocity.y = checkDownSpeed(velocity.y);
        else if (velocity.y > 0)
            velocity.y = checkUpSpeed(velocity.y);


    }

    private void GetPlayerInputs () {

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        mouseHorizontal = Input.GetAxis("Mouse X") * lookSpeed;
        mouseVertical = Input.GetAxis("Mouse Y") * lookSpeed;

        if (Input.GetButtonDown("Sprint"))
            isSprinting = true;
        if (Input.GetButtonUp("Sprint"))
            isSprinting = false;

        if (isGrounded && Input.GetButtonDown("Jump"))
            jumpRequest = true;

    }

    private float checkDownSpeed (float downSpeed) {

        if (
            world.CheckForVoxel(transform.position.x - playerWidth + .1f, transform.position.y + downSpeed, transform.position.z - playerWidth + .1f) ||
            world.CheckForVoxel(transform.position.x + playerWidth - .1f, transform.position.y + downSpeed, transform.position.z - playerWidth + .1f) ||
            world.CheckForVoxel(transform.position.x + playerWidth - .1f, transform.position.y + downSpeed, transform.position.z + playerWidth - .1f) ||
            world.CheckForVoxel(transform.position.x - playerWidth + .1f, transform.position.y + downSpeed, transform.position.z + playerWidth - .1f)
           ) {

            isGrounded = true;
            return 0;

        } else {

            isGrounded = false;
            return downSpeed;

        }

    }

    private float checkUpSpeed (float upSpeed) {

        if (
            world.CheckForVoxel(transform.position.x - playerWidth + .1f, transform.position.y + upSpeed, transform.position.z - playerWidth + .1f) ||
            world.CheckForVoxel(transform.position.x + playerWidth - .1f, transform.position.y + upSpeed, transform.position.z - playerWidth + .1f) ||
            world.CheckForVoxel(transform.position.x + playerWidth - .1f, transform.position.y + upSpeed, transform.position.z + playerWidth - .1f) ||
            world.CheckForVoxel(transform.position.x - playerWidth + .1f, transform.position.y + upSpeed, transform.position.z + playerWidth - .1f)
           ) {

            return 0;

        } else {

            return upSpeed;

        }

    }

    public bool front {

        get {
            if (
                world.CheckForVoxel(transform.position.x + playerWidth - 0.08f, transform.position.y, transform.position.z + playerWidth) ||
                world.CheckForVoxel(transform.position.x - playerWidth + 0.08f, transform.position.y, transform.position.z + playerWidth) ||
                world.CheckForVoxel(transform.position.x + playerWidth - 0.08f, transform.position.y + 1f, transform.position.z + playerWidth) ||
                world.CheckForVoxel(transform.position.x - playerWidth + 0.08f, transform.position.y + 1f, transform.position.z + playerWidth)
                )
                return true;
            else
                return false;
        }

    }
    public bool back {

        get {
            if (
                world.CheckForVoxel(transform.position.x + playerWidth - 0.08f, transform.position.y, transform.position.z - playerWidth) ||
                world.CheckForVoxel(transform.position.x - playerWidth + 0.08f, transform.position.y, transform.position.z - playerWidth) ||
                world.CheckForVoxel(transform.position.x + playerWidth - 0.08f, transform.position.y + 1f, transform.position.z - playerWidth) ||
                world.CheckForVoxel(transform.position.x - playerWidth + 0.08f, transform.position.y + 1f, transform.position.z - playerWidth)
                )
                return true;
            else
                return false;
        }

    }
    public bool left {

        get {
            if (
                world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y, transform.position.z + playerWidth - 0.08f) ||
                world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y, transform.position.z - playerWidth + 0.08f) ||
                world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y + 1f, transform.position.z + playerWidth - 0.08f) ||
                world.CheckForVoxel(transform.position.x - playerWidth, transform.position.y + 1f, transform.position.z - playerWidth + 0.08f)
                )
                return true;
            else
                return false;
        }

    }
    public bool right {

        get {
            if (
                world.CheckForVoxel(transform.position.x + playerWidth, transform.position.y, transform.position.z + playerWidth - 0.08f) ||
                world.CheckForVoxel(transform.position.x + playerWidth, transform.position.y, transform.position.z - playerWidth + 0.08f) ||
                world.CheckForVoxel(transform.position.x + playerWidth, transform.position.y + 1f, transform.position.z + playerWidth - 0.08f) ||
                world.CheckForVoxel(transform.position.x + playerWidth, transform.position.y + 1f, transform.position.z - playerWidth + 0.08f)
                )
                return true;
            else
                return false;
        }

    }

}