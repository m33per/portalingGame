using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBehavior : MonoBehaviour
{
    private GameObject sam;

    public ParticleSystem killEnemyParticle;

    private ArrowManager arrowManager;
    private GameManager gameManager;

    private Rigidbody2D arrowRb;

    public float speed;

    public bool isFlying;
    private bool canTeleport;

    // Start is called before the first frame update
    void Start()
    {
        sam = GameObject.Find("Sam");

        arrowManager = GameObject.Find("Arrow Manager").GetComponent<ArrowManager>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        arrowRb = gameObject.GetComponent<Rigidbody2D>();

        speed = 30;

        canTeleport = true;

        gameObject.SetActive(true);
        ShootArrow();
    }

    // Update is called once per frame
    void Update()
    {
        // Orient arrow correctly in air
        if (isFlying)
        {
            Vector3 forward = arrowRb.velocity;
            gameObject.transform.right = forward;
        }
    }

    public void ShootArrow()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector3 dir = (mousePos - transform.position).normalized;
        Vector2 direction = new Vector2(dir.x, dir.y);

        Quaternion rot = Quaternion.LookRotation(mousePos);
        rot.y = 0;
        gameObject.transform.rotation = rot;

        arrowRb.AddForce(direction * speed);
        isFlying = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Arrow destroys enemy and itself if it hits enemy. Also adds points to score
        if (collision.gameObject.CompareTag("Enemy") && isFlying)
        {
            //killEnemyParticle.Play();
            //gameManager.KillEnemyParticlePlay(transform.position);
            Instantiate(killEnemyParticle, transform.position, killEnemyParticle.transform.rotation);
            gameManager.AddPoints(1);
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }

        isFlying = false;

        

        arrowRb.velocity = Vector2.zero;

        // Arrow teleports through portal
        if (collision.gameObject.CompareTag("Portal Teleport Plane") && canTeleport)
        {
            var portalScript = collision.gameObject.GetComponentInParent<PortalBehaviors>();
            portalScript.Teleport(collision.gameObject.name, gameObject);
            canTeleport = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Teleport
        if (collision.gameObject.CompareTag("Portal Teleport Plane") && canTeleport)
        {
            var portalScript = collision.gameObject.GetComponentInParent<PortalBehaviors>();
            portalScript.Teleport(collision.gameObject.name, gameObject);
            canTeleport = false;
        }
    }

    
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Allows arrow to teleport after it finishes teleporting
        if (collision.gameObject.CompareTag("Portal Teleport Plane") && !canTeleport)
        {
            canTeleport = true;
        }

        // Sam picks up arrow
        if (collision.gameObject.CompareTag("Sam Collision Detector") && !isFlying)
        {
            arrowManager.AddArrow(1);
            Destroy(gameObject);
        }

    }
    
}
