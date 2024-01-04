using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    private ArrowManager arrowManager;

    public ParticleSystem killEnemyParticle;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI arrowsText;
    public int points;

    // Start is called before the first frame update
    void Start()
    {
        arrowManager = GameObject.Find("Arrow Manager").GetComponent<ArrowManager>();

        scoreText.text = "Score: " + points;
        points = 0;
        arrowsText.text = "Arrows: " + arrowManager.numArrows;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Load game scene
    public void ChangeToGameScene()
    {
        SceneManager.LoadScene("Sam Portals");
    }

    // Load start scene
    public void ChangeToStartScene()
    {
        SceneManager.LoadScene("Start Screen");
    }

    // Update arrows text
    public void UpdateArrowsText()
    {
        arrowsText.text = "Arrows: " + arrowManager.numArrows;
    }

    public void AddPoints(int numPoints)
    {
        points += numPoints;
        scoreText.text = "Points: " + points;
    }

    public void KillEnemyParticlePlay(Vector2 pos)
    {
        Debug.Log("Play kill enemy");
        killEnemyParticle.gameObject.SetActive(true);
        //killEnemyParticle.Play();
    }
}
