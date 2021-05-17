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
    [SerializeField] private int levelNumber = 1;
    

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
    public Image[] hearts;
    private Image injuredImageEffect;

    [Header("EndGame")]
    public GameObject winMenu;
    public TextMeshProUGUI completionTimeText;
    public GameObject highScoreObject;
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
        injuredImageEffect = timerSlider.gameObject.transform.parent.GetChild(0).gameObject.GetComponent<Image>();
        currentTimerValue = initialTimerValue;
        timerTextValue.text = ((int)currentTimerValue).ToString();
        UpdateCntrlEnergy(initialCntrlEnergy);
        timerSlider.value = currentTimerValue / 60;
        timerTextValue.text = ((int)currentTimerValue).ToString();
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
        timerSlider.value = currentTimerValue / 60;
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


    public void UpdateHealth(int health)
    {
        if (isRunning)
        {
            for (int i = health; i < 3; i++)
            {
                hearts[i].enabled = false;
            }
            StartCoroutine(InjuredEffect());
            if (health == 0)
            {
                reasonForLossText.text = "You ran out of lives.";
                LoseLevel();
            }
        }
    }

    IEnumerator InjuredEffect()
    {
        float current = 0f;
        injuredImageEffect.color = new Color(injuredImageEffect.color.r, injuredImageEffect.color.g, injuredImageEffect.color.b, .3f);
        while (current < .3f)
        {
            current += Time.deltaTime;
            injuredImageEffect.color = new Color(injuredImageEffect.color.r, injuredImageEffect.color.g, injuredImageEffect.color.b, 1f - current/.3f);
            yield return null;
        }
        injuredImageEffect.color = new Color(injuredImageEffect.color.r, injuredImageEffect.color.g, injuredImageEffect.color.b, 0f);
    }

    private void LoseLevel()
    {
        if (isRunning)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            DisableGameObjects();

            isRunning = false;
            Debug.Log("Lost level");

            loseMenu.SetActive(true);
            if (AudioManager.instance != null)
                AudioManager.instance.Play("Lose");
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
        PlayerManager.instance.isActive = false;
    }

    public void WinLevel(GameObject key)
    {
        if (isRunning)
        {
            if (key.layer == LayerMask.NameToLayer("Key"))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                DisableGameObjects();
                isRunning = false;
                Debug.Log("Won level!");
                float completionTime = Mathf.Round((Time.time - startTime) * 100f) / 100f;
                string pPrefVariable = "Level" + levelNumber.ToString();
                if (PlayerPrefs.HasKey(pPrefVariable))
                {
                    if (PlayerPrefs.GetFloat(pPrefVariable) > completionTime)
                    {
                        highScoreObject.SetActive(true);
                        PlayerPrefs.SetFloat(pPrefVariable, completionTime);
                    }
                }
                else
                {
                    PlayerPrefs.SetFloat(pPrefVariable, completionTime);
                }
                completionTimeText.text = "Completion Time: " + completionTime.ToString() + "s";
                winMenu.SetActive(true);
                if (AudioManager.instance != null)
                    AudioManager.instance.Play("Win");

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
