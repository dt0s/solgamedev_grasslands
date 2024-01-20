using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UnityEditor.Experimental;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class JumpKing : MonoBehaviour
{
    
    // OBJECT FIELDS
    private Rigidbody2D rb;
    private Camera _camera;
    private SpriteRenderer _sRend;
    public Animator animator;

    private LayerMask _whatIsGround;
    private LayerMask _whatIsEnemy;

    private float _moveInput;
    private int _whatIsMove;

    private Vector2 _leftFootPos;
    private Vector2 _rightFootPos;
    private Vector2 _stuckRightPos;
    private Vector2 _stuckLeftPos;
    private Vector2 _stuckVector;
    private Vector2 _throwLeft;
    private Vector2 _throwRight;
    
    private Vector2 _groundCheckBox;
    private Vector2 _stopVector;
    private float groundedRadius = 0.03f;
    private float stuckRadius = 0.03f;
    private bool _isGroundedLeft;
    private bool _isGroundedRight;
    private bool _isGrounded;
    private bool _isStuckLeft;
    private bool _isStuckRight;
    private bool _isThrowing;
    private float _jumpValue = 0f;
    
    public Transform footPositionRight;
    public Transform footPositionLeft;
    public Transform stuckCheckLeft;
    public Transform stuckCheckRight;
    public Transform projectileSpawnPosRight;
    public Transform projectileSpawnPosLeft;
    
    [SerializeField] private GameObject projectile;

    // Movement Settings
    public float moveSpeed;
    public float maxJump;
    public float sideFactorForJump;
    public float jumpIncrement;
    public float shootCooldown;
    public float projectileX;
    public float projectileY;
    public float throwAmplification;

    // Player Stats
    public float maxHealth;
    private float damageTaken;

    // ----------------------------------------
    // SERIALIZATION    
    //      SAVE PLAYER PROGRESS BOOLS & DATA
    // ----------------------------------------
    // SERIALIZABLE FIELDS
    [SerializeField] public float[] savePosition;
    [SerializeField] public float[] saveVelocity;
    [SerializeField] public float[] saveCamPosition;
    [SerializeField] public float saveCurrentHealth;
    [SerializeField] public float saveMaxHealth;
    [SerializeField] public float saveDamageBoost;
    [SerializeField] public string saveCurrentScene;
    [SerializeField] public int saveCurrentMoney;
    [SerializeField] public bool[] savePortalBools;
    [SerializeField] public bool[] saveItemBools;
    [SerializeField] public bool[] saveTrophyBools;
// REDO THESE FIELDS? AS IS IS CONTAINED IN BOOL LIST!
    [SerializeField] public bool hasBasePortal;
    [SerializeField] public bool hasDesertPortal;
    // LOADER FIELDS TO CONTAIN CURRENT GAMESTATE WHICH GETS SAVED CONTINUALLY
    // POSITION, SCENE, HEALTH SETUP
    private Vector3 _sPos;
    private Vector3 _cPos;
    private Vector3 _sVel;

    private float _currentMaxHealth;
    private float _currentHealth;
    private float _currentDamageBoost;
    private string _currentScene;
    // CASH
    private int _currentMoney;
    // PORTALS
    private bool[] _portalBools;
    private bool _bP_BasePortal;
    private bool _bP_DesertPortal;
    // TROPHIES
    private bool[] _trophyBools;
    private bool _bT_TrophyOne;
    // ITEMS
    private bool[] _itemBools;
    private bool _bI_hasRock;
    private bool _bI_hasItemTwo;
    
    private string _savePath;

    // GAME OBJECTS ACCORDING TO SAVED PLAYER DATA
    private bool hasRock;
    
    // PORTAL SPAWN LOCATIONS!
    private string inputForPortal;
    private bool isTakingPortal;

    // PORTAL ANIMATIONS
    private GameObject basePortalEffect;
    private GameObject desertPortalEffect;
    
    public string _currentTransitionName;


    void Start()
    {
        // GAME OBJECTS AND REFERENCES
        Application.targetFrameRate = 60;
        _camera = FindObjectOfType<Camera>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        saveCurrentScene = SceneManager.GetActiveScene().name;
        // ONLY NEEDED IN PORTAL ROOM, OTHERWISE SINGLE ONES ARE NEEDED ACCORDING TO SCENE
        basePortalEffect = GameObject.FindGameObjectWithTag("basePortalEffect");
        desertPortalEffect = GameObject.FindGameObjectWithTag("desertPortalEffect");
        Debug.Log("DESERT PORTAL EFFECT: " + desertPortalEffect);
        _sRend = GetComponent<SpriteRenderer>();
        _whatIsGround = 1 << LayerMask.NameToLayer("Ground");
        _whatIsEnemy = 1 << LayerMask.NameToLayer("Enemy");
        _whatIsMove = _whatIsEnemy | _whatIsGround;
        _stopVector = new Vector2(0, 0);
        _stuckVector = new Vector2(0.5f, 0.1f);
        _throwLeft = new Vector2(-projectileX, projectileY);
        _throwRight = new Vector2(projectileX, projectileY);

        // DEFAULT ANIAMTOR STATE
        animator.SetBool("isAirborne", true);
        animator.SetBool("startsCharging", false);
        animator.SetBool("isWalking", false);
        animator.SetBool("isIdle", false);
        
        // SERIALIZATION
        string path = Application.persistentDataPath;
        string saveFileName = "aaaSave.data";
        _savePath = Path.Combine(path, saveFileName);
        Debug.Log(GameData.TransitionName);
        _currentTransitionName = GameData.TransitionName;

        // DOES HE COME FROM A PORTAL??
        if (_currentTransitionName != null)
        {
            Debug.Log("START TAKE PORTAL! Current Portal: " + _currentTransitionName);
            LoadPortal(_currentTransitionName);
            _currentTransitionName = null;
            GameData.TransitionName = _currentTransitionName;
            SetupPortalAnimations();
        }
        // DOES HE COME FROM A LEVEL SWITCH??
       
        // LOAD FILE IF IT EXISTS AND HE DOESNT COME FROM A PORTAL
        else if (File.Exists(_savePath))
        {
            LoadData();
            SetupPortalAnimations();
        }
        // CREATE A NEW USER SAVE FILE IN CASE OF FIRST USE (OR DELETION OF SAVEFILE)
        else
        {
            Debug.Log("CREATING NEW SAVE FILE!");
            // GET DATA TO SAVE AT END OF FRAME
            Vector3 sPos = this.transform.position;
            Vector3 sVel = this.rb.velocity;
            Vector3 cPos = _camera.transform.position;
            // PREPARE INITIAL SAVE FILE WITH ALL RELEVANT FIELDS
            savePosition = new float[3];
            savePosition[0] = GameData.startPos_gameStart.x;
            savePosition[1] = GameData.startPos_gameStart.y;
            savePosition[2] = GameData.startPos_gameStart.z;

            saveVelocity = new float[3];
            saveVelocity[0] = 0f;
            saveVelocity[1] = 0f;
            saveVelocity[2] = 0f;
            
            saveCamPosition = new float[3];
            saveCamPosition[0] = savePosition[0];
            saveCamPosition[1] = savePosition[1] + 2f;
            saveCamPosition[2] = savePosition[2] - 10f;
            
            saveCurrentHealth = 100;
            saveMaxHealth = 100;
            saveDamageBoost = 0;
            _currentDamageBoost = 0;
            // WE START IN MAIN GAME SCENE
            saveCurrentScene = "MainGame";
            // ZERO CASH
            saveCurrentMoney = 0;
            
            savePortalBools = new bool[8];
            savePortalBools[0] = false;
            savePortalBools[1] = false;
            savePortalBools[2] = false;
            savePortalBools[3] = false;
            savePortalBools[4] = false;
            savePortalBools[5] = false;
            savePortalBools[6] = false;
            savePortalBools[7] = false;

            saveItemBools = new bool[8];
            saveItemBools[0] = false;
            saveItemBools[1] = false;
            saveItemBools[2] = false;
            saveItemBools[3] = false;
            saveItemBools[4] = false;
            saveItemBools[5] = false;
            saveItemBools[6] = false;
            saveItemBools[7] = false;

            saveTrophyBools = new bool[8];
            saveItemBools[0] = false;
            saveItemBools[1] = false;
            saveItemBools[2] = false;
            saveItemBools[3] = false;
            saveItemBools[4] = false;
            saveItemBools[5] = false;
            saveItemBools[6] = false;
            saveItemBools[7] = false;

            // SAVE DATA AT END OF FRAME
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(_savePath, FileMode.Create);
            PlayerData data = new PlayerData(this);
            formatter.Serialize(stream, data);
            stream.Close();
            Debug.Log("CREATED INITIAL SAVE FILE FOR NEW PLAYER!");
            Debug.Log("LOADING THIS NEWLY CREATED SAVE FILE");
            LoadData();
        }
    }
    
    private void Update()
    {        
        // POSITIONS OF FEED AND STUCK AND GROUND SETUP
        _currentScene = SceneManager.GetActiveScene().name;
        _leftFootPos = new Vector2(footPositionLeft.position.x, footPositionLeft.position.y);
        _rightFootPos = new Vector2(footPositionRight.position.x, footPositionRight.position.y);
        _stuckLeftPos = new Vector2(stuckCheckLeft.position.x, stuckCheckLeft.position.y);
        _stuckRightPos = new Vector2(stuckCheckRight.position.x, stuckCheckRight.position.y);
        _isStuckLeft = Physics2D.OverlapCircle(_stuckLeftPos, stuckRadius, _whatIsMove);
        _isStuckRight = Physics2D.OverlapCircle(_stuckRightPos, stuckRadius, _whatIsMove);
        _isGroundedLeft = Physics2D.OverlapCircle(_leftFootPos, groundedRadius, _whatIsMove);
        _isGroundedRight = Physics2D.OverlapCircle(_rightFootPos, groundedRadius, _whatIsMove);
        _isGrounded = (_isGroundedLeft || _isGroundedRight);
        if (!_isGrounded && (_isStuckLeft || _isStuckRight) && rb.velocity.y == 0)
        {
            if (_isStuckLeft)
            {
                rb.velocity = _stuckVector;
            } 
            else if (_isStuckRight)
            {
                rb.velocity = -1 * _stuckVector;
            }
        }

        // MOVEMENT LEFT & RIGHT
        _moveInput = Input.GetAxisRaw("Horizontal");
        if (Input.GetKey(KeyCode.Space) == false && _isGrounded && !_isStuckLeft && !_isStuckRight && rb.velocity.y == 0) //&& !stuckCheck
        {

            animator.SetBool("isAirborne", false);
            animator.SetBool("startsCharging", false);
            if (_moveInput != 0)
            {
                if (_moveInput < 0)
                {
                    _sRend.flipX = true;
                } else if (_moveInput > 0)
                {
                    _sRend.flipX = false;
                }
                animator.SetBool("isIdle", false);
                animator.SetBool("isWalking", true);
                if (_isGrounded)
                {
                    rb.velocity = new Vector2(_moveInput * moveSpeed, rb.velocity.y);                    
                }
            }
            else
            {
                animator.SetBool("isWalking", false);
                animator.SetBool("isIdle", true);
            }
        }
        // KEEP CHARGING JUMP
        if (Input.GetKey(KeyCode.Space) && _isGrounded && !_isStuckLeft && !_isStuckRight)
        {
            if (animator.GetBool("startsCharging") == false)
            {
                animator.SetBool("isAirborne", false);
                animator.SetBool("startsCharging", true);
                animator.SetBool("isWalking", false);
                animator.SetBool("isIdle", false);
            }
            _jumpValue += Time.deltaTime * jumpIncrement;
            if (_moveInput < 0)
            {
                _sRend.flipX = true;
            } else if (_moveInput > 0)
            {
                _sRend.flipX = false;
            }
        }
        // START CHARGING JUMP
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded && !_isStuckLeft && !_isStuckRight) // && !stuckCheck
        {
            rb.velocity = _stopVector;
        }
        // JUMP - STOP CHARGING
        if ((Input.GetKeyUp(KeyCode.Space) || _jumpValue >= maxJump) && _isGrounded && !_isStuckLeft && !_isStuckRight) // && !stuckCheck
        {
            _isGrounded = false;
            if (_jumpValue > maxJump)
                _jumpValue = maxJump;
            float tempx = _moveInput * Mathf.Sqrt(sideFactorForJump);
            float tempy = _jumpValue;
            rb.velocity = new Vector2(tempx, tempy);
            _jumpValue = 0;
            animator.SetBool("startsCharging", false);
            animator.SetBool("isWalking", false);
            animator.SetBool("isAirborne", true);
            animator.SetBool("isIdle", false);
        }
        // ATTACK
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (_isThrowing)
                return;
            
            animator.SetTrigger("t_throw");
            _isThrowing = true;
            
            // Instantiate and shoot bullet
            StartCoroutine(DelayProjectileInstance(_sRend.flipX));
        }
    }

    public void FixedUpdate()
    {
        // SAVE DATA
        SaveData();
    }
    
    private void OnApplicationQuit()
    {
        // SAVE DATA
        SaveData();
    }
    private void ResetThrow()
    {
        // APPLY THROW COOLDOWN
        _isThrowing = false;
    }
    
    // WORLD TRIGGER EVENTS (Projectiles, Portals, LevelSwitchers to transition scenes)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other)
        {
            Debug.Log(other.tag);
            switch (other.tag)
            {
                case "FireBall":
                    // Add death animation for player
                    //animator.SetTrigger("Die");
                    damageTaken = ProjectileFireBall.fireBallDamage;
                    TakeDamage(damageTaken);
                    break;
                case "switch_DesertLevel":
                    Debug.Log("SWITCH VALUE: " + saveCurrentScene);
                    if (saveCurrentScene == "MainGame")
                    {
                        SceneManager.LoadScene("DesertLevel", LoadSceneMode.Single);
                    } else if (saveCurrentScene == "DesertLevel")
                    {
                         
                        SceneManager.LoadScene("MainGame", LoadSceneMode.Single);
                    }
                    break;
            }
        }
    }
    public void TakeDamage(float damage)
    {
        saveCurrentHealth -= damage;
        if (saveCurrentHealth <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        // RESPAWN IN PORTAL ROOM!
        Vector3 respawnPos = GameData.startPos_respawn;
        Vector3 camPos = GameData.startPos_respawn + GameData.cameraAdjustment;
        SaveData(respawnPos, camPos);
        // RELOAD START SCENE AND START AGAIN AND SHIT
        //PLAY DEATH ANIMATION AND RELOAD IN PORTAL ROOM!
        // CHARACTER ANIMATION
        // SCENE ANIMTION
        // RELOAD PORTAL ROOM AFTER x SECONDS
        
    }
    IEnumerator DelayProjectileInstance(bool direction)
    {
        yield return new WaitForSeconds(.2f);
        GameObject element = Instantiate(projectile);

        if (direction)
        {
            element.transform.position = projectileSpawnPosLeft.transform.position;
            element.GetComponent<Rigidbody2D>().velocity = _throwLeft + throwAmplification * rb.velocity;
        }
        else
        {
            element.transform.position = projectileSpawnPosRight.transform.position;
            element.GetComponent<Rigidbody2D>().velocity = _throwRight + throwAmplification * rb.velocity;
        }
        Invoke("ResetThrow", shootCooldown);
    }


    public void SaveData(Vector3 respawn = default(Vector3), Vector3 cam = default(Vector3), Vector3 vel = default(Vector3))
    {
        if (respawn != default(Vector3))
        {
            _sPos = respawn;
            _cPos = cam;
            _sVel = vel;
        }
        else
        {
            _sPos = transform.position;
            _cPos = _camera.transform.position;
            _sVel = rb.velocity;
        }
        // GET DATA TO SAVE AT END OF FRAME
        // PREPARE DATA TO SAVE
        //- position
        savePosition = new float[3];
        savePosition[0] = _sPos.x;
        savePosition[1] = _sPos.y;
        savePosition[2] = _sPos.z;
        //- velocity
        saveVelocity = new float[3];
        saveVelocity[0] = _sVel.x;
        saveVelocity[1] = _sVel.y;
        saveVelocity[2] = _sVel.z;
        //- camPosition
        saveCamPosition = new float[3];
        saveCamPosition[0] = _cPos.x;
        saveCamPosition[1] = _cPos.y;
        saveCamPosition[2] = _cPos.z;
        //- currentHealth
        saveCurrentHealth = _currentHealth;
        //- maxHealth
        saveMaxHealth = _currentMaxHealth;
        //- currentScene
        saveCurrentScene = _currentScene;
        //- money
        saveCurrentMoney = _currentMoney;
        //- portalBools
        savePortalBools = _portalBools;
        savePortalBools[0] = GameData.hasBasePortal || hasBasePortal;
        savePortalBools[1] = GameData.hasDesertPortal || hasDesertPortal;
        //- itemBools
        saveItemBools = _itemBools;
        //- trophies
        saveTrophyBools = _trophyBools;
        // SAVE DATA AT END OF FRAME
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(_savePath, FileMode.Create);
        PlayerData data = new PlayerData(this);
        formatter.Serialize(stream, data);
        stream.Close();
        if (respawn != default(Vector3))
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
        }
    }
    

    public void LoadData()
    {
        Debug.Log("LOAD DATA()");
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(_savePath, FileMode.Open);
        PlayerData data = formatter.Deserialize(stream) as PlayerData;
        stream.Close();
        _sPos.x = data.position[0];
        _sPos.y = data.position[1];
        _sPos.z = data.position[2];

        _sVel.x = data.velocity[0];
        _sVel.y = data.velocity[1];
        _sVel.z = data.velocity[2];

        _cPos.x = data.camPosition[0];
        _cPos.y = data.camPosition[1];
        _cPos.z = data.camPosition[2];
        _camera.transform.position = new Vector3(_cPos.x, _cPos.y, _cPos.z);
        Debug.Log(data.currentScene);
        _currentScene = data.currentScene;        
        
        // PLAYER STATS
        _currentHealth = data.currentHealth;
        _currentMaxHealth = data.currentMaximumHealth;
        _currentDamageBoost = data.damageBoost;
        _currentMoney = data.currentMoney;
        _portalBools = data.portalBools;
        _itemBools = data.itemBools;
        _trophyBools = data.trophies;
        // SPLITTER TO ASSIGN BOOL FROM ARRAY TO THEIR RESPECTIVE ACTIONS
        // PORTALS
        hasBasePortal = _portalBools[0] || GameData.hasBasePortal;
        hasDesertPortal = _portalBools[1] || GameData.hasDesertPortal;
        GameData.hasBasePortal = hasBasePortal;
        GameData.hasDesertPortal = hasDesertPortal;
        // ITEMS
        hasRock = _itemBools[0];
        // TROPHIES
        
        if (!isTakingPortal)
        {
            Debug.Log("-- NOT travelling with portal - LOADING LAST RECORDED POSITION");
            transform.position = _sPos;
            rb.velocity = _sVel;
            _camera.transform.position = _cPos;
        }
        // ACTIVATE PORTAL EFFECTS
        if (!hasBasePortal && basePortalEffect)
        {
            basePortalEffect.SetActive(false);
        }
        if (!hasDesertPortal && desertPortalEffect)
        {
            Debug.Log("INSIDE DEACTIVATION WHY???");
            Debug.Log(hasDesertPortal);
            desertPortalEffect.SetActive(false);
        }
        isTakingPortal = false;
    }

    public void LoadPortal(string portalName)
    {
        inputForPortal = null;
        inputForPortal = portalName + saveCurrentScene;
        isTakingPortal = true;
        // PREPARE DATA

        Debug.Log("LOAD PORTAL TRAVEL DATA");
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(_savePath, FileMode.Open);
        PlayerData data = formatter.Deserialize(stream) as PlayerData;
        stream.Close();

        // PLAYER STATS
        _currentHealth = data.currentHealth;
        _currentMaxHealth = data.currentMaximumHealth;
        _currentDamageBoost = data.damageBoost;
        _currentMoney = data.currentMoney;
        _portalBools = data.portalBools;
        _itemBools = data.itemBools;
        _trophyBools = data.trophies;
        // SPLITTER TO ASSIGN BOOL FROM ARRAY TO THEIR RESPECTIVE ACTIONS
        // PORTALS
        hasBasePortal = _portalBools[0] && GameData.hasBasePortal;
        hasDesertPortal = _portalBools[1] && GameData.hasDesertPortal;
        // ITEMS
        hasRock = _itemBools[0];
        // TROPHIES
        Debug.Log("INPUT FOR PORTAL" + inputForPortal);
        switch (inputForPortal)
        {
            case "BasePortalPortalRoomMainGame":
                if (!hasBasePortal)
                {
                    Debug.Log("FIRST TIME TRAVEL - BASEPORTAL");
                    activateCurrentPortal("basePortal");
                }
                Debug.Log("TAKING: " + inputForPortal);
                setLocation(GameData.location_basePortal_mainGame);
                break;
            case "BasePortalMainGamePortalRoom":
                if (!hasBasePortal)
                {
                    Debug.Log("FIRST TIME TRAVEL - BASEPORTAL");
                    activateCurrentPortal("basePortal");
                }
                Debug.Log("TAKING: " + inputForPortal);
                setLocation(GameData.location_basePortal_portalRoom);
                break;
            case "DesertPortalPortalRoomDesertLevel":
                if (!hasDesertPortal)
                {
                    Debug.Log("FIRST TIME TRAVEL - DESERTPORTAL");
                    activateCurrentPortal("desertPortal");
                }
                Debug.Log("TAKING" + inputForPortal);
                setLocation(GameData.location_desertPortal_desertLevel);
                break;
            case "DesertPortalDesertLevelPortalRoom":
                Debug.Log("HAS DESERT PORTAL - " + hasDesertPortal);
                if (!hasDesertPortal)
                {
                    Debug.Log("FIRST TIME TRAVEL - DESERTPORTAL");
                    activateCurrentPortal("desertPortal");
                }
                Debug.Log("TAKING " + inputForPortal);
                setLocation(GameData.location_desertPortal_portalRoom);
                break;
            case "DesertWorldTriggerDesertLevel":
                Debug.Log("TAKING " + inputForPortal);
                setLocation(GameData.startPos_main_to_desert);
                break;
            case "DesertWorldTriggerMainGame":
                Debug.Log("TAKING " + inputForPortal);
                setLocation(GameData.startPos_desert_to_main);
                break;
        }
        SaveData();
        isTakingPortal = false;

        void activateCurrentPortal(string portalToActivate)
        {
            Debug.Log("ACTIVATE CURRENT PORTAL: " + portalToActivate);
            switch (portalToActivate)
            {
                case "basePortal":
                    hasBasePortal = true;
                    Debug.Log("ACTIVATING BASE PORTAL!!!!!!!!" + _portalBools);
                    Debug.Log(_portalBools[0]);
                    _portalBools[0] = true;
                    GameData.hasBasePortal = true;
                    break;
                case "desertPortal":
                    hasDesertPortal = true;
                    Debug.Log("ACTIVATING DESERT PORTAL!!!!!!! " + _portalBools);
                    _portalBools[1] = true;
                    GameData.hasDesertPortal = true;
                    break;
                case "nextPortal":
                    break;
            }
        }

        void setLocation(Vector3 input)
        {
            rb.velocity = Vector3.zero;
            transform.position = input;
            _camera.transform.position = input + GameData.cameraAdjustment;
        }
    }

    public void SetupPortalAnimations()
    {
        Debug.Log("SETUP PORTAL ANIMATIONS!");
        // PORTAL EFFECTS (ONLY IN LOAD OR LOADPORTAL EFFECTS? AS MAIN HAS NO PORTAL ACTIVES ANYWAYS?
        if (!hasBasePortal && basePortalEffect)
        {
            Debug.Log("DEACTIVATING BASE EFFECT");

            basePortalEffect.SetActive(false);
        }

        if (!hasDesertPortal && desertPortalEffect)
        {
            Debug.Log("DEACTIVATING DESERT EFFECT");
            desertPortalEffect.SetActive(false);
        }
    }
}
