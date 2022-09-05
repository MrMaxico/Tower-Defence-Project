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

    Vector3 rotation;
    Vector3 camrotation;
    Vector3 movement;

    public int currentSlot;

    Rigidbody playerRB;

    RaycastHit groundCheck;

    private void Start()
    {
        playerRB = GetComponent<Rigidbody>();
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
        if (Input.GetButtonDown("Fire1"))
        {
            if (Physics.Raycast(transform.position, rotation, out groundCheck, 500f))
            {
                Debug.Log("Ground found");
                Instantiate(towers[currentSlot], groundCheck.point, Quaternion.identity);
            }
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
