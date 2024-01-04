using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public GameObject sam;
    public GameObject portalBasic1;
    public GameObject portalBasic2;

    public GameObject portal1;
    public GameObject portal2;

    public ParticleSystem portalBlueOn;
    public ParticleSystem portalYellowOn;

    private float maxPortalFireDistance;

    public bool maxPortalsAreOut;
    public RaycastHit2D hitPortalDestination;
    public LayerMask whatIsGround;

    private Vector3 mousePos;

    // Start is called before the first frame update
    void Start()
    {
        portal1 = portalBasic1;
        portal2 = portalBasic2;

        maxPortalFireDistance = 10f;

        maxPortalsAreOut = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FirePortal(portal1);
        }
        if (Input.GetMouseButtonDown(1))
        {
            FirePortal(portal2);
        }

        // Checks if Sam has maximum portals out, so they cannot fire any more
        maxPortalsAreOut = portal1.activeSelf && portal2.activeSelf;
        /*if (maxPortalsAreOut)
        {
            portalYellowOn.transform.position = portal1.transform.position;
            portalBlueOn.transform.position = portal2.transform.position;
            if (!portalYellowOn.isPlaying)
            {
                portalYellowOn.Play();
            }
            if (!portalBlueOn.isPlaying)
            {
                portalBlueOn.Play();
            }
        } else
        {
            if (portalBlueOn.isPlaying)
            {
                portalBlueOn.Stop();
            }
            if (portalYellowOn.isPlaying)
            {
                portalYellowOn.Stop();
            }
        }*/
        TerminateSamPortalsIfNeeded();
    }

    public void FirePortal(GameObject portal)
    {
        SetTargetForPortal();
        Vector3 targetPoint = hitPortalDestination.point;
        //var portal = portal1;

        if (targetPoint != new Vector3(0.0f, 0.0f, 0.0f))
        {
            portal.transform.position = targetPoint;
            portal.GetComponent<PortalBehaviors>().RotateOrientPortal(hitPortalDestination);
            if (portal.transform.rotation.y == 1)
            {
                portal.transform.Rotate(0f, 180f, 180f);
            }
            portal.GetComponent<PortalBehaviors>().ActivatePortal();
        }

        // Fires portal - old method, before left and right click
        /*public void FirePortal()
        {
            SetTargetForPortal();
            Vector3 targetPoint = hitPortalDestination.point;
            var portal = portal1;

            if (targetPoint != new Vector3(0.0f, 0.0f, 0.0f))
            {
                // Portal follows Sam if mouse is over Sam
                var shouldFollowSam = sam.GetComponent<Collider2D>().bounds.Contains(mousePos) && 
                    !portal1.GetComponent<PortalBehaviors>().followSam && !portal2.GetComponent<PortalBehaviors>().followSam;

                if (portal1.activeSelf)
                {
                    portal = portal2;
                }
                else
                {
                    portal = portal1;
                }

                if (shouldFollowSam)
                {
                    portal.GetComponent<PortalBehaviors>().ActivatePortal();
                    portal.GetComponent<PortalBehaviors>().followSam = true;
                }
                else
                {
                    portal.transform.position = targetPoint;
                    portal.GetComponent<PortalBehaviors>().RotateOrientPortal(hitPortalDestination);
                    portal.GetComponent<PortalBehaviors>().ActivatePortal();
                }
            }*/
    }

    // Uses raycast to get position for portal's destination
    private void SetTargetForPortal()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector3 dir = (mousePos - sam.transform.position).normalized;
        hitPortalDestination = Physics2D.Raycast(sam.transform.position, dir, maxPortalFireDistance, whatIsGround);
    }

    // Terminates portals that are clicked
    public void TerminateSamPortalsIfNeeded()
    {
        if (portal1.GetComponent<PortalBehaviors>().wasClicked)
        {
            portal1.GetComponent<PortalBehaviors>().TerminatePortal();
        }
        if (portal2.GetComponent<PortalBehaviors>().wasClicked)
        {
            portal2.GetComponent<PortalBehaviors>().TerminatePortal();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            portal1.GetComponent<PortalBehaviors>().TerminatePortal();
            portal2.GetComponent<PortalBehaviors>().TerminatePortal();
        }
    }
}
