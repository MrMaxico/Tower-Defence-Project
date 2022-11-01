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
    public float launchSpeed;
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
    public GameObject spyglass;
    public GameObject[] canvases;

    public Transform[] mineToChestRoute;
    public Transform[] springpadJump;

    public Vector3[] towerOffsets;
    Vector3 rotation;
    Vector3 camrotation;
    Vector3 movement;

    public int[] towerPrices;
    public int currentSlot;
    public int money;
    int jumpProgress;

    bool previewSpawned;
    bool previewIsRange;
    bool canWalk;
    bool gameOver;

    public TextMeshProUGUI moneyDisplay;
    public TextMeshProUGUI upgradeCostDisplay;
    public TextMeshProUGUI sellDisplay;

    public Animator deathScreen;

    Rigidbody playerRB;

    RaycastHit groundCheck;

    private void Start()
    {
        playerRB = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        AcceptPoorness(tooPoorPopup);
        FindObjectOfType<AudioManagerScript>().Play("WaveMusic");
        canWalk = true;
    }
    private void Update()
    {
        //check the gems
        GameObject[] gems = GameObject.FindGameObjectsWithTag("Gem");

        if (chest.GetComponent<Chest>().gemsLeft <= 0 && gems.Length == 0)
        {
            Debug.Log("Game over!");
            gameOver = true;
            deathScreen.SetBool("IsGameOver", true);
        }

        if (gameOver)
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            chest.GetComponent<Chest>().gemsLeft = 0;
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
        if (!PauseMenuScript.gameIsPaused && !tooPoorPopup.activeSelf && !upgradePopup.activeSelf)
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

        //spyglass
        if (currentSlot == 1 && Input.GetButton("Fire2") && !gameOver)
        {
            spyglass.SetActive(true);
            foreach (GameObject canvas in canvases)
            {
                canvas.SetActive(false);
            }
            cam.GetComponent<Camera>().fieldOfView = 10;
        }
        else
        {
            spyglass.SetActive(false);
            foreach (GameObject canvas in canvases)
            {
                canvas.SetActive(true);
            }
            cam.GetComponent<Camera>().fieldOfView = 60;
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

                if (Input.GetButtonDown("Fire1") && !PauseMenuScript.gameIsPaused && towerPrices[currentSlot] <= money && !tooPoorPopup.activeSelf && !upgradePopup.activeSelf)
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
                else if (Input.GetButtonDown("Fire1") && !PauseMenuScript.gameIsPaused && towerPrices[currentSlot] > money)
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

                if (Input.GetButtonDown("Fire1") && !PauseMenuScript.gameIsPaused && towerPrices[currentSlot] <= money && !tooPoorPopup.activeSelf && !upgradePopup.activeSelf)
                {
                    money -= towerPrices[currentSlot];
                    GameObject placed = Instantiate(towers[currentSlot], groundCheck.point + towerOffsets[currentSlot], transform.rotation);
                    placed.transform.rotation = groundCheck.transform.rotation;
                }
                else if (Input.GetButtonDown("Fire1") && !PauseMenuScript.gameIsPaused && towerPrices[currentSlot] > money)
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
                groundCheck.transform.gameObject.GetComponent<TowerValues>().starVisTime = 1f;
                if (groundCheck.transform.gameObject.GetComponent<TowerValues>().level < groundCheck.transform.gameObject.GetComponent<TowerValues>().maxLevel) 
                {
                    sellDisplay.text = $"Sell tower ({groundCheck.transform.gameObject.GetComponent<TowerValues>().sellFor}G)";
                    upgradeCostDisplay.text = $"Upgrade tower ({groundCheck.transform.gameObject.GetComponent<TowerValues>().upgradeCost[groundCheck.transform.gameObject.GetComponent<TowerValues>().level]}G)";
                }
                else
                {
                    upgradeCostDisplay.text = "";
                }
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

                if (Input.GetButtonDown("Use") && groundCheck.transform.gameObject.GetComponent<TowerValues>().maxLevel > groundCheck.transform.gameObject.GetComponent<TowerValues>().level)
                {
                    Upgrade(groundCheck.transform.gameObject);
                    Cursor.lockState = CursorLockMode.None;
                }
                else if (Input.GetButtonDown("Sell"))
                {
                    Sell(groundCheck.transform.gameObject);
                    DestroyPreview();
                }
                //DestroyPreview();
            }
            else if (groundCheck.transform.gameObject.tag == "Tower" && !tooPoorPopup.activeSelf && !upgradePopup.activeSelf)
            {
                towerTips.SetActive(true);
                groundCheck.transform.gameObject.GetComponent<TowerValues>().starVisTime = 1f;
                if (groundCheck.transform.gameObject.GetComponent<TowerValues>().level < groundCheck.transform.gameObject.GetComponent<TowerValues>().maxLevel)
                {
                    sellDisplay.text = $"Sell tower ({groundCheck.transform.gameObject.GetComponent<TowerValues>().sellFor}G)";
                    upgradeCostDisplay.text = $"Upgrade tower ({groundCheck.transform.gameObject.GetComponent<TowerValues>().upgradeCost[groundCheck.transform.gameObject.GetComponent<TowerValues>().level]}G)";
                }
                else
                {
                    upgradeCostDisplay.text = "";
                }
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
                else if (Input.GetButtonDown("Use") && groundCheck.transform.gameObject.GetComponent<TowerValues>().maxLevel > groundCheck.transform.gameObject.GetComponent<TowerValues>().level)
                {
                    Upgrade(groundCheck.transform.gameObject);
                    DestroyPreview();
                }
                else if (Input.GetButtonDown("Sell"))
                {
                    Sell(groundCheck.transform.gameObject);
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
                groundCheck.transform.gameObject.GetComponent<TowerValues>().starVisTime = 1f;
                if (groundCheck.transform.gameObject.GetComponent<TowerValues>().level < groundCheck.transform.gameObject.GetComponent<TowerValues>().maxLevel)
                {
                    sellDisplay.text = $"Sell tower ({groundCheck.transform.gameObject.GetComponent<TowerValues>().sellFor}G)";
                    upgradeCostDisplay.text = $"Upgrade tower ({groundCheck.transform.gameObject.GetComponent<TowerValues>().upgradeCost[groundCheck.transform.gameObject.GetComponent<TowerValues>().level]}G)";
                }
                else
                {
                    upgradeCostDisplay.text = "-";
                }
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
                else if (Input.GetButtonDown("Use") && groundCheck.transform.gameObject.GetComponent<TowerValues>().maxLevel > groundCheck.transform.gameObject.GetComponent<TowerValues>().level)
                {
                    Upgrade(groundCheck.transform.gameObject);
                    DestroyPreview();
                }
                else if (Input.GetButtonDown("Sell"))
                {
                    Sell(groundCheck.transform.gameObject);
                    DestroyPreview();
                }
                else if (Input.GetButtonDown("Rotate"))
                {
                    groundCheck.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, transform.position - new Vector3(0, -1, 0) - groundCheck.transform.position, 10000 * Time.deltaTime, 0.0f));
                    groundCheck.transform.rotation = Quaternion.Euler(0, groundCheck.transform.rotation.eulerAngles.y, 0);
                }

            }
            else if (groundCheck.transform.gameObject.tag == "Enemy")
            {
                Debug.Log("Target in sight!");
                groundCheck.transform.gameObject.GetComponent<PathFollowingScript>().hpBarVisTime = 2f;
                towerTips.SetActive(false);
                DestroyPreview();
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

        //display money
        moneyDisplay.text = $"{money} G";
    }

    private void FixedUpdate()
    {
        //movement
        if (canWalk)
        {
            movement.x = Input.GetAxis("Horizontal");
            movement.z = Input.GetAxis("Vertical");
        }
        else
        {
            movement.x = 0;
            movement.z = 0;
        }
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

    public void Upgrade(GameObject _targetTower)
    {
        GameObject targetTower = _targetTower;
        if (money >= targetTower.GetComponent<TowerValues>().upgradeCost[targetTower.GetComponent<TowerValues>().level])
        {
            money -= targetTower.GetComponent<TowerValues>().upgradeCost[targetTower.GetComponent<TowerValues>().level];
            targetTower.GetComponent<TowerValues>().sellFor += targetTower.GetComponent<TowerValues>().upgradeCost[targetTower.GetComponent<TowerValues>().level] - 25;
            targetTower.GetComponent<TowerValues>().level += 1;
            GameObject spawnedParticles = Instantiate(upradeParticles, targetTower.transform.position, Quaternion.identity);
            spawnedParticles.transform.rotation = Quaternion.Euler(-90, 0, 0);
            Cursor.lockState = CursorLockMode.Locked;
        }
        targetTower.GetComponent<TowerValues>().UpdateMaterial();
    }

    public void Sell(GameObject popup)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject targetTower = player.GetComponent<PlayerScript>().groundCheck.transform.gameObject;
        player.GetComponent<PlayerScript>().money += targetTower.GetComponent<TowerValues>().sellFor;
        //GameObject coinsFX = Instantiate(sellParticles, transform.position, transform.rotation);
        Destroy(targetTower);
        Cursor.lockState = CursorLockMode.Locked;
        popup.SetActive(false);
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
        else if (collision.gameObject.tag == "Springpad")
        {
            FindObjectOfType<AudioManagerScript>().Play("JumpPadBoing");
            StartCoroutine(SpringpadLaunch());
        }
    }

    IEnumerator SpringpadLaunch()
    {
        if (gameObject.transform.position != springpadJump[jumpProgress].position)
        {
            canWalk = false;
            playerRB.velocity = new Vector3(0, 0, 0);
            playerRB.useGravity = false;
            transform.position = Vector3.MoveTowards(transform.position, springpadJump[jumpProgress].position, launchSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
            StartCoroutine(SpringpadLaunch());
        }
        else if (jumpProgress < springpadJump.Length - 1)
        {
            canWalk = false;
            playerRB.velocity = new Vector3(0, 0, 0);
            jumpProgress++;
            yield return new WaitForEndOfFrame();
            StartCoroutine(SpringpadLaunch());
        }
        else if (gameObject.transform.position == springpadJump[springpadJump.Length - 1].position)
        {
            canWalk = true;
            jumpProgress = 0;
            playerRB.useGravity = true;
        }
    }
}
