using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Sink : MonoBehaviour
{
    PlayerController player;
    float actionCooldown = 1f;
    float cooldownTimer = Mathf.Infinity;
    public ParticleSystem washEffect;

    AudioSource audioSource;
    public AudioClip failedAction;

    // Start is called before the first frame update
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        GameStats.gameStats.sinks.Add(this);

    }
    private void Update()
    {
        cooldownTimer += Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.transform.parent.CompareTag("Player")) 
        {
            if (cooldownTimer > actionCooldown)
            {
                cooldownTimer = 0;

                if (player.dirtyHands)
                {
                    player.WashHands();
                    player.haveMask = true;
                    washEffect.Play();
                }
                else PlayFailedActionSound();
            }
        }
    }

    private void PlayFailedActionSound()
    {
        audioSource.PlayOneShot(failedAction);
    }


}
