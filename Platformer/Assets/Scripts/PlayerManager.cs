using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private PlayerController playerController;
    private PlayerControls controls;

    private float horizontalInput = 0f;

    [Header("Movement")]
    [SerializeField] private float runSpeed = 40f;
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashDistance = 10f;


    [Header("AbilityStats")]
    [SerializeField] private int dashCost = 1;
    [SerializeField] private int cutCost = 1;
    [SerializeField] private int copyCost = 1;
    [SerializeField] private int pasteCost = 1;


    [SerializeField] private int initialCntrlEnergy = 10;
    private int currentCntrlEnergy;

    private bool isJumping = false;
    private bool isDashing = false;

    private void Awake()
    {
        controls = new PlayerControls();
    }

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        // set up jump input event
        controls.Gameplay.Jump.performed += _ => Jump();
        controls.Gameplay.Dash.performed += _ => Dash();

        // set initial values
        currentCntrlEnergy = Mathf.Min(LevelManager.instance.maxCntrlEnergy,initialCntrlEnergy);
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
        // get input
        horizontalInput = controls.Gameplay.Move.ReadValue<float>() * runSpeed;
    }

    private void FixedUpdate()
    {
        // move our player
        playerController.Move(horizontalInput * Time.fixedDeltaTime, false, isJumping,isDashing,dashSpeed);
        isJumping = false;
        isDashing = false;
    }

    private void Dash()
    {
        if (currentCntrlEnergy - dashCost >= 0)
        {
            isDashing = true;
            UpdateCntrlEnergy(-dashCost);
        }
        
    }

    private void Copy()
    {

    }

    private void Paste()
    {

    }

    private void Cut()
    {

    }

    private void Jump()
    {
        isJumping = true;
    }

    // Update the current cntrl energy and notify the level manager
    private void UpdateCntrlEnergy(int change)
    {
        currentCntrlEnergy += change;
        LevelManager.instance.UpdateCntrlEnergy(currentCntrlEnergy);
    }
}
