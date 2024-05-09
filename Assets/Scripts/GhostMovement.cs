using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    Vector2[] dirArray = { Vector2.up, Vector2.right, Vector2.down, Vector2.left };
    private bool canGoToNextDot;
    public LayerMask dotLayer;
    Rigidbody2D rb;
    Animator anim;

    public float speed = 1f;
    public Vector2 curDirection;
    public Vector3 jailPoint;
    public List<Dot> path;

    private Pathfinding aStar;

    [SerializeField]private Dot nextDot;
    // Start is called before the first frame update

    private void Awake()
    {
        aStar = GetComponent<Pathfinding>();
    }
    void Start()
    {
        canGoToNextDot = true;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        curDirection = dirArray[0];
        transform.position = CurrentDot().transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
        path = aStar.GhostPath;
        MovementDirection();

    }

    void MovementDirection()
    {
        transform.Translate(curDirection.normalized * speed * Time.deltaTime);
        if (canGoToNextDot)
        {
            canGoToNextDot = false;
            if (path.Count > 0)
            {
                Debug.Log(path.Count);
                nextDot = path[0];
                path.RemoveAt(0);
            }
            curDirection = dirArray[DirectionIndex()];
            anim.SetInteger("DirectionIndex", DirectionIndex());
        }
        else
        {
            if (Vector2.Distance(transform.position, nextDot.transform.position) <= .03f)
            {
                transform.position = nextDot.transform.position;
                canGoToNextDot = true;
            }
        }    

    }

    private int DirectionIndex()
    {
        if (CurrentDot().transform.position.y < nextDot.transform.position.y) return 0;
        else if (CurrentDot().transform.position.x < nextDot.transform.position.x) return 1;
        else if (CurrentDot().transform.position.y > nextDot.transform.position.y) return 2;
        else return 3;
    }    
    private Dot CurrentDot()
    {
        Collider2D[] dots= Physics2D.OverlapCircleAll(transform.position, .08f, dotLayer);
        return dots[0].GetComponent<Dot>() ;
    }    

    private void OnTriggerEnter2D(Collider2D other)
    {     
        if (other.gameObject.CompareTag("Player")) 
        {
            GameController.instance.GameOver();
        }

        if (other.gameObject.CompareTag("Teleport"))
        {
            rb.transform.position = new Vector3(rb.transform.position.x * -.95f, rb.transform.position.y, rb.transform.position.z);
        }
    }

  

}
