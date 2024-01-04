using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public float speed;
    private float timerToTurn;
    private float turnTime;
    private int flip;

    private bool canTeleport;

    private GameObject sam;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        speed = 1f;
        timerToTurn = 0;
        turnTime = 5;
        flip = 1;

        canTeleport = true;

        sam = GameObject.Find("Sam");
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Translate(Vector2.left * speed * Time.deltaTime);
        MoveBackAndForth();
        //MoveTowardSam();
    }

    // Teleport
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Portal Teleport Plane") && canTeleport)
        {
            var portalScript = collision.gameObject.GetComponentInParent<PortalBehaviors>();
            portalScript.Teleport(collision.gameObject.name, gameObject);
            canTeleport = false;
            //Teleport(collision.gameObject.name);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Portal Teleport Plane") && !canTeleport)
        {
            canTeleport = true;
        }

    }

    private void MoveBackAndForth()
    {
        timerToTurn += Time.deltaTime;
        if (timerToTurn >= turnTime)
        {
            Flip();
            timerToTurn = 0;
        }

        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void MoveTowardSam()
    {
        Vector2 dir = (sam.transform.position - transform.position).normalized;
        rb.AddForce(dir * speed);

        Debug.Log("Dir: " + dir);
    }

    private void Flip()
    {
        //transform.Rotate(0f, 180f, 0f);
        speed = -speed;
        flip = -flip;
        //gameObject.transform.localScale = new Vector2(flip, 1);
    }
}
