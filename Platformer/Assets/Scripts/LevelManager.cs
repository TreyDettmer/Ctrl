using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManager : MonoBehaviour
{
    // class for handling altf4 timer, cntrl meter, player health

    public static LevelManager instance;
    public Tilemap foregroundTilemap;
    [SerializeField] Animator cinematicAnimator;


    public float initialTimerValue = 60f;
    private float currentTimerValue = 60f;


    private bool isRunning = false;
    float startTime = 0f;
    public int initialCntrlEnergy = 0;
    public int maxCntrlEnergy = 10;

    [Header("AbilityStats")]
    [SerializeField] private int tabCost = 1;
    [SerializeField] private int cutCost = 1;
    [SerializeField] private int copyCost = 1;
    [SerializeField] private int pasteCost = 1;


    [Header("UI")]
    public Slider timerSlider;
    public TextMeshProUGUI timerTextValue;
    public Image timerFillImage;
    public Slider cntrlEnergySlider;
    public TextMeshProUGUI cntrlEnergyTextValue;
    public Image clipboard;
    public TextMeshProUGUI[] shortcutCostsText;
    public Image[] hearts;

    [Header("EndGame")]
    public GameObject winMenu;
    public TextMeshProUGUI completionTimeText;
    public GameObject loseMenu;
    public TextMeshProUGUI reasonForLossText;
    

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
        PlayerManager.instance.enabled = false;
        currentTimerValue = initialTimerValue;
        timerTextValue.text = ((int)currentTimerValue).ToString();
        UpdateCntrlEnergy(initialCntrlEnergy);
        SetShortcutCosts(tabCost, copyCost, cutCost, pasteCost);
        StartCinematic();
    }

    void StartCinematic()
    {
        cinematicAnimator.Play("MainCamera");
        Invoke("StartLevel", 3f);
    }

    void StartLevel()
    {
        startTime = Time.time;
        PlayerManager.instance.enabled = true;
        isRunning = true;
    }


    private void UpdateTimerValue()
    {
        currentTimerValue -= Time.deltaTime;
        timerSlider.value = currentTimerValue / initialTimerValue;
        timerTextValue.text = ((int)currentTimerValue).ToString();
        if (currentTimerValue <= 10)
        {
            timerFillImage.color = Color.red;
        }

        if (currentTimerValue <= 0f)
        {
            reasonForLossText.text = "The timer ran out.";
            LoseLevel();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isRunning)
        {
            UpdateTimerValue();
            if (PlayerManager.instance.transform.position.y <= -100)
            {
                reasonForLossText.text = "You fell into the abyss.";
                LoseLevel();
            }
        }
        
    }

    public void UpdateCntrlEnergy(int value)
    {
        value = Mathf.Min(Mathf.Max(0, value),maxCntrlEnergy);
        cntrlEnergySlider.value = (float)value / (float)maxCntrlEnergy;
        cntrlEnergyTextValue.text = value.ToString();
        
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

    public void UpdateHealth(int health)
    {
        if (isRunning)
        {
            for (int i = health; i < 3; i++)
            {
                hearts[i].enabled = false;
            }
            
            if (health == 0)
            {
                reasonForLossText.text = "You ran out of lives.";
                LoseLevel();
            }
        }
    }

    private void LoseLevel()
    {
        if (isRunning)
        {

            DisableGameObjects();

            isRunning = false;
            Debug.Log("Lost level");

            loseMenu.SetActive(true);
        }
    }

    private void DisableGameObjects()
    {
        // disable enemies
        SquiggleEnemy[] enemies = FindObjectsOfType<SquiggleEnemy>();
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].gameObject.GetComponent<NavMeshAgent>().enabled = false;
            enemies[i].enabled = false;
        }

        BouncyEnemy[] bouncyEnemies = FindObjectsOfType<BouncyEnemy>();
        for (int i = 0; i < bouncyEnemies.Length; i++)
        {
            bouncyEnemies[i].enabled = false;
            bouncyEnemies[i].GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        }
        // disable player
        PlayerManager.instance.GetComponent<PlayerController>().enabled = false;
        PlayerManager.instance.GetComponent<LineRenderer>().enabled = false;
        PlayerManager.instance.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        PlayerManager.instance.enabled = false;
    }

    public void WinLevel(GameObject key)
    {
        if (isRunning)
        {
            if (key.layer == LayerMask.NameToLayer("Key"))
            {
                DisableGameObjects();
                isRunning = false;
                Debug.Log("Won level!");

                completionTimeText.text = "Completion Time: " + (Mathf.Round((Time.time - startTime)*100f)/100f).ToString() + "s";
                winMenu.SetActive(true);

            }
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadNextLevel()
    {
        int c = SceneManager.GetActiveScene().buildIndex;
        if (c < SceneManager.sceneCountInBuildSettings - 1)
        {
            SceneManager.LoadScene(c + 1);
        }
    }

    public int GetCutCost() { return cutCost;}
    public int GetCopyCost() { return copyCost; }
    public int GetPasteCost() { return pasteCost; }
    public int GetTabCost() { return tabCost; }



}
