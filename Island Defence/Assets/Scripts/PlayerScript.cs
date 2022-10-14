using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PlayerScript : MonoBehaviour
{
    public float sensitivity;
    public float speed;
    public float maxXRotation;
    float mouseVertical;
    public GameObject chest;
    public GameObject cam;
    public GameObject tooPoorPopup;
    public GameObject upgradePopup;
    public GameObject rangePreview;
    public GameObject[] previewTowers;
    public GameObject[] towers;
    GameObject[] previewTags;
    GameObject previewTower;
    public GameObject rotatingTower;
    public GameObject upradeParticles;
    public GameObject towerTips;

    public Transform[] mineToChestRoute;

    public Vector3[] towerOffsets;
    Vector3 rotation;
    Vector3 camrotation;
    Vector3 movement;

    public int[] towerPrices;
    public int currentSlot;
    public int money;

    public bool paused;
    bool previewSpawned;
    bool previewIsRange;
    bool rotating;

    public TextMeshProUGUI moneyDisplay;
    public TextMeshProUGUI upgradeCostDisplay;

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
        //check the gems
        GameObject[] gems = GameObject.FindGameObjectsWithTag("Gem");

        if (chest.GetComponent<Chest>().gemsLeft <= 0 && gems.Length == 0)
        {
            Debug.Log("Game over!");
            SceneManager.LoadScene("Main Menu");
        }

        //insta-ultra-kill
        if (Input.GetKeyDown(KeyCode.K))
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            for (int i = 0; i < enemies.Length; i++)
            {
                Destroy(enemies[i]);
            }
        }
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

        //place, upgrade and rotate towers
        if (Physics.Raycast(transform.position, cam.GetComponent<Transform>().forward, out groundCheck, 4))
        {
            if (groundCheck.transform.gameObject.tag == "Floor" && currentSlot != 4)
            {
                towerTips.SetActive(false);
                if (previewIsRange)
                {
                    DestroyPreview();
                    previewIsRange = false;
                }

                if (Input.GetButtonDown("Fire1") && !paused && towerPrices[currentSlot] <= money && !tooPoorPopup.activeSelf && !upgradePopup.activeSelf)
                {
                    money -= towerPrices[currentSlot];
                    GameObject placed = Instantiate(towers[currentSlot], groundCheck.point + towerOffsets[currentSlot], transform.rotation);
                    if (placed.tag == "MineMinion")
                    {
                        placed.GetComponent<MineMinion>().player = gameObject;
                        placed.GetComponent<MineMinion>().mineToChestRoute = mineToChestRoute;
                    }
                    if (currentSlot != 3)
                    {
                        placed.transform.Rotate(new Vector3(0, -90, 0));
                    }
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
                        DestroyPreview();
                        previewTower = Instantiate(previewTowers[currentSlot], groundCheck.point + towerOffsets[currentSlot], transform.rotation);
                        if (currentSlot != 3)
                        {
                            previewTower.transform.Rotate(new Vector3(0, -90, 0));
                        }
                        previewSpawned = true;
                    }
                    else
                    {
                        previewTower.transform.position = groundCheck.point + towerOffsets[currentSlot];
                        previewTower.transform.rotation = transform.rotation;
                        if (currentSlot != 3)
                        {
                            previewTower.transform.Rotate(new Vector3(0, -90, 0));
                        }
                    }
                }
            }
            else if (groundCheck.transform.gameObject.tag == "Path" && currentSlot == 4)
            {
                towerTips.SetActive(false);
                if (previewIsRange)
                {
                    DestroyPreview();
                    previewIsRange = false;
                }

                if (Input.GetButtonDown("Fire1") && !paused && towerPrices[currentSlot] <= money && !tooPoorPopup.activeSelf && !upgradePopup.activeSelf)
                {
                    money -= towerPrices[currentSlot];
                    GameObject placed = Instantiate(towers[currentSlot], groundCheck.point + towerOffsets[currentSlot], transform.rotation);
                    placed.transform.rotation = groundCheck.transform.rotation;
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
                        DestroyPreview();
                        previewTower = Instantiate(previewTowers[currentSlot], groundCheck.point + towerOffsets[currentSlot], transform.rotation);
                        previewTower.transform.Rotate(new Vector3(0, -90, 0));
                        previewSpawned = true;
                    }
                    else
                    {
                        previewTower.transform.position = groundCheck.point + towerOffsets[currentSlot];
                        previewTower.transform.rotation = groundCheck.transform.rotation;
                    }
                }
            }
            else if (groundCheck.transform.gameObject.tag == "Totem")
            {
                towerTips.SetActive(true);
                if (!previewIsRange)
                {
                    DestroyPreview();
                }

                if (Input.GetButtonDown("Fire1"))
                {
                    previewIsRange = true;
                    DestroyPreview();
                    previewTower = Instantiate(rangePreview, groundCheck.transform.position, transform.rotation);
                    rangePreview.transform.localScale = new Vector3(groundCheck.transform.gameObject.GetComponent<TowerValues>().range[groundCheck.transform.gameObject.GetComponent<TowerValues>().level] * 2, 0.05f, groundCheck.transform.gameObject.GetComponent<TowerValues>().range[groundCheck.transform.gameObject.GetComponent<TowerValues>().level] * 2);
                }

                if (Input.GetButtonDown("Fire2") && groundCheck.transform.gameObject.GetComponent<TowerValues>().maxLevel > groundCheck.transform.gameObject.GetComponent<TowerValues>().level)
                {
                    upgradePopup.SetActive(true);
                    DestroyPreview();
                    upgradeCostDisplay.text = $"Upgrade? Cost: {groundCheck.transform.gameObject.GetComponent<TowerValues>().upgradeCost}";
                    Cursor.lockState = CursorLockMode.None;
                }
                //DestroyPreview();
            }
            else if (groundCheck.transform.gameObject.tag == "Tower" && !tooPoorPopup.activeSelf && !upgradePopup.activeSelf)
            {
                towerTips.SetActive(true);
                if (!previewIsRange)
                {
                    DestroyPreview();
                }

                if (Input.GetButtonDown("Fire1"))
                {
                    previewIsRange = true;
                    DestroyPreview();
                    previewTower = Instantiate(rangePreview, groundCheck.transform.position, transform.rotation);
                    rangePreview.transform.localScale = new Vector3(groundCheck.transform.gameObject.GetComponent<TowerValues>().range[groundCheck.transform.gameObject.GetComponent<TowerValues>().level] * 2, 0.05f, groundCheck.transform.gameObject.GetComponent<TowerValues>().range[groundCheck.transform.gameObject.GetComponent<TowerValues>().level] * 2);
                }
                else if (Input.GetButtonDown("Fire2") && groundCheck.transform.gameObject.GetComponent<TowerValues>().maxLevel > groundCheck.transform.gameObject.GetComponent<TowerValues>().level)
                {
                    upgradePopup.SetActive(true);
                    upgradeCostDisplay.text = $"Upgrade? Cost: {groundCheck.transform.gameObject.GetComponent<TowerValues>().upgradeCost[groundCheck.transform.gameObject.GetComponent<TowerValues>().level]}";
                    Cursor.lockState = CursorLockMode.None;
                    DestroyPreview();
                }
                else if (Input.GetButtonDown("Rotate"))
                {
                    groundCheck.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, transform.position - new Vector3(0, -1, 0) - groundCheck.transform.position, 10000 * Time.deltaTime, 0.0f));
                    groundCheck.transform.rotation = Quaternion.Euler(0, groundCheck.transform.rotation.eulerAngles.y, 0);
                }
            }
            else if (groundCheck.transform.gameObject.tag == "Trap")
            {
                towerTips.SetActive(true);
                if (!previewIsRange)
                {
                    DestroyPreview();
                }

                if (Input.GetButtonDown("Fire1"))
                {
                    previewIsRange = true;
                    DestroyPreview();
                    previewTower = Instantiate(rangePreview, groundCheck.transform.position, transform.rotation);
                    rangePreview.transform.localScale = new Vector3(groundCheck.transform.gameObject.GetComponent<TowerValues>().range[groundCheck.transform.gameObject.GetComponent<TowerValues>().level] * 2, 0.05f, groundCheck.transform.gameObject.GetComponent<TowerValues>().range[groundCheck.transform.gameObject.GetComponent<TowerValues>().level] * 2);
                }
                else if (Input.GetButtonDown("Fire2") && groundCheck.transform.gameObject.GetComponent<TowerValues>().maxLevel > groundCheck.transform.gameObject.GetComponent<TowerValues>().level)
                {
                    upgradePopup.SetActive(true);
                    upgradeCostDisplay.text = $"Upgrade? Cost: {groundCheck.transform.gameObject.GetComponent<TowerValues>().upgradeCost[groundCheck.transform.gameObject.GetComponent<TowerValues>().level]}";
                    Cursor.lockState = CursorLockMode.None;
                    DestroyPreview();
                }
                else if (Input.GetButtonDown("Rotate"))
                {
                    groundCheck.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, transform.position - new Vector3(0, -1, 0) - groundCheck.transform.position, 10000 * Time.deltaTime, 0.0f));
                    groundCheck.transform.rotation = Quaternion.Euler(0, groundCheck.transform.rotation.eulerAngles.y, 0);
                }

            }
            else if (groundCheck.transform.gameObject.tag != "Preview")
            {
                towerTips.SetActive(false);
                DestroyPreview();
            }
        }
        else
        {
            towerTips.SetActive(false);
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
        else if (Input.GetButtonDown("SlotFour"))
        {
            currentSlot = 4;
            DestroyPreview();
        }
        else if (Input.GetButtonDown("SlotFive"))
        {
            currentSlot = 5;
            DestroyPreview();
        }
        else if (Input.GetButtonDown("SlotSix"))
        {
            currentSlot = 6;
            DestroyPreview();
        }
        else if (Input.GetButtonDown("SlotZero"))
        {
            currentSlot = 0;
            DestroyPreview();
        }

        int lastFrameSlot = currentSlot;
        currentSlot -= Mathf.RoundToInt(Input.mouseScrollDelta.y);
        if (currentSlot > towers.Length - 1)
        {
            currentSlot = 1;
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
        moneyDisplay.text = $"{money} G";
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
        rotating = false;

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
        GameObject _upradeParticles = player.GetComponent<PlayerScript>().upradeParticles;
        GameObject targetTower = player.GetComponent<PlayerScript>().groundCheck.transform.gameObject;
        if (player.GetComponent<PlayerScript>().money >= targetTower.GetComponent<TowerValues>().upgradeCost[targetTower.GetComponent<TowerValues>().level])
        {
            player.GetComponent<PlayerScript>().money -= targetTower.GetComponent<TowerValues>().upgradeCost[targetTower.GetComponent<TowerValues>().level];
            targetTower.GetComponent<TowerValues>().sellFor += targetTower.GetComponent<TowerValues>().upgradeCost[targetTower.GetComponent<TowerValues>().level] - 25;
            targetTower.GetComponent<TowerValues>().level += 1;
            GameObject spawnedParticles = Instantiate(_upradeParticles, targetTower.transform.position, Quaternion.identity);
            spawnedParticles.transform.rotation = Quaternion.Euler(-90, 0, 0);
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            player.GetComponent<PlayerScript>().tooPoorPopup.SetActive(true);
        }
        targetTower.GetComponent<TowerValues>().UpdateMaterial();
        popup.SetActive(false);
    }

    public void Sell(GameObject popup)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject targetTower = player.GetComponent<PlayerScript>().groundCheck.transform.gameObject;
        player.GetComponent<PlayerScript>().money += targetTower.GetComponent<TowerValues>().sellFor;
        Destroy(targetTower);
        Cursor.lockState = CursorLockMode.Locked;
        popup.SetActive(false);
    }

    public void Decline(GameObject popup)
    {
        popup.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Ive got a feeling!");
        if (collision.gameObject.tag == "Gem")
        {
            Debug.Log("Ooh shiny!");
            if (collision.gameObject.GetComponent<PickUps>().dropped)
            {
                Debug.Log("Im rich now!");
                collision.gameObject.GetComponent<PickUps>().chest.GetComponent<Chest>().gemsLeft++;
                Destroy(collision.gameObject);
            }
        }
        else if (collision.gameObject.tag == "Coin")
        {
            if (collision.gameObject.GetComponent<PickUps>().dropped)
            {
                Debug.Log("Feeling like mario");
                money++;
                Destroy(collision.gameObject);
            }
        }
    }
}
