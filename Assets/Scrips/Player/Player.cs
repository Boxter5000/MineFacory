using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public bool isGrounded;
    public bool isSprinting;

    private Transform cam;
    private World world;

    public float walkSpeed = 3f;
    public float sprintSpeed = 6f;
    public float jumpForce = 5f;
    public float gravity = -9.8f;

    public float playerWidth = 0.15f;
    public float boundsTolerance = 0.1f;
    public float playerHeight = 1.8f;

    private float horizontal;
    private float vertical;
    private float mouseHorizontal;
    private float mouseVertical;
    private Vector3 velocity;
    private float verticalMomentum = 0;
    private bool jumpRequest;

    public Transform highlightBlock;
    public Transform placeBlock;
    public float checkIncrement = 0.1f;
    public float reach = 8f;
    
    public byte selectedBlockIndex = 1;
    
    private const float MaxTurnY = 89.9f;
    private const float MinTurnY = -89.9f; 
    [SerializeField] private float rotationSpeed = 10f;
    
    private float rotY = 0.0f;

    private void Start() {

        cam = GameObject.Find("Main Camera").transform;
        world = GameObject.Find("World").GetComponent<World>();

        Cursor.lockState = CursorLockMode.Locked;

    }

    private void FixedUpdate() {
        
        CalculateVelocity();
        if (jumpRequest)
            Jump();

        //transform.Rotate(Vector3.up * mouseHorizontal);
        //scam.Rotate(Vector3.right * -mouseVertical);
        transform.Translate(velocity, Space.World);

    }

    private void Update() {

        GetPlayerInputs();
        placeCursorBlocks();
        

       
        mouseVertical = Mathf.Clamp(mouseVertical, MinTurnY, MaxTurnY);
        transform.eulerAngles += new Vector3(transform.eulerAngles.x, mouseHorizontal, 0);
        cam.eulerAngles = new Vector3(mouseVertical, cam.eulerAngles.y, 0);
    }

    void Jump () {

        verticalMomentum = jumpForce;
        isGrounded = false;
        jumpRequest = false;

    }

    private void CalculateVelocity () {

        // Affect vertical momentum with gravity.
        if (verticalMomentum > gravity)
            verticalMomentum += Time.fixedDeltaTime * gravity;

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

        if (velocity.y < 0)
            velocity.y = checkDownSpeed(velocity.y);
        else if (velocity.y > 0)
            velocity.y = checkUpSpeed(velocity.y);


    }

    private void GetPlayerInputs () {

        if (Input.GetButtonDown("Inventory") || (world.isInventoryOpen && Input.GetKeyDown(KeyCode.Escape)))
        {
            world.isInventoryOpen = !world.isInventoryOpen;
        }
        
        if (!world.isInventoryOpen)
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
            mouseHorizontal = Input.GetAxis("Mouse X") * rotationSpeed;
            mouseVertical -= Input.GetAxis("Mouse Y") * rotationSpeed;

            if (Input.GetButtonDown("Sprint"))
                isSprinting = true;
            if (Input.GetButtonUp("Sprint"))
                isSprinting = false;

            if (isGrounded && Input.GetButtonDown("Jump"))
                jumpRequest = true;

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            Cursor.lockState = CursorLockMode.Locked;
            if (highlightBlock.gameObject.activeSelf) {

                // Destroy block.
                if (Input.GetMouseButtonDown(0))
                    world.GetChunkFromVector3(highlightBlock.position).EditVoxel(highlightBlock.position, 0);

                // Place block.
                if (Input.GetMouseButtonDown(1))
                    world.GetChunkFromVector3(placeBlock.position).EditVoxel(placeBlock.position, selectedBlockIndex);

            
            }
        }
        else
        {
            horizontal = 0;
            vertical = 0;
            mouseHorizontal = 0;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    private void placeCursorBlocks () {

        float step = checkIncrement;
        Vector3 lastPos = new Vector3();

        while (step < reach) {

            Vector3 pos = cam.position + (cam.forward * step);

            if (world.CheckForVoxel(pos)) {

                highlightBlock.position = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
                placeBlock.position = lastPos;

                highlightBlock.gameObject.SetActive(true);
                placeBlock.gameObject.SetActive(true);

                return;

            }

            lastPos = new Vector3(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));

            step += checkIncrement;

        }

        highlightBlock.gameObject.SetActive(false);
        placeBlock.gameObject.SetActive(false);

    }

    private float checkDownSpeed (float downSpeed) {

        if (
            world.CheckForVoxel(new Vector3(transform.position.x - playerWidth + .1f, transform.position.y + downSpeed, transform.position.z - playerWidth + .1f)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + playerWidth - .1f, transform.position.y + downSpeed, transform.position.z - playerWidth + .1f)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + playerWidth - .1f, transform.position.y + downSpeed, transform.position.z + playerWidth - .1f)) ||
            world.CheckForVoxel(new Vector3(transform.position.x - playerWidth + .1f, transform.position.y + downSpeed, transform.position.z + playerWidth - .1f))
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
            world.CheckForVoxel(new Vector3(transform.position.x - playerWidth + .1f, transform.position.y + upSpeed + playerHeight, transform.position.z - playerWidth + .1f)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + playerWidth - .1f, transform.position.y + upSpeed + playerHeight, transform.position.z - playerWidth + .1f)) ||
            world.CheckForVoxel(new Vector3(transform.position.x + playerWidth - .1f, transform.position.y + upSpeed + playerHeight, transform.position.z + playerWidth - .1f)) ||
            world.CheckForVoxel(new Vector3(transform.position.x - playerWidth + .1f, transform.position.y + upSpeed + playerHeight, transform.position.z + playerWidth - .1f))
           ) {

            return 0;

        } else {

            return upSpeed;

        }

    }

    public bool front {

        get {
            if (
                world.CheckForVoxel(new Vector3(transform.position.x + playerWidth - 0.08f, transform.position.y, transform.position.z + playerWidth)) ||
                world.CheckForVoxel(new Vector3(transform.position.x - playerWidth + 0.08f, transform.position.y, transform.position.z + playerWidth)) ||
                world.CheckForVoxel(new Vector3(transform.position.x + playerWidth - 0.08f, transform.position.y + playerHeight, transform.position.z + playerWidth)) ||
                world.CheckForVoxel(new Vector3(transform.position.x - playerWidth + 0.08f, transform.position.y + playerHeight, transform.position.z + playerWidth))
                )
                return true;
            else
                return false;
        }

    }
    public bool back {

        get {
            if (
                world.CheckForVoxel(new Vector3(transform.position.x + playerWidth - 0.08f, transform.position.y, transform.position.z - playerWidth)) ||
                world.CheckForVoxel(new Vector3(transform.position.x - playerWidth + 0.08f, transform.position.y, transform.position.z - playerWidth)) ||
                world.CheckForVoxel(new Vector3(transform.position.x + playerWidth - 0.08f, transform.position.y + playerHeight, transform.position.z - playerWidth)) ||
                world.CheckForVoxel(new Vector3(transform.position.x - playerWidth + 0.08f, transform.position.y + playerHeight, transform.position.z - playerWidth))
                )
                return true;
            else
                return false;
        }

    }
    public bool left {

        get {
            if (
                world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y, transform.position.z + playerWidth - 0.08f)) ||
                world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y, transform.position.z - playerWidth + 0.08f)) ||
                world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + playerHeight, transform.position.z + playerWidth - 0.08f)) ||
                world.CheckForVoxel(new Vector3(transform.position.x - playerWidth, transform.position.y + playerHeight, transform.position.z - playerWidth + 0.08f))
                )
                return true;
            else
                return false;
        }

    }
    public bool right {

        get {
            if (
                world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y, transform.position.z + playerWidth - 0.08f)) ||
                world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y, transform.position.z - playerWidth + 0.08f)) ||
                world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + playerHeight, transform.position.z + playerWidth - 0.08f)) ||
                world.CheckForVoxel(new Vector3(transform.position.x + playerWidth, transform.position.y + playerHeight, transform.position.z - playerWidth + 0.08f))
                )
                return true;
            else
                return false;
        }

    }

}