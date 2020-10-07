using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HamsterController : MonoBehaviour
{
    private Animator anim;
    public readonly float cycleDistance = .2f * Mathf.PI; //8 inches in hamster wheel diameter * pi
    public readonly Color BLACK = new Color(0, 0, 0);
    public readonly Color RED = new Color(255, 0, 0);

    public TMP_Text timeText;
    public TMP_Text distanceText;
    public TMP_Text totalDistanceText;
    public TMP_Text levelText;
    public TMP_Text cyclesText;
    public TMP_Text speedText;
    public TMP_Text availablePointsText;
    public TMP_Text maxSpeedUpgradesText;
    public TMP_Text accelerationUpgradesText;
    public TMP_Text enduranceUpgradesText;
    public Slider enduranceSlider;
    public GameObject upgradeButton;
    public GameObject upgradeScreen;
    public GameObject upgradePointButtons;
    public GameObject slowingDown;
    public GameObject titleScreen;
    public GameObject instructionPopup;

    private float startTime;
    private float distanceTraveled;
    private float totalDistanceTraveled;
    private double maxSpeed = 3;
    private float speedMultiplier = 1f;
    private float acceleration = 0.01f;
    private int endurance = 75;
    private int cyclesSinceLevelUp;
    private int cyclesBeforeLevelUp;
    private int level = 1;
    private int upgradesToUse;
    private int maxSpeedUpgrades;
    private int accelerationUpgrades;
    private int enduranceUpgrades;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        upgradeScreen.SetActive(false);
        upgradeButton.SetActive(false);
        slowingDown.SetActive(false);
        _ = StartCoroutine(StartScreen());
    }

    private IEnumerator StartScreen()
    {
        while (true)
        {
            if (Input.mousePresent && Input.GetMouseButtonDown(0))
            {
                titleScreen.SetActive(false);
                _ = StartCoroutine(Instructions());
                break;
            }
            yield return null;
        }
    }

    private IEnumerator Instructions()
    {
        yield return null;
        while (true)
        {
            if (Input.anyKeyDown)
            {
                instructionPopup.SetActive(false);
                _ = StartCoroutine(AtRest());
                break;
            }
            yield return null;
        }
    }

    private IEnumerator AtRest()
    {
        timeText.text = "0:00:00.00";
        distanceTraveled = 0;
        distanceText.text = "0 m";
        enduranceSlider.maxValue = endurance;
        enduranceSlider.value = 0;
        while (true)
        {
            if(Input.anyKeyDown && Input.GetAxis("Horizontal") > 0)
            {
                anim.SetBool("isRunning", true);
                _ = StartCoroutine(StartRunning());
                break;
            }
            yield return null;
        }
    }

    private IEnumerator StartRunning()
    {
        _ = StartCoroutine(UpdateTimeText());
        while (enduranceSlider.value < endurance)
        {
            if (Input.anyKeyDown && Input.GetAxis("Horizontal") > 0)
            {
                double speed = System.Math.Round(anim.speed * speedMultiplier, 2);
                if (speed < maxSpeed)
                {
                    speedText.color = BLACK;
                    speedMultiplier += acceleration;
                    anim.SetFloat("SpeedMultiplier", speedMultiplier);
                }
                else
                {
                    speed = maxSpeed;
                    speedText.color = RED;
                }
                speedText.text = speed + "";
            }
            enduranceSlider.value += Time.deltaTime * speedMultiplier;
            
            yield return null;
        }
        _ = StartCoroutine(Tired());
    }

    private IEnumerator UpdateTimeText()
    {
        startTime = Time.time;
        while (anim.GetBool("isRunning"))
        {
            timeText.text = TimeToString(System.Math.Round(Time.time - startTime, 2));
            yield return null;
        }
    }

    private string TimeToString(double time)
    {
        int minutes = (int)(time / 60);
        int hours = minutes / 60;
        time -= minutes * 60;
        minutes -= hours * 60;
        if(time < 10)
        {
            return hours + ":" + minutes.ToString("D2") + ":0" + string.Format("{0:F2}", time);
        }
        return hours + ":" + minutes.ToString("D2") + ":" + string.Format("{0:F2}", time);
    }

    private IEnumerator Tired()
    {
        slowingDown.SetActive(true);
        for (float i = speedMultiplier - .2f; i >= 0f; i -= .15f + acceleration)
        {
            speedMultiplier = i;
            anim.SetFloat("SpeedMultiplier", speedMultiplier);
            speedText.text = System.Math.Round(anim.speed * speedMultiplier, 2) + "";
            speedText.color = BLACK;
            yield return new WaitForSeconds(.5f - acceleration);
        }
        speedText.text = "0";
        slowingDown.SetActive(false);
        anim.SetBool("isRunning", false);
        yield return new WaitForSeconds(2f);
        _ = StartCoroutine(AtRest());
    }

    public void UpgradeAcceleration()
    {
        if (upgradesToUse > 0)
        {
            acceleration += .003f;
            accelerationUpgrades++;
            accelerationUpgradesText.text = accelerationUpgrades + "";
            upgradesToUse--;
            availablePointsText.text = "Available Points: " + upgradesToUse;
            CheckIfPointsAvailable();
        }
    }

    public void UpgradeMaxSpeed()
    {
        if (upgradesToUse > 0)
        {
            speedText.color = BLACK;
            maxSpeed += 1;
            maxSpeedUpgrades++;
            maxSpeedUpgradesText.text = maxSpeedUpgrades + "";
            upgradesToUse--;
            availablePointsText.text = "Available Points: " + upgradesToUse;
            CheckIfPointsAvailable();
        }
    }

    public void UpgradeEndurance()
    {
        if (upgradesToUse > 0)
        {
            endurance += 50;
            enduranceUpgrades++;
            enduranceUpgradesText.text = enduranceUpgrades + "";
            upgradesToUse--;
            availablePointsText.text = "Available Points: " + upgradesToUse;
            enduranceSlider.maxValue = endurance;
            CheckIfPointsAvailable();
        }
    }

    public void CompletedCycle()
    {
        distanceTraveled += cycleDistance;
        totalDistanceTraveled += cycleDistance;
        distanceText.text = Mathf.Round(distanceTraveled) + " m";
        totalDistanceText.text = Mathf.Round(totalDistanceTraveled) + " m";
        cyclesSinceLevelUp++;
        cyclesBeforeLevelUp = (int)(10 + 1.5 * (level-1)) - cyclesSinceLevelUp;
        cyclesText.text = cyclesBeforeLevelUp + "";
        CheckIfLevelUp();
    }

    private void CheckIfLevelUp()
    {
        if (cyclesBeforeLevelUp == 0)
        {
            LevelUp();
            levelText.text = level + "";
            cyclesSinceLevelUp = 0;
        }
    }

    private void LevelUp()
    {
        level++;
        upgradesToUse++;
        upgradeButton.SetActive(true);
        CheckIfPointsAvailable();
    }

    public void OnUpgradeClick()
    {
        upgradeScreen.SetActive(true);
        upgradePointButtons.SetActive(true);
        availablePointsText.text = "Available Points: " + upgradesToUse;
    }

    public void CloseUpgradeWindow()
    {
        upgradeScreen.SetActive(false);
    }

    public void CheckIfPointsAvailable()
    {
        if(upgradesToUse == 0)
        {
            upgradePointButtons.SetActive(false);
            upgradeButton.SetActive(false);
        }
        else
        {
            upgradePointButtons.SetActive(true);
            availablePointsText.text = "Available Points: " + upgradesToUse;
        }
    }
}

