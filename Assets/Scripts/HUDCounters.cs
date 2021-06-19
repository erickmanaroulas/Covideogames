using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUDCounters : MonoBehaviour
{
    public Text deadCounter;
    public Text healedCounter;
    public Text maskedCounter;
    public Text popCounter;
    public Text scoreCounter;
    public Text sickCounter;
    public Text healthCounter;


    Health playerHealth;
    public CanvasGroup sickIconOverlay;

    // Use this for initialization
    void Start()
    {
        playerHealth = GameObject.FindWithTag("Player").GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (deadCounter) deadCounter.text = GameStats.gameStats.peopleDead.ToString();
        //if (healedCounter) healedCounter.text = GameStats.gameStats.peopleHealed.ToString();
        //if (maskedCounter) maskedCounter.text = GameStats.gameStats.peopleMasked.ToString();
        //if (popCounter) popCounter.text = GameStats.gameStats.peopleTotal.ToString();
        if (scoreCounter) scoreCounter.text = GameStats.gameStats.score.ToString();
        //if (sickCounter) sickCounter.text = GameStats.gameStats.peopleSick.ToString();
        //if (healthCounter) healthCounter.text = (((int)(100 - playerHealth.infectionLevel)).ToString() + "%");
        //if (sickIconOverlay) sickIconOverlay.alpha = playerHealth.infectionLevel / 100;
      

    }
}
