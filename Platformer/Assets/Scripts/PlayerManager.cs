using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AI;

public class PlayerManager : MonoBehaviour
{
    private PlayerController playerController;
    private PlayerControls controls;
    private LineRenderer selectionRay;

    private float horizontalInput = 0f;

    [Header("Movement")]
    [SerializeField] private float runSpeed = 40f;
    [SerializeField] private float dashSpeed = 10f;
    Animator animator;



    [Header("Interaction")]
    [SerializeField] private float interactibilityDistance = 5f;
    [SerializeField] private LayerMask interactableLayer;


    private int currentCntrlEnergy;


    [SerializeField] private TileBase clipboardTile = null;
    [SerializeField] private Interactable clipboardObject = null;

    private bool isJumping = false;
    private bool isDashing = false;
    int health = 3;

    private Vector2 aimDirection = Vector2.zero;
    private NavMeshSurface2d navMeshSurface;

    public static PlayerManager instance;
    public bool isActive = true;

    private void Awake()
    {
        // singleton
        if (instance == null)
        {
            instance = this;
            controls = new PlayerControls();
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
        playerController = GetComponent<PlayerController>();
        selectionRay = GetComponent<LineRenderer>();
        navMeshSurface = FindObjectOfType<NavMeshSurface2d>();
        animator = GetComponent<Animator>();
        // set up input events
        controls.Gameplay.Jump.performed += _ => Jump();
        controls.Gameplay.Dash.performed += _ => Tab();
        controls.Gameplay.Copy.performed += _ => Copy();
        controls.Gameplay.Cut.performed += _ => Cut();
        controls.Gameplay.Paste.performed += _ => Paste();
        controls.Gameplay.Restart.performed += _ => LevelManager.instance.RestartLevel();

        // set initial values
        currentCntrlEnergy = LevelManager.instance.initialCntrlEnergy;
        LevelManager.instance.UpdateCntrlEnergy(currentCntrlEnergy);
        
    }

    // OnEnable and OnDisable methods are required for new input system
    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            // get input
            horizontalInput = controls.Gameplay.Move.ReadValue<float>() * runSpeed;
            animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
            Aim(controls.Gameplay.MousePosition.ReadValue<Vector2>());
        }
    }

    private void FixedUpdate()
    {
        if (isActive)
        {
            // move our player
            playerController.Move(horizontalInput * Time.fixedDeltaTime, false, isJumping, aimDirection, isDashing, dashSpeed);
            isJumping = false;
            isDashing = false;
        }
    }

    private void Tab()
    {
        if (isActive)
        {
            if (currentCntrlEnergy - LevelManager.instance.GetTabCost() >= 0)
            {
                isDashing = true;
                if (AudioManager.instance != null)
                    AudioManager.instance.Play("Tab");
                UpdateCntrlEnergy(-LevelManager.instance.GetTabCost());
            }
        }
        
    }

    private void Copy()
    {
        if (isActive)
        {
            if (currentCntrlEnergy - LevelManager.instance.GetCopyCost() >= 0)
            {
                (GameObject item, RaycastHit2D hit) = Interact();
                if (item != null)
                {
                    if (item.GetComponent<Tilemap>())
                    {
                        Vector3 hitPosition = Vector3.zero;
                        hitPosition.x = hit.point.x - .01f * hit.normal.x;
                        hitPosition.y = hit.point.y - .01f * hit.normal.y;
                        clipboardTile = LevelManager.instance.foregroundTilemap.GetTile(LevelManager.instance.foregroundTilemap.WorldToCell(hitPosition));
                        Sprite tileSprite = LevelManager.instance.foregroundTilemap.GetSprite(LevelManager.instance.foregroundTilemap.WorldToCell(hitPosition));
                        LevelManager.instance.UpdateClipboard(tileSprite);
                        if (clipboardObject)
                        {
                            Destroy(clipboardObject.gameObject);
                            clipboardObject = null;
                        }
                        UpdateCntrlEnergy(-LevelManager.instance.GetCopyCost());
                        if (AudioManager.instance != null)
                            AudioManager.instance.Play("Copy");

                    }
                    else if (item.GetComponent<Interactable>())
                    {
                        if (clipboardObject)
                        {
                            Destroy(clipboardObject.gameObject);
                        }
                        clipboardObject = Instantiate(item, new Vector3(-10, -10, 0), Quaternion.identity).GetComponent<Interactable>();
                        clipboardObject.gameObject.SetActive(false);
                        if (clipboardObject.sprite != null)
                        {
                            LevelManager.instance.UpdateClipboard(clipboardObject.sprite);
                        }
                        clipboardTile = null;
                        
                        UpdateCntrlEnergy(-LevelManager.instance.GetCopyCost());
                        if (AudioManager.instance != null)
                            AudioManager.instance.Play("Copy");
                    }
                    

                }
            }
        }
    }

    private void Paste()
    {
        if (isActive)
        {
            if (currentCntrlEnergy - LevelManager.instance.GetPasteCost() >= 0)
            {
                Vector3 offset = (aimDirection * interactibilityDistance);
                Vector3 target = transform.position + offset;
                if (clipboardTile != null)
                {
                    (GameObject item, RaycastHit2D hit) = Interact();
                    if (!item)
                    {

                        LevelManager.instance.foregroundTilemap.SetTile(LevelManager.instance.foregroundTilemap.WorldToCell(target), clipboardTile);
                        UpdateCntrlEnergy(-LevelManager.instance.GetPasteCost());
                        navMeshSurface.BuildNavMeshAsync();
                        if (AudioManager.instance != null)
                            AudioManager.instance.Play("Paste");

                    }
                }
                else if (clipboardObject != null)
                {

                    GameObject clone = Instantiate(clipboardObject.gameObject, target, Quaternion.identity);
                    clone.SetActive(true);
                    UpdateCntrlEnergy(-LevelManager.instance.GetPasteCost());
                    navMeshSurface.BuildNavMeshAsync();
                    if (AudioManager.instance != null)
                        AudioManager.instance.Play("Paste");

                }

            }
        }
    }

    private void Cut()
    {
        if (isActive)
        {
            if (currentCntrlEnergy - LevelManager.instance.GetCutCost() >= 0)
            {
                (GameObject item, RaycastHit2D hit) = Interact();
                if (item != null)
                {
                    if (item.GetComponent<Tilemap>())
                    {
                        Vector3 hitPosition = Vector3.zero;
                        hitPosition.x = hit.point.x - .01f * hit.normal.x;
                        hitPosition.y = hit.point.y - .01f * hit.normal.y;
                        clipboardTile = LevelManager.instance.foregroundTilemap.GetTile(LevelManager.instance.foregroundTilemap.WorldToCell(hitPosition));
                        Sprite tileSprite = LevelManager.instance.foregroundTilemap.GetSprite(LevelManager.instance.foregroundTilemap.WorldToCell(hitPosition));
                        LevelManager.instance.foregroundTilemap.SetTile(LevelManager.instance.foregroundTilemap.WorldToCell(hitPosition), null);
                        LevelManager.instance.UpdateClipboard(tileSprite);
                        if (clipboardObject)
                        {
                            Destroy(clipboardObject.gameObject);
                            clipboardObject = null;
                        }
                        navMeshSurface.BuildNavMeshAsync();
                        if (AudioManager.instance != null)
                            AudioManager.instance.Play("Cut");


                    }
                    else if (item.GetComponent<Interactable>())
                    {
                        if (clipboardObject)
                        {
                            Destroy(clipboardObject.gameObject);
                        }
                        clipboardObject = Instantiate(item, new Vector3(-10, -10, 0), Quaternion.identity).GetComponent<Interactable>();
                        clipboardObject.gameObject.SetActive(false);
                        Destroy(item);
                        clipboardTile = null;
                        if (clipboardObject.sprite != null)
                        {
                            LevelManager.instance.UpdateClipboard(clipboardObject.sprite);
                        }
                        navMeshSurface.BuildNavMeshAsync();
                        if (AudioManager.instance != null)
                            AudioManager.instance.Play("Cut");
                    }

                    UpdateCntrlEnergy(-LevelManager.instance.GetCutCost());

                }
            }
        }
    }

    private void Jump()
    {
        if (isActive)
        {
            isJumping = true;
        }
    }

    // Update the current cntrl energy and notify the level manager
    public void UpdateCntrlEnergy(int change)
    {

        currentCntrlEnergy += change;
        currentCntrlEnergy = Mathf.Min(currentCntrlEnergy, LevelManager.instance.maxCntrlEnergy);
        LevelManager.instance.UpdateCntrlEnergy(currentCntrlEnergy);
        
    }


    private void Aim(Vector3 aimValue)
    {
        // store the direction that we are aiming
        Vector2 target = Camera.main.ScreenToWorldPoint(aimValue);
        aimDirection = (target - new Vector2(transform.position.x,transform.position.y)).normalized;

        // draw selection ray
        Vector3 rayTarget = aimDirection * interactibilityDistance;
        selectionRay.SetPosition(0, transform.position);
        selectionRay.SetPosition(1, transform.position + rayTarget);


    }

    // Called when we want to interact with an object
    private (GameObject, RaycastHit2D) Interact()
    {

        RaycastHit2D hit = Physics2D.Raycast(transform.position, aimDirection, interactibilityDistance, interactableLayer);
        if (hit.collider != null)
        {
            return (hit.collider.gameObject, hit);
        }

        return (null,hit);
    }

    public void TakeDamage(int damage = 1)
    {
        if (isActive)
        {
            animator.SetTrigger("Injured");
            health -= damage;
            LevelManager.instance.UpdateHealth(Mathf.Max(0, health));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActive)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Key"))
            {
                LevelManager.instance.WinLevel(collision.gameObject);
            }
        }
    }

}
