using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseBehavior : MonoBehaviour
{
    public GameObject leftClick;
    public GameObject rightClick;
    // private Rigidbody2D leftClickRb;
    // private Rigidbody2D rightClickRb;
    public List<GameObject> mouses;
    public int mouseNumber;
    public bool zerovar;
    public bool onevar;
    public bool hitRight;
    public bool spacevar;
    Vector2 mousePos;

    public GameObject spaceBar;
    private ArrowBehavior arrowBehaviorScript;
    public GameManager gameManagerScript;




    // Start is called before the first frame update
    void Start()
    {
        //leftClickRb = leftClick.GetComponent<Rigidbody2D>();
        //rightClickRb = rightClick.GetComponent<Rigidbody2D>();
        
        //gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {


        if (Input.GetKeyDown(KeyCode.X))
        {
             Destroy(mouses[2]);
        }



        /*if (Input.GetMouseButtonDown(1) || isRightTrig == false)
        {
            Destroy(rightClick);
         
        }

        if (Input.GetMouseButtonDown(0) || isLeftTrig == false)
        {
            Destroy(leftClick);
        }*/

        if (Input.GetMouseButton(0) == true)
        {
            zerovar = true;
            OnMouseDown();
        }
        else if (Input.GetMouseButton(1) == true)
        {
            onevar = true;
            OnMouseDown();
        }
        //else if (Input.GetKeyDown(KeyCode.Space))
        //{
        //  spacevar = true;

        // }
        else
        {
            zerovar = false;
            onevar = false;
            //spacevar = true;
        }


        /* if(arrowBehaviorScript.canDestroySpace == true)
         {
             Debug.Log("heyo");
             Destroy(spaceBar);
         }*/

        



    }


    void OnMouseDown()
    {



        if (onevar == true)
        {
            //Destroy(mouses[0]);
            //Debug.Log(this.gameObject.name);

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D rayHit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (rayHit.collider != null)
            {
                if (rayHit.collider.gameObject.name == "right click final indicator")//(rayHit.collider.gameObject.CompareTag("Right Click"))
                {
                    Destroy(mouses[0]);
                }



            }
        }

        if (zerovar == true)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D rayHit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (rayHit.collider != null)
            {
                if (rayHit.collider.gameObject.name == "left click final indicator")//(rayHit.collider.gameObject.CompareTag("Left Click"))
                {
                    Destroy(mouses[1]);
                }



            }



        }



    }

    //Look at OnMouseOver
    /*public void GetSpaceDown()
    {
        if (spacevar == true)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D rayHit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (rayHit.collider != null)
            {
                if (rayHit.collider.gameObject.CompareTag("Space Bar"))
                {
                    Debug.Log("spoosh!");
                    Destroy(mouses[2]);
                }

            }
        }
    }*/


    

        
        
    }



    

