using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    public float sensitivity;
    public float speed;
    public float maxXRotation;
    float mouseVertical;

    public GameObject cam;
    public GameObject tooPoorPopup;
    public GameObject upgradePopup;
    public GameObject[] previewTowers;
    public GameObject[] towers;
    GameObject[] previewTags;
    GameObject previewTower;

    public Vector3[] towerOffsets;
    Vector3 rotation;
    Vector3 camrotation;
    Vector3 movement;

    public int[] towerPrices;
    public int currentSlot;
    public int money;

    public bool paused;
    bool previewSpawned;

    public Text moneyDisplay;
    public Text upgradeCostDisplay;

    Rigidbody playerRB;

    RaycastHit groundCheck;

    private void Start()
    {
        playerRB = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        paused = true;
        AcceptPoorness(tooPoorPopup);
    }
    private void Update()
    {
        //camera
        if (!paused && !tooPoorPopup.activeSelf && !upgradePopup.activeSelf)
        {
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
        }

        //place and upgrade towers
        if (Physics.Raycast(transform.position, cam.GetComponent<Transform>().forward, out groundCheck, 4))
        {
            if (groundCheck.transform.gameObject.tag == "Floor")
            {
                if (Input.GetButtonDown("Fire1") && !paused && towerPrices[currentSlot] <= money && !tooPoorPopup.activeSelf && !upgradePopup.activeSelf)
                {
                    money -= towerPrices[currentSlot];
                    GameObject placed = Instantiate(towers[currentSlot], groundCheck.point + towerOffsets[currentSlot], transform.rotation);
                    placed.transform.Rotate(new Vector3(0, -90, 0));
                }
                else if (Input.GetButtonDown("Fire1") && !paused && towerPrices[currentSlot] > money)
                {
                    Debug.Log("You poor");
                    tooPoorPopup.SetActive(true);
                    Cursor.lockState = CursorLockMode.None;
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
            else if (groundCheck.transform.gameObject.tag == "Totem")
            {
                if (Input.GetButtonDown("Fire2") && !tooPoorPopup.activeSelf && !upgradePopup.activeSelf)
                {
                    upgradePopup.SetActive(true);
                    Cursor.lockState = CursorLockMode.None;
                }
                DestroyPreview();
            }
            else if (groundCheck.transform.gameObject.tag == "Tower" && !tooPoorPopup.activeSelf && !upgradePopup.activeSelf)
            {
                if (Input.GetButtonDown("Fire2"))
                {
                    upgradePopup.SetActive(true);
                    upgradeCostDisplay.text = $"Upgrade? Cost: {groundCheck.transform.gameObject.GetComponent<TowerValues>().upgradeCost}";
                    Cursor.lockState = CursorLockMode.None;
                }
                DestroyPreview();
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
        else if (Input.GetButtonDown("SlotThree"))
        {
            currentSlot = 3;
            DestroyPreview();
        }
        else if (Input.GetButtonDown("SlotZero"))
        {
            currentSlot = 0;
            DestroyPreview();
        }

        int lastFrameSlot = currentSlot;
        currentSlot += Mathf.RoundToInt(Input.mouseScrollDelta.y);
        if (currentSlot > towers.Length - 1)
        {
            currentSlot = 0;
            DestroyPreview();
        }
        else if (currentSlot < 0)
        {
            currentSlot = towers.Length - 1;
            DestroyPreview();
        }
        else if (lastFrameSlot != currentSlot)
        {
            DestroyPreview();
        }

        //pause game
        if (Input.GetButtonDown("Pause"))
        {
            Cursor.lockState = CursorLockMode.None;
            paused = true;
        }

        if (Input.GetButtonDown("Fire1") && !tooPoorPopup.activeSelf && !upgradePopup.activeSelf)
        {
            Cursor.lockState = CursorLockMode.Locked;
            paused = false;
        }

        //display money
        moneyDisplay.text = $"Gold: {money}";
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
        // Debug.Log("Trying to kill previews"); // if you enable this the console will be spammed
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

    public void AcceptPoorness(GameObject popup)
    {
        popup.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Upgrade(GameObject popup)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject targetTower = player.GetComponent<PlayerScript>().groundCheck.transform.gameObject;
        if (player.GetComponent<PlayerScript>().money >= targetTower.GetComponent<TowerValues>().upgradeCost)
        {
            player.GetComponent<PlayerScript>().money -= targetTower.GetComponent<TowerValues>().upgradeCost;
            targetTower.GetComponent<TowerValues>().upgradeCost += targetTower.GetComponent<TowerValues>().inflation;
            targetTower.GetComponent<TowerValues>().level += 1;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            player.GetComponent<PlayerScript>().tooPoorPopup.SetActive(true);
        }
        popup.SetActive(false);
    }

    public void Decline(GameObject popup)
    {
        popup.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }
}
