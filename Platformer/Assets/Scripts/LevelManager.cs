using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    // class for handling altf4 timer, cntrl meter, player health

    public static LevelManager instance;


    public float initialTimerValue = 60f;
    private float currentTimerValue = 60f;
    public Slider timerSlider;
    public TextMeshProUGUI timerTextValue;

    private bool isRunning = false;

    private int currentCntrlEnergy = 0;
    public int maxCntrlEnergy = 10;
    public Slider cntrlEnergySlider;
    public TextMeshProUGUI cntrlEnergyTextValue;


    private void Awake()
    {
        // singleton
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentTimerValue = initialTimerValue;
        isRunning = true;
    }

    private void UpdateTimerValue()
    {
        currentTimerValue -= Time.deltaTime;
        timerSlider.value = currentTimerValue / initialTimerValue;
        timerTextValue.text = ((int)currentTimerValue).ToString();

        if (currentTimerValue <= 0f)
        {
            isRunning = false;
            LoseLevel();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isRunning)
        {
            UpdateTimerValue();
        }
        
    }

    public void UpdateCntrlEnergy(int value)
    {
        value = Mathf.Max(0, value);
        cntrlEnergySlider.value = (float)value / (float)maxCntrlEnergy;
        cntrlEnergyTextValue.text = value.ToString();

    }

    private void LoseLevel()
    {

    }

    private void WinLevel()
    {

    }

}
