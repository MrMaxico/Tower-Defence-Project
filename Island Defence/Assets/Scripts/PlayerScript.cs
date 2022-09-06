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
    public GameObject[] previewTowers;
    public GameObject[] towers;
    GameObject[] previewTags;
    GameObject previewTower;

    public Vector3[] towerOffsets;
    Vector3 rotation;
    Vector3 camrotation;
    Vector3 movement;

    public int currentSlot;

    public bool paused;
    bool previewSpawned;

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
        if (Physics.Raycast(transform.position, cam.GetComponent<Transform>().forward, out groundCheck, 4))
        {
            if (groundCheck.transform.gameObject.tag == "Floor")
            {
                if (Input.GetButtonDown("Fire1") && !paused)
                {
                    GameObject placed = Instantiate(towers[currentSlot], groundCheck.point + towerOffsets[currentSlot], transform.rotation);
                    placed.transform.Rotate(new Vector3(0, -90, 0));
                }
                else
                {
                    if (!previewSpawned)
                    {
                        previewTower = Instantiate(previewTowers[currentSlot], groundCheck.point + towerOffsets[currentSlot], transform.rotation);
                        previewTower.transform.Rotate(new Vector3(0, -90, 0));
                        previewSpawned = true;
                    }
                    else
                    {
                        previewTower.transform.position = groundCheck.point + towerOffsets[currentSlot];
                        previewTower.transform.rotation = transform.rotation;
                        previewTower.transform.Rotate(new Vector3(0, -90, 0));
                    }
                }
            }
            else if (groundCheck.transform.gameObject.tag != "Preview")
            {
                DestroyPreview();
            }
        }
        else
        {
            DestroyPreview();
        }

        //select tower
        if (Input.GetButtonDown("SlotOne"))
        {
            currentSlot = 1;
            DestroyPreview();
        }
        else if (Input.GetButtonDown("SlotTwo"))
        {
            currentSlot = 2;
            DestroyPreview();
        }
        else if (Input.GetButtonDown("SlotZero"))
        {
            currentSlot = 0;
            DestroyPreview();
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

    private void DestroyPreview()
    {
        Debug.Log("Trying to kill previews");
        previewTags = GameObject.FindGameObjectsWithTag("Preview");
        if (previewTags.Length > 0)
        {
            for (int i = 0; i < previewTags.Length; i++)
            {
                Destroy(previewTags[i]);
            }
        }
        previewSpawned = false;
    }
}
