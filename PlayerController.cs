using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float groundCheckRadius;
    public float slopeCheckDistance;
    public float maxSlopeAngle;
    public Transform groundCheck;
    public LayerMask whatIsGround;
    public LayerMask whatIsLadder;
    public PhysicsMaterial2D noFriction;
    public PhysicsMaterial2D fullFriction;

    private float horizontalInput;
    private float verticalInput;
    private float slopeDownAngle;
    private float slopeSideAngle;
    private float lastSlopeAngle;

    private int facingDirection;

    public bool bowEquipped;
    private bool canTeleport;
    public bool isInAirAfterTeleport;
    public bool touchingLadder;
    public bool climbingLadder;
    public bool isGrounded;
    private bool isOnSlope;
    private bool canWalkOnSlope;

    private Vector2 newVelocity;
    private Vector2 ccSize;
    private Vector2 slopeNormalPerp;

    private Rigidbody2D samRb;
    private CapsuleCollider2D cc;
    private Animator samAnim;

    private ArrowManager arrowManager;
    private GameManager gameManager;

    public GameObject enemy1Prefab;
    public GameObject gemPrefab;

    // Start is called before the first frame update
    void Start()
    {
        speed = 5f;
        groundCheckRadius = 0.5f;
        slopeCheckDistance = 1f;
        maxSlopeAngle = 45.0f;

        facingDirection = 1;

        bowEquipped = true;
        canTeleport = true;
        touchingLadder = false;
        climbingLadder = false;
        isInAirAfterTeleport = false;

        samRb = gameObject.GetComponent<Rigidbody2D>();
        cc = gameObject.GetComponent<CapsuleCollider2D>();
        ccSize = cc.size;
        samAnim = gameObject.GetComponent<Animator>();

        arrowManager = GameObject.Find("Arrow Manager").GetComponent<ArrowManager>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        InstantiateEnemies();
        InstantiateGems();
    }

    // Update is called once per frame
    void Update()
    {
        //MoveSam();
        //OrientSam();
        CheckInput();

        if (Input.GetKeyDown("1"))
        {
            transform.position = new Vector3(-29f, 0, 0);
            //ReOrientSamAfterTeleport();
        }
        if (Input.GetKeyDown("2"))
        {
            //var pos = new Vector3(1, -1.95f, 0);
            //Instantiate(enemy1Prefab, pos, enemy1Prefab.transform.rotation);
        }
    }

    private void FixedUpdate()
    {
        CheckGround();
        CheckLadder();
        SlopeCheck();
        MoveSam();
    }

    private void CheckInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (horizontalInput > 0 && facingDirection == -1)
        {
            Flip();
        }
        else if (horizontalInput < 0 && facingDirection == 1)
        {
            Flip();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            arrowManager.LoadArrow();
        }
    }
    
    private void Flip()
    {
        facingDirection *= -1;
        transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    private void CheckLadder()
    {
        touchingLadder = Physics2D.OverlapCircle(transform.position, groundCheckRadius, whatIsLadder);
    }

    private void CheckGround()
    {
        float extraHeightText = 0.05f;
        RaycastHit2D hit = Physics2D.BoxCast(cc.bounds.center, cc.bounds.size, 0f, Vector2.down, extraHeightText, whatIsGround);
        isGrounded = hit.collider != null;
        if (isGrounded)
        {
            isInAirAfterTeleport = false;
        }
    }

    private void SlopeCheck()
    {
        Vector2 checkPos = gameObject.transform.position - (Vector3)(new Vector2(0.0f, ccSize.y / 2));

        SlopeCheckHorizontal(checkPos);
        SlopeCheckVertical(checkPos);
    }

    private void SlopeCheckHorizontal(Vector2 checkPos)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, whatIsGround);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, whatIsGround);
        
        if (slopeHitFront)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
        }
        else if (slopeHitBack)
        {
            isOnSlope = true;
            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        }
        else
        {
            slopeSideAngle = 0.0f;
            isOnSlope = false;
        }
        if (slopeSideAngle == 90f)
        {
            isOnSlope = false;
            slopeSideAngle = 0.0f;
        }
    }

    private void SlopeCheckVertical(Vector2 checkPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, whatIsGround);

        if (hit)
        {
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if(slopeDownAngle != lastSlopeAngle)
            {
                isOnSlope = true;
            }

            lastSlopeAngle = slopeDownAngle;
        }

        if(slopeDownAngle > maxSlopeAngle || slopeSideAngle > maxSlopeAngle)
        {
            canWalkOnSlope = true;//false;
        }
        else
        {
            canWalkOnSlope = true;
        }

        if (isOnSlope && canWalkOnSlope && horizontalInput == 0.0f)
        {
            samRb.sharedMaterial = fullFriction;
        }
        else
        {
            samRb.sharedMaterial = noFriction;
        }
    }

    // Move Sam
    private void MoveSam()
    {
        if (horizontalInput != 0.0f)
        {
            samAnim.SetBool("isWalking", true);
        }
        else
        {
            samAnim.SetBool("isWalking", false);
        }
        if (isGrounded && !isOnSlope) //if not on slope
        {
            newVelocity.Set(speed * horizontalInput, samRb.velocity.y);//0.0f);
            samRb.velocity = newVelocity;
        }
        else if(isGrounded && isOnSlope && canWalkOnSlope) //if on slope
        {
            newVelocity.Set(speed * slopeNormalPerp.x * -horizontalInput, speed * slopeNormalPerp.y * -horizontalInput);
            samRb.velocity = newVelocity;
        }
        else if (!isGrounded && !climbingLadder)// && !isInAirAfterTeleport) //if in air and not climbing a ladder
        {
            if (horizontalInput != 0f)
            {
                newVelocity.Set(speed * horizontalInput, samRb.velocity.y);
                samRb.velocity = newVelocity;
            }
            samAnim.SetBool("isWalking", false);
        }

        // Movement for ladders
        if (touchingLadder)
        {
            //Climb the ladder
            climbingLadder = verticalInput != 0f;
            if (climbingLadder)
            {
                newVelocity.Set(0, speed * verticalInput);
                if (horizontalInput != 0f)
                {
                    newVelocity.Set(samRb.velocity.x, speed * verticalInput);//0.0f);
                }
                samRb.velocity = newVelocity;
                samRb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
            else
            {
                samRb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                newVelocity.Set(samRb.velocity.x, 0f);
                samRb.velocity = newVelocity;
            }
        }
        else
        {
            climbingLadder = false;
            samRb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    public void ReOrientSamAfterTeleport()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        facingDirection = 1;
    }

    // Equips bow, so if player clicks, arrow is shot
    public void BowEquip()
    {
        bowEquipped = true;
    }

    // Unequips bow, so if player clicks, arrow does not shoot
    public void BowUnequip()
    {
        bowEquipped = false;
    }

    // Teleport
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

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Collect gem
        if (collision.gameObject.CompareTag("Gem"))
        {
            Destroy(collision.gameObject);
            gameManager.AddPoints(1);
        }
    }

    // Make a bunch of enemies
    private void InstantiateEnemies()
    {
        /*
        Instantiate(enemy1Prefab, new Vector3(25f, 8.5f, 0.0f), enemy1Prefab.transform.rotation);
        Instantiate(enemy1Prefab, new Vector3(35f, 14f, 0.0f), enemy1Prefab.transform.rotation);
        Instantiate(enemy1Prefab, new Vector3(49f, 17f, 0.0f), enemy1Prefab.transform.rotation);
        Instantiate(enemy1Prefab, new Vector3(42f, 22.5f, 0.0f), enemy1Prefab.transform.rotation);
        Instantiate(enemy1Prefab, new Vector3(29f, 28f, 0.0f), enemy1Prefab.transform.rotation);
        Instantiate(enemy1Prefab, new Vector3(23f, 18f, 0.0f), enemy1Prefab.transform.rotation);
        */
        Instantiate(enemy1Prefab, new Vector3(22f, 6.6f, 0.0f), enemy1Prefab.transform.rotation);
        Instantiate(enemy1Prefab, new Vector3(40f, 8.8f, 0.0f), enemy1Prefab.transform.rotation);
        Instantiate(enemy1Prefab, new Vector3(29f, 7f, 0.0f), enemy1Prefab.transform.rotation);
        Instantiate(enemy1Prefab, new Vector3(51f, 0f, 0.0f), enemy1Prefab.transform.rotation);
        Instantiate(enemy1Prefab, new Vector3(60f, 0f, 0.0f), enemy1Prefab.transform.rotation);
        Instantiate(enemy1Prefab, new Vector3(105f, 9f, 0.0f), enemy1Prefab.transform.rotation);
        Instantiate(enemy1Prefab, new Vector3(142f, 3f, 0.0f), enemy1Prefab.transform.rotation);
        Instantiate(enemy1Prefab, new Vector3(134f, 26f, 0.0f), enemy1Prefab.transform.rotation);
        Instantiate(enemy1Prefab, new Vector3(128f, 27f, 0.0f), enemy1Prefab.transform.rotation);
        Instantiate(enemy1Prefab, new Vector3(114f, 43f, 0.0f), enemy1Prefab.transform.rotation);
        Instantiate(enemy1Prefab, new Vector3(88f, 30f, 0.0f), enemy1Prefab.transform.rotation);
        Instantiate(enemy1Prefab, new Vector3(100f, 30f, 0.0f), enemy1Prefab.transform.rotation);
        Instantiate(enemy1Prefab, new Vector3(59f, 28f, 0.0f), enemy1Prefab.transform.rotation);
        Instantiate(enemy1Prefab, new Vector3(52f, 28f, 0.0f), enemy1Prefab.transform.rotation);
        Instantiate(enemy1Prefab, new Vector3(45f, 28f, 0.0f), enemy1Prefab.transform.rotation);
        Instantiate(enemy1Prefab, new Vector3(44f, 32f, 0.0f), enemy1Prefab.transform.rotation);
        Instantiate(enemy1Prefab, new Vector3(68f, 10f, 0.0f), enemy1Prefab.transform.rotation);
        Instantiate(enemy1Prefab, new Vector3(88f, -2f, 0.0f), enemy1Prefab.transform.rotation);
        Instantiate(enemy1Prefab, new Vector3(126f, 12f, 0.0f), enemy1Prefab.transform.rotation);
        Instantiate(enemy1Prefab, new Vector3(103f, -2f, 0.0f), enemy1Prefab.transform.rotation);
        Instantiate(enemy1Prefab, new Vector3(95f, -2f, 0.0f), enemy1Prefab.transform.rotation);
        Instantiate(enemy1Prefab, new Vector3(110f, 30f, 0.0f), enemy1Prefab.transform.rotation);
        Instantiate(enemy1Prefab, new Vector3(-3f, 0f, 0.0f), enemy1Prefab.transform.rotation);
    }

    // Make a bunch of gems
    private void InstantiateGems()
    {
        Instantiate(gemPrefab, new Vector3(74.5f, 7.5f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(74.5f, 5.5f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(74.5f, 3.5f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(74.5f, 1.5f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(64.5f, -0.45f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(65.5f, -0.45f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(66.5f, -0.45f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(67.5f, -0.45f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(68.5f, -0.45f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(69.5f, -0.45f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(70.5f, -0.45f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(71.5f, -0.45f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(81.5f, 20.5f, 0.0f), gemPrefab.transform.rotation); // Maze
        Instantiate(gemPrefab, new Vector3(77.5f, 211.5f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(76.5f, 19.5f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(86.5f, 26.5f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(91.5f, 19.5f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(88.5f, 26.5f, 0.0f), gemPrefab.transform.rotation); // Maze
        Instantiate(gemPrefab, new Vector3(91f, 36.5f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(95f, 38.5f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(99f, 40.5f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(103f, 42.5f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(114.4f, 46.5f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(116.5f, 46.5f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(118.5f, 46.4f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(50.5f, 34.5f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(45.5f, 31.5f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(134f, 33.5f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(138f, 38.5f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(142f, 41.5f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(145f, 41.5f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(148f, 41.5f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(95f, -7.5f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(97f, -7.5f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(99f, -7.5f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(120.5f, 2.5f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(126.5f, 2.5f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(126.5f, 18.5f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(157.5f, 8.5f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(159.5f, 8.5f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(161.5f, 8.5f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(115f, 37f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(117f, 37f, 0.0f), gemPrefab.transform.rotation);
        Instantiate(gemPrefab, new Vector3(119f, 37f, 0.0f), gemPrefab.transform.rotation);
    }
}
