using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;

public enum Status
{
    Normal,
    SocialDistance,
    GoHome
}
public class AiController : MonoBehaviour
{
    NavMeshAgent agent;
    public float runFactor = 1.25f;

    public GameObject target;
    public GameObject mask;
    public GameObject mouth;
    PlayerController player;
    Health health;

    AudioSource audioSource;
    public AudioClip maskON;
    public AudioClip maskOFF;
    public AudioClip failedAction;

    //tOM = takeoffmask
    public ParticleSystem pOMEffect;
    public ParticleSystem tOMEffect;
    public bool tOMBehaviourAllowed = true;
    public bool tOMBehaviourActivated = false;
    public float tOMMaskChance = 0;
    public float tOMBehaviourActivatedChance = 25f;
    public float tOMCooldown = 10f;
    float tOMTimer = 0f;

    public Transform hospitalWaypoint;

    public bool wearingMask = false;
    public bool goHospital = false;

    public Status status = Status.Normal;
    float timeInThisStatus = 0f;
    public float socialDistance = 2f;
    GameObject socialDistanceIndicator;

    public float movementAccuracy = 3f;
    GameObject[] goalLocations;
    float speedMult;

    public float minTimeToStay = 1f;
    public float maxTimeToStay = 5f;
    float timeStayed = 0f;
    float timeToStay;
    bool checkedForTime = false;

    public float goHomeChance = 5f;
    public Transform exit;


    float actionCooldown = 1f;
    float cooldownTimer = Mathf.Infinity;

    // Start is called before the first frame update
    private void Awake()
    {
        health = GetComponent<Health>();
        agent = this.GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if(GameObject.FindGameObjectWithTag("Hospital") != null) hospitalWaypoint = GameObject.FindGameObjectWithTag("Hospital").transform;
        audioSource = this.GetComponent<AudioSource>();
        socialDistanceIndicator = gameObject.transform.Find("Social Distance Indicator").gameObject;
        goalLocations = GameObject.FindGameObjectsWithTag("Waypoint");
        if (GameObject.FindGameObjectWithTag("Finish")) exit = GameObject.FindGameObjectWithTag("Finish").transform;


    }
    void Start()
    {
        GameStats.gameStats.npcs.Add(this);
        ResetAgent();
        DetermineMaskBehaviour();
        ChooseRandomWaypoint();
    }

    private void DetermineMaskBehaviour()
    {
        if (!tOMBehaviourAllowed) return;
        if (UnityEngine.Random.Range(0, 100) < tOMBehaviourActivatedChance)
        {
            tOMBehaviourActivated = true;
        }
    }

    void Seek(Vector3 location)
    {
        agent.SetDestination(location);
    }

    void Flee()
    {
        Vector3 fleeVector;
        fleeVector = player.transform.position - this.transform.position;
        agent.SetDestination(this.transform.position - fleeVector);
    }

    void Update()
    {
        if (GameManager.gameManager.gameIsOver)
        {
            agent.ResetPath();
            return;
        }

        NavigationBehaviour();
        TakeOffMaskBehaviour();
        ProcessStates();
        cooldownTimer += Time.deltaTime;
    }

    private void ProcessStates()
    {
        timeInThisStatus += Time.deltaTime;
        if (status == Status.SocialDistance && timeInThisStatus > 5f)
        {
            ChangeStatus(Status.Normal);
        }
        if(status == Status.Normal && timeInThisStatus > 5f && exit)
        {
            if (UnityEngine.Random.Range(0, 100) < goHomeChance)
            {
                ChangeStatus(Status.GoHome);
            }
            timeInThisStatus = 0;

        }
    }

    private void NavigationBehaviour()
    {
        if (goHospital)
        {
            if (agent.destination != hospitalWaypoint.transform.position)
            {
                agent.SetDestination(hospitalWaypoint.transform.position);
            }
        }
        else if (status == Status.SocialDistance)
        {
            Flee();
        }
        //else Wander();
        /*else if (status == Status.GoHome)
        {
            GoHome();
        }*/
        else WaypointWander();

    }

    private void GoHome()
    {
        if(agent.destination != exit.position) agent.SetDestination(exit.position);

        if (Vector3.Distance(exit.position,transform.position) <3f)
        {
            GameStats.gameStats.npcs.Remove(this);
            GameStats.gameStats.sickPeopleList.Remove(health);
            GameStats.gameStats.peopleHealed++;
            if (wearingMask) GameStats.gameStats.peopleMasked--;

            Destroy(this.gameObject);
        }
        
    }

    private void WaypointWander()
    {
        CycleWaypoints();
    }

    public void ChooseRandomWaypoint()
    {
        agent.SetDestination(goalLocations[UnityEngine.Random.Range(0, goalLocations.Length)].transform.position);

    }
    public void CycleWaypoints()
    {
        
        if (agent.remainingDistance < movementAccuracy)
        {
            if (!checkedForTime)
            {
                timeToStay = UnityEngine.Random.Range(minTimeToStay, maxTimeToStay);
                checkedForTime = true;
            }
            timeStayed += Time.deltaTime;
            ResetAgent();

            if (timeStayed > timeToStay)
            {
                timeStayed = 0;
                checkedForTime = false;
                ChooseRandomWaypoint();
            }
           
        }
    }
    private void ResetAgent()
    {
        speedMult = UnityEngine.Random.Range(0.5f, 1.5f);
        agent.speed = 1.5f * speedMult;
        agent.angularSpeed = 300;

        agent.ResetPath();
    }

    private void TakeOffMaskBehaviour()
    {
        if (tOMBehaviourActivated)
        {
            if (!wearingMask) return;
            tOMTimer += Time.deltaTime;
            if (tOMTimer > tOMCooldown)
            {
                if (tOMMaskChance < UnityEngine.Random.Range(0, 100))
                {
                    TakeOffMask();
                }
                tOMTimer = 0;
            }
        }
    }
    public void WearMask()
    {
        mask.SetActive(true);
        mouth.SetActive(false);
        GameStats.gameStats.peopleMasked += 1;
        wearingMask = true;
        health.infectionMultiplier -= .75f;
        health.spreadMultiplier -= .75f;
        audioSource.PlayOneShot(maskON);
        ParticleSystem particle = Instantiate(pOMEffect, this.gameObject.transform);
        Destroy(particle, 2f);
    }
    private void TakeOffMask()
    {
        mask.SetActive(false);
        mouth.SetActive(true);

        GameStats.gameStats.peopleMasked -= 1;
        wearingMask = false;
        health.infectionMultiplier += .75f;
        health.spreadMultiplier += .75f;
        audioSource.PlayOneShot(maskOFF);
        ParticleSystem particle = Instantiate(tOMEffect, this.gameObject.transform);
        Destroy(particle, 2f);
    }

    public void GoToHospital()
    {
        ResetAgent();
        agent.speed = 2f;
        goHospital = true;
        tOMBehaviourActivated = false;
        agent.speed = agent.speed * runFactor;
        agent.SetDestination(hospitalWaypoint.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        HospitalTrigger(other);
        PlayerTrigger(other);
    }

    private void PlayerTrigger(Collider other)
    {
        if (other.transform.parent.CompareTag("Player"))
        {
            if (wearingMask && health.isSick && hospitalWaypoint!= null) GoToHospital();
            if (cooldownTimer > actionCooldown)
            {
                cooldownTimer = 0;
                if (player.haveMask)
                {
                    if (!wearingMask)
                    {
                        WearMask();
                        player.dirtyHands = true;
                        player.haveMask = false;
                    }
                    else PlayFailedActionSound();
                }
                else PlayFailedActionSound();
            }
        }
    }

    private void PlayFailedActionSound()
    {
        audioSource.PlayOneShot(failedAction);
    }

    private void HospitalTrigger(Collider other)
    {
        if (hospitalWaypoint == null) return;
        if (other.gameObject.transform == hospitalWaypoint.transform)
        {
            GameStats.gameStats.npcs.Remove(this);
            GameStats.gameStats.sickPeopleList.Remove(health);
            GameStats.gameStats.peopleHealed++;
            if (wearingMask) GameStats.gameStats.peopleMasked--;

            Destroy(this.gameObject);
        }
    }

    public void ChangeStatus(Status newStatus)
    {
        ResetAgent();
        status = newStatus;
        if (newStatus == Status.SocialDistance)
        {
            socialDistanceIndicator.SetActive(true);
            timeInThisStatus = 0;
            ResetAgent();
            agent.speed = 2f;

        }
        if (newStatus == Status.Normal)
        {
            ResetAgent();
            ChooseRandomWaypoint();
            socialDistanceIndicator.SetActive(false);
            timeInThisStatus = 0;
        }
        if (newStatus == Status.GoHome)
        {
            ResetAgent();
            socialDistanceIndicator.SetActive(false);
        }
    }
}
