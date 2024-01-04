using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalBehaviors : MonoBehaviour
{
    public bool wasClicked;
    public bool followSam;

    public PortalManager portalManager;

    public LayerMask whatIsGround;

    private GameObject sam;
    private GameObject exitPortal;

    public Color lineColor;
    private LineRenderer lineRend;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        followSam = false;

        portalManager = GameObject.Find("Portal Manager").GetComponent<PortalManager>();

        sam = GameObject.Find("Sam");

        lineRend = GetComponent<LineRenderer>();
        lineRend.positionCount = 2;
        lineRend.material.color = lineColor;
        lineRend.startColor = lineColor;
        lineRend.startWidth = 0.02f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            RaycastHit2D hit = Physics2D.Raycast(sam.transform.position, mousePos - transform.position);
            if (GetComponent<Collider2D>().bounds.Contains(hit.point))
            {
                //TerminatePortal();
            }
        }
        
        lineRend.SetPosition(0, transform.position);
        lineRend.SetPosition(1, sam.transform.position);
        
    }

    // Properly rotates portal based on surface it initially attached to
    public void RotateOrientPortal(RaycastHit2D hitPortalDestination)
    {
        var dir = hitPortalDestination.normal;
        transform.right = dir;
        }

    // Deactivates portal
    public void TerminatePortal()
    {
        gameObject.SetActive(false);
        followSam = false;
        wasClicked = false;
    }

    // Activates portal
    public void ActivatePortal()
    {
        gameObject.SetActive(true);
    }

    // TELEPORT!!!!
    public void Teleport(string nameOfEnterPortalPlane, GameObject objToTeleport)
    {
        var objRb = objToTeleport.GetComponent<Rigidbody2D>();
        if (portalManager.maxPortalsAreOut)
        {
            if (nameOfEnterPortalPlane == "Teleport Plane 1")
            {
                exitPortal = portalManager.portal2.transform.GetChild(0).gameObject;
            }
            else
            {
                exitPortal = portalManager.portal1.transform.GetChild(0).gameObject;
            }

            Vector2 hitDirection = objRb.velocity.normalized;
            float speed = objRb.velocity.magnitude;
            objRb.velocity = Vector3.zero;

            float hitAngle = Vector3.Angle(gameObject.transform.up, hitDirection);

            Vector3 pos = exitPortal.transform.position + (exitPortal.transform.right * 0.7f);
            objToTeleport.transform.position = pos;
            objToTeleport.transform.right = exitPortal.transform.up;
            Vector3 dir = Quaternion.AngleAxis(hitAngle, objToTeleport.transform.forward) * objToTeleport.transform.right;
            
            // Change direction of object to teleport if needed (otherwise it would go back into exit portal)
            if (Physics2D.Raycast(objToTeleport.transform.position, dir, 1f, whatIsGround))
            {
                hitAngle *= -1f;
                dir = Quaternion.AngleAxis(hitAngle, objToTeleport.transform.forward) * objToTeleport.transform.right;
            }

            objToTeleport.transform.right = dir;
            
            Vector2 newVelocity = objToTeleport.transform.right.normalized * speed;
            objRb.velocity = newVelocity;

            if (objToTeleport.CompareTag("Sam"))
            {
                PlayerController samPC = sam.GetComponent<PlayerController>();
                samPC.ReOrientSamAfterTeleport();
                samPC.isGrounded = false;
                samPC.isInAirAfterTeleport = true;
            }
        }
        
    }

    private void OnDrawGizmos()
    {
        if (gameObject.activeInHierarchy)
        {
            //Gizmos.color = lineColor;
            //Gizmos.DrawLine(transform.position, sam.transform.position);
        }
    }
}
