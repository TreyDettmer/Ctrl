using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using TMPro;

public class LevelManager : MonoBehaviour
{
    // class for handling altf4 timer, cntrl meter, player health

    public static LevelManager instance;
    public Tilemap foregroundTilemap;
    


    public float initialTimerValue = 60f;
    private float currentTimerValue = 60f;


    private bool isRunning = false;

    private int currentCntrlEnergy = 0;
    public int maxCntrlEnergy = 10;


    [Header("UI")]
    public Slider timerSlider;
    public TextMeshProUGUI timerTextValue;
    public Slider cntrlEnergySlider;
    public TextMeshProUGUI cntrlEnergyTextValue;
    public Image clipboard;
    public TextMeshProUGUI[] shortcutCostsText;


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
        value = Mathf.Min(Mathf.Max(0, value),maxCntrlEnergy);
        cntrlEnergySlider.value = (float)value / (float)maxCntrlEnergy;
        cntrlEnergyTextValue.text = value.ToString();
        currentCntrlEnergy = value;
    }

    public void UpdateClipboard(Sprite sprite)
    {
        if (sprite != null)
        {
            clipboard.sprite = sprite;
            clipboard.enabled = true;
            
        }
        else
        {
            clipboard.enabled = false;
        }
        
    }

    public void SetShortcutCosts(int tabCost, int copyCost, int cutCost, int pasteCost)
    {
        shortcutCostsText[0].text = "Tab\t\t" + tabCost;
        shortcutCostsText[1].text = "Copy\t\t" + copyCost;
        shortcutCostsText[2].text = "Cut\t\t" + cutCost;
        shortcutCostsText[3].text = "Paste\t\t" + pasteCost;
    }

    private void LoseLevel()
    {

    }

    private void WinLevel()
    {

    }

}
