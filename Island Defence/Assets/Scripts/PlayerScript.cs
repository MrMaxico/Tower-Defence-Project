using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float sensitivity;
    public float speed;
    public float maxXRotation;
    float mouseVertical;

    public GameObject cam;
    public GameObject[] towers;

    public Vector3[] towerOffsets;
    Vector3 rotation;
    Vector3 camrotation;
    Vector3 movement;

    public int currentSlot;

    public bool paused;

    Rigidbody playerRB;

    RaycastHit groundCheck;

    private void Start()
    {
        playerRB = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        paused = true;
    }
    private void Update()
    {
        //camera
        rotation.y += Input.GetAxis("Mouse X") * sensitivity;
        transform.eulerAngles = rotation;
        camrotation.y = rotation.y;
        mouseVertical = Input.GetAxis("Mouse Y");
        if (camrotation.x < maxXRotation && camrotation.x > -maxXRotation)
        {
            camrotation.x -= mouseVertical * sensitivity;
        }
        else if (camrotation.x >= maxXRotation && mouseVertical > 0)
        {
            camrotation.x -= mouseVertical * sensitivity;
        }
        else if (camrotation.x <= -maxXRotation && mouseVertical < 0)
        {
            camrotation.x -= mouseVertical * sensitivity;
        }
        cam.transform.eulerAngles = camrotation;

        //place towers
        if (Input.GetButtonDown("Fire1") && !paused)
        {
            if (Physics.Raycast(transform.position, cam.GetComponent<Transform>().forward, out groundCheck, 7))
            {
                if (groundCheck.transform.gameObject.tag == "Floor")
                {
                    Debug.Log("Ground found");
                    GameObject placed = Instantiate(towers[currentSlot], groundCheck.point + towerOffsets[currentSlot], transform.rotation);
                    placed.transform.Rotate(new Vector3(0, -90, 0));
                }
            }
        }

        //select tower
        if (Input.GetButtonDown("SlotOne"))
        {
            currentSlot = 1;
        }
        else if (Input.GetButtonDown("SlotZero"))
        {
            currentSlot = 0;
        }

        //pause game
        if (Input.GetButtonDown("Pause"))
        {
            Cursor.lockState = CursorLockMode.None;
            paused = true;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Cursor.lockState = CursorLockMode.Locked;
            paused = false;
        }
    }

    private void FixedUpdate()
    {
        //movement
        movement.x = Input.GetAxis("Horizontal");
        movement.z = Input.GetAxis("Vertical");
        MovePlayer();
    }

    private void MovePlayer()
    {
        Vector3 moveVector = transform.TransformDirection(movement) * speed * Time.deltaTime;
        playerRB.velocity = new Vector3(moveVector.x, playerRB.velocity.y, moveVector.z);
    }
}
