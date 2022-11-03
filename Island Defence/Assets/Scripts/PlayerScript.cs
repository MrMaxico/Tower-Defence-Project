using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PlayerScript : MonoBehaviour
{
    [Header("Movement")]
    public float sensitivity;
    public float speed;
    public float launchSpeed;
    public float maxXRotation;
    float mouseVertical;
    float defaultSensitivity;
    public GameObject cam;
    Vector3 rotation;
    Vector3 camrotation;
    Vector3 movement;
    int jumpProgress;
    bool canWalk;
    Rigidbody playerRB;
    public Transform[] springpadJump;
    [Space(20)]
    [Header("Towers")]
    public GameObject rangePreview;
    public GameObject[] previewTowers;
    public GameObject[] towers;
    GameObject[] previewTags;
    GameObject previewTower;
    public Vector3[] towerOffsets;
    public int[] towerPrices;
    bool previewSpawned;
    bool previewIsRange;
    public Material[] previewMaterials;
    public Material cantPlacePreview;
    [Space(20)]
    [Header("Game info")]
    public GameObject chest;
    public GameObject towerTips;
    public GameObject[] spawners;
    public int currentSlot;
    public int money;
    bool gameOver;
    bool jingleStarted;
    RaycastHit groundCheck;
    public Transform[] mineToChestRoute;
    [Space(20)]
    [Header("UX")]
    public GameObject upradeParticles;
    public GameObject openMine;
    public GameObject closedMine;
    public GameObject spyglass;
    public GameObject[] canvases;
    public TextMeshProUGUI moneyDisplay;
    public TextMeshProUGUI upgradeCostDisplay;
    public TextMeshProUGUI sellDisplay;
    public Animator deathScreen;
    public GameObject sellParticles;

    private void Start()
    {
        playerRB = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        FindObjectOfType<AudioManagerScript>().Play("WaveMusic");
        canWalk = true;
        defaultSensitivity = sensitivity;
    }
    private void Update()
    {
        //check the gems
        GameObject[] gems = GameObject.FindGameObjectsWithTag("Gem");

        //game-over
        if (chest.GetComponent<Chest>().gemsLeft <= 0 && gems.Length == 0)
        {
            Debug.Log("Game over!");
            gameOver = true;
            FindObjectOfType<AudioManagerScript>().StopPlaying("WaveMusic1");
            FindObjectOfType<AudioManagerScript>().StopPlaying("WaveMusic2");
            FindObjectOfType<AudioManagerScript>().StopPlaying("MamaSquidMusic");
            if (!jingleStarted)
            {
                jingleStarted = true;
                FindObjectOfType<AudioManagerScript>().Play("GameOverJingle");
            }
            deathScreen.SetBool("IsGameOver", true);
        }

        if (gameOver)
        {
            Cursor.lockState = CursorLockMode.None;
        }

        //game-over debug
        if (Input.GetKeyDown(KeyCode.N))
        {
            chest.GetComponent<Chest>().gemsLeft = 0;
        }

        //skip to next wave
        if (Input.GetKeyDown(KeyCode.K))
        {
            foreach (GameObject spawner in spawners)
            {
                if (spawner.GetComponent<Spawner>().waves.Length - 1 > spawner.GetComponent<Spawner>().currentWave)
                {
                    spawner.GetComponent<Spawner>().currentWave++;
                    spawner.GetComponent<Spawner>().waveProgress = 0;
                }
            }
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            for (int i = 0; i < enemies.Length; i++)
            {
                Destroy(enemies[i]);
            }
        }
        //camera
        if (!PauseMenuScript.gameIsPaused)
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

        //zoom
        if (Input.GetButton("Zoom") && !gameOver)
        {
            currentSlot = 1;
            sensitivity = defaultSensitivity / 2;
            DestroyPreview();
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
            sensitivity = defaultSensitivity;
            foreach (GameObject canvas in canvases)
            {
                canvas.SetActive(true);
            }
            cam.GetComponent<Camera>().fieldOfView = 60;
        }

        //main raycast
        if (Physics.Raycast(transform.position, cam.GetComponent<Transform>().forward, out groundCheck, 4))
        {
            //place default towers
            if (groundCheck.transform.gameObject.tag == "Floor" && currentSlot != 4)
            {
                if (previewSpawned)
                {
                    if (towerPrices[currentSlot] <= money)
                    {
                        ChangePreviewMaterial(previewTower, previewMaterials[currentSlot]);
                    }
                    else
                    {
                        ChangePreviewMaterial(previewTower, cantPlacePreview);
                    }
                }
                towerTips.SetActive(false);
                if (previewIsRange)
                {
                    DestroyPreview();
                    previewIsRange = false;
                }

                if (Input.GetButtonDown("Fire1") && !PauseMenuScript.gameIsPaused && towerPrices[currentSlot] <= money)
                {
                    money -= towerPrices[currentSlot];
                    GameObject placed = Instantiate(towers[currentSlot], groundCheck.point + towerOffsets[currentSlot], transform.rotation);
                    if (currentSlot != 3)
                    {
                        placed.transform.Rotate(new Vector3(0, -90, 0));
                    }
                }
                else if (Input.GetButtonDown("Fire1") && !PauseMenuScript.gameIsPaused && towerPrices[currentSlot] > money)
                {
                    Debug.Log("You poor");
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
            //place spear trap
            else if (groundCheck.transform.gameObject.tag == "Path" && currentSlot == 4)
            {
                if (previewSpawned)
                {
                    if (towerPrices[currentSlot] <= money)
                    {
                        ChangePreviewMaterial(previewTower, previewMaterials[currentSlot]);
                    }
                    else
                    {
                        ChangePreviewMaterial(previewTower, cantPlacePreview);
                    }
                }

                towerTips.SetActive(false);
                if (previewIsRange)
                {
                    DestroyPreview();
                    previewIsRange = false;
                }

                if (Input.GetButtonDown("Fire1") && !PauseMenuScript.gameIsPaused && towerPrices[currentSlot] <= money)
                {
                    money -= towerPrices[currentSlot];
                    GameObject placed = Instantiate(towers[currentSlot], groundCheck.point + towerOffsets[currentSlot], transform.rotation);
                    placed.transform.rotation = groundCheck.transform.rotation;
                }
                else if (Input.GetButtonDown("Fire1") && !PauseMenuScript.gameIsPaused && towerPrices[currentSlot] > money)
                {
                    Debug.Log("You poor");
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
            //see totem info
            else if (groundCheck.transform.gameObject.tag == "Totem")
            {
                towerTips.SetActive(true);
                groundCheck.transform.gameObject.GetComponent<TowerValues>().starVisTime = 1f;
                upgradeCostDisplay.text = "-";
                sellDisplay.text = $"Sell tower ({groundCheck.transform.gameObject.GetComponent<TowerValues>().sellFor}G)";
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
                else if (Input.GetButtonDown("Sell"))
                {
                    Sell(groundCheck.transform.gameObject);
                    DestroyPreview();
                }
            }
            //see tower info
            else if (groundCheck.transform.gameObject.tag == "Tower")
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
                    sellDisplay.text = $"Sell tower ({groundCheck.transform.gameObject.GetComponent<TowerValues>().sellFor}G)";
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
            //see trap info
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

            }
            //display enemy healthbar
            else if (groundCheck.transform.gameObject.tag == "Enemy")
            {
                Debug.Log("Target in sight!");
                groundCheck.transform.gameObject.GetComponent<PathFollowingScript>().hpBarVisTime = 2f;
                towerTips.SetActive(false);
                DestroyPreview();
            }
            //talk to the mayor
            else if (groundCheck.transform.gameObject.tag == "CampLeader")
            {
                towerTips.SetActive(true);
                upgradeCostDisplay.text = "Start waves";
                sellDisplay.text = $"-";
                if (Input.GetButtonDown("Use"))
                {
                    foreach (GameObject spawner in spawners)
                    {
                        StartCoroutine(spawner.GetComponent<Spawner>().SpawnCycle());
                    }
                }
            }
            //buy mine minions
            else if (groundCheck.transform.gameObject.tag == "Mine" && !spawners[0].GetComponent<Spawner>().firstWave)
            {
                towerTips.SetActive(true);
                upgradeCostDisplay.text = "Buy miner (25G)";
                sellDisplay.text = $"-";
                if (Input.GetButtonDown("Use") && money >= 25)
                {
                    GameObject spawnedMiner = Instantiate(towers[7], mineToChestRoute[0].position, Quaternion.identity);
                    spawnedMiner.GetComponent<MineMinion>().player = gameObject;
                    spawnedMiner.GetComponent<MineMinion>().mineToChestRoute = mineToChestRoute;
                    money -= 25;
                }
            }
            else if (groundCheck.transform.gameObject.tag == "Mine" && spawners[0].GetComponent<Spawner>().firstWave)
            {
                towerTips.SetActive(true);
                upgradeCostDisplay.text = "Come back later!";
                sellDisplay.text = $"-";
            }
            //unable to place previews
            else if (groundCheck.transform.gameObject.tag == "Path")
            {
                if (previewSpawned)
                {
                    ChangePreviewMaterial(previewTower, cantPlacePreview);
                    previewTower.transform.position = groundCheck.point + towerOffsets[currentSlot];
                    previewTower.transform.rotation = transform.rotation;
                    if (currentSlot != 3 && currentSlot != 4)
                    {
                        previewTower.transform.Rotate(new Vector3(0, -90, 0));
                    }
                }
                else
                {
                    previewTower = Instantiate(previewTowers[currentSlot], groundCheck.point + towerOffsets[currentSlot], transform.rotation);
                    if (currentSlot != 3 && currentSlot != 4)
                    {
                        previewTower.transform.Rotate(new Vector3(0, -90, 0));
                    }
                    previewSpawned = true;
                }
            }
            else if (groundCheck.transform.gameObject.tag == "Floor")
            {
                if (previewSpawned)
                {
                    ChangePreviewMaterial(previewTower, cantPlacePreview);
                    previewTower.transform.position = groundCheck.point + towerOffsets[currentSlot];
                    previewTower.transform.rotation = transform.rotation;
                    if (currentSlot != 3 && currentSlot != 4)
                    {
                        previewTower.transform.Rotate(new Vector3(0, -90, 0));
                    }
                }
                else
                {
                    previewTower = Instantiate(previewTowers[currentSlot], groundCheck.point + towerOffsets[currentSlot], transform.rotation);
                    if (currentSlot != 3 && currentSlot != 4)
                    {
                        previewTower.transform.Rotate(new Vector3(0, -90, 0));
                    }
                    previewSpawned = true;
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

        //select towerslot
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

    private void ChangePreviewMaterial(GameObject preview, Material newMaterial)
    {
        foreach (GameObject mesh in preview.GetComponent<PreviewTowers>().meshes)
        {
            if (mesh.TryGetComponent<MeshRenderer>(out MeshRenderer meshRenderer))
            {
                meshRenderer.material = newMaterial;
            }
            else if (mesh.TryGetComponent<SkinnedMeshRenderer>(out SkinnedMeshRenderer skinnedMeshRenderer))
            {
                skinnedMeshRenderer.material = newMaterial;
            }
        }
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
        GameObject coinsFX = Instantiate(sellParticles, targetTower.transform.position, targetTower.transform.rotation);
        coinsFX.transform.Rotate(new Vector3(-90, 0, 0));
        FindObjectOfType<AudioManagerScript>().Play("TowerSellSound");
        Destroy(targetTower);
        Cursor.lockState = CursorLockMode.Locked;
        popup.SetActive(false);
    }

    //pickups and launchpad
    private void OnTriggerEnter(Collider collision)
    {
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
