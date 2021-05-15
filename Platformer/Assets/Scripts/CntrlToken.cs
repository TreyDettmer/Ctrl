using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// when player collides with this object the player gains cntrl energy
public class CntrlToken : MonoBehaviour
{
    [SerializeField] private int cntrlEnergy = 2;
    private bool wasPickedUp = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!wasPickedUp)
        {
            if (collision.gameObject.GetComponent<PlayerManager>())
            {
                collision.gameObject.GetComponent<PlayerManager>().UpdateCntrlEnergy(cntrlEnergy);
                wasPickedUp = true;
                AudioManager.instance.Play("Token");
                Destroy(this.gameObject);
            }
        }
    }
}
