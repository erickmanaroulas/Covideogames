using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour
{
    //handling
    public float speed = 10.0f;
    public float rotationSpeed = 450;
    public float currentSpeed = 0;
    public float actionRange = 2f;

    Vector3 input;
    public float yellRange = 5f;
    public int yellCost = 20;
    public float yellCooldown = 20;
    float yellTimer = Mathf.Infinity;
    public ParticleSystem yellEffect;
    public AudioClip yellSound;
    public GameObject yellReadyIndicator;
    bool yellReady = true;

    public Collider actionArea;
    public LoseScreen loseScreen;

    AudioSource audioSource;
    public AudioClip washSFX;


    public bool dirtyHands = false;

    Health health;
    private Quaternion targetRotation;

   
    [SerializeField] Transform moveSprite;

    private Joystick joystick;

    public bool moving, haveMask;
    public GameObject notificationPrefab, notificationUi;
    private void Awake()
    {
        joystick = GameObject.FindGameObjectWithTag("Joystick").GetComponent<Joystick>();
        audioSource = GetComponent<AudioSource>();

        health = GetComponent<Health>();
        health.infectionMultiplier -= .75f;

    }
    void Update()
    {
        if (GameManager.gameManager.gameIsOver) return;
        if(!haveMask){
            if(notificationUi == null) notificationUi = Instantiate(notificationPrefab);
        }
        else{
            if(notificationUi != null) Destroy(notificationUi);
        }
        if (health.infectionLevel >= 100)
        {
            TriggerPlayerDeath();
        }
        else
        {
            ProcessMovement();
            ProcessActions();
            //ProcessMovementMobile();
            //ProcessActionsMobile();
        }
        yellTimer += Time.deltaTime;
        if (yellTimer > yellCooldown)
        {
            if (!yellReady)
            {
                yellReady = true;
                GameObject indicator = Instantiate(yellReadyIndicator, this.transform.position, transform.rotation, this.transform);
                Destroy(indicator, 2f);
            }
        }
        if (Application.platform == RuntimePlatform.Android)
        {
            MobileManager();
        }
    }

    float minimumHeldDuration = 1.25f;
    float keyPressedTime = 0;
    bool holdingKey = false;
    void ProcessActionsMobile()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                keyPressedTime = 0;
                holdingKey = false;
            }
            else if(touch.phase == TouchPhase.Stationary)
            {
                keyPressedTime += Time.deltaTime;
                if (keyPressedTime > minimumHeldDuration)
                {
                    holdingKey = true;
                    if(!moving)EnforceSocialDistance();

                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (!holdingKey)
                {
                    StartCoroutine(ActivateActionArea());
                }
            }
        }
    }
    void ProcessMovementMobile()
    {
       if(joystick.Horizontal > 0 || joystick.Horizontal < 0 || joystick.Vertical > 0 || joystick.Vertical < 0)
       {
           transform.Translate(Vector3.forward * Time.deltaTime * 4);
           moving = true;
       }
       else if(joystick.Horizontal == 0 && joystick.Vertical == 0){
           moving = false;
       }
    }


    private void ProcessActions()
    {
       if (Input.GetKeyDown(KeyCode.Space))
        {
            keyPressedTime = 0;
            holdingKey = false;

        }
        else if (Input.GetKey(KeyCode.Space))
        {
            keyPressedTime += Time.deltaTime;
            if (keyPressedTime > minimumHeldDuration)
            {
                holdingKey = true;
                EnforceSocialDistance();

            }
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            if (!holdingKey)
            {
                StartCoroutine(ActivateActionArea());
            }
        }

    }

    private IEnumerator ActivateActionArea()
    {
        actionArea.enabled = false;
        yield return new WaitForSeconds(.2f);
        actionArea.enabled = true;
    }

    private void EnforceSocialDistance()
    {
        //if (score.totalScore < yellCost) return;
        if (yellTimer > yellCooldown)
        {
            yellTimer = 0;
            yellReady = false;
            yellEffect.Play();
            audioSource.PlayOneShot(yellSound);
            foreach (AiController npc in GameStats.gameStats.npcs)
            {
                if (Vector3.Distance(this.transform.position, npc.transform.position) < yellRange)
                {
                    npc.ChangeStatus(Status.SocialDistance);
                }
            }
        }
        //score.ChangeScore(-yellCost);
    }

    private void ProcessMovement()
    {
        GetMovementInput();
        if (Mathf.Abs(input.x) < 1 && Mathf.Abs(input.z) < 1) return;
        Rotate();
        Move();
    }


    void GetMovementInput()
    {
        input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

    }
    
    void MobileManager()
    {
        ProcessMovementMobile();
        ProcessActionsMobile();
        MoveSpriteManager();
    }
    void MoveSpriteManager()
    {
        moveSprite.position = new Vector3(joystick.Horizontal + transform.position.x, -0.88f, joystick.Vertical + transform.position.z);
        transform.LookAt(new Vector3(moveSprite.position.x,-0.88f, moveSprite.position.z));
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    void Rotate()
    {

        targetRotation = Quaternion.LookRotation(input);
        transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void Move()
    {
        transform.Translate(0, 0, speed * Time.deltaTime);
    }

    public void WashHands()
    {
        dirtyHands = false;
        health.infectionLevel -= 10;
        health.infectionLevel = Mathf.Max(health.infectionLevel, 0);
        audioSource.PlayOneShot(washSFX);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<AiController>() != null)
        {
            if (collision.gameObject.GetComponent<AiController>().wearingMask == false) return;
        }
    }
    private void OnDestroy()
    {
        foreach (AiController npc in GameStats.gameStats.npcs)
        {
            if (npc == null) return;
            npc.gameObject.SetActive(false);
        }
        TriggerPlayerDeath();
    }
    private void TriggerPlayerDeath()
    {
        GameManager.gameManager.EndGame();
    }

}

