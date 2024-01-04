using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowManager : MonoBehaviour
{
    private GameManager gameManager;

    private GameObject sam;
    public GameObject arrowPrefab;

    public int numArrows;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        sam = GameObject.Find("Sam");

        numArrows = 100;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadArrow()
    {
        if (numArrows > 0)
        {
            Vector2 samPos = sam.transform.position;
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = (mousePos - samPos).normalized;
            Vector2 pos = samPos + (dir * 1.4f);
            Instantiate(arrowPrefab, pos, transform.rotation);
            numArrows -= 1;
        } else
        {
            Debug.Log(numArrows + " arrows left.");
        }

        gameManager.UpdateArrowsText();
        
    }

    // Adds arrow to inventory
    public void AddArrow(int quantity)
    {
        numArrows += 1;//quantity;
        gameManager.UpdateArrowsText();
    }
}
