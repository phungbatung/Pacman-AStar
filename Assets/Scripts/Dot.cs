using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour, IheapItem<Dot>
{
    public Dot parent;
    public GameObject[] neighbors;    
    public bool hasCollected = false;

    public float gCost, hCost;
    public int heapIndex;


    int scorePerDot = 10;
    float radius = 0.16F;
    [SerializeField] private LayerMask dotLayer;

    GameController controller;


    void Start()
     {
        FindNodes();
        GameObject gc = GameObject.FindGameObjectWithTag("GameController");
        controller = gc.GetComponent<GameController>();
     }
    
    //Finds neighboring nodes within a radius of a node
    void FindNodes()
    {
        List<GameObject> list = new List<GameObject>();
        Collider2D[] Adjacent =
            Physics2D.OverlapCircleAll(transform.position, radius, dotLayer);
       foreach (var neighbor in Adjacent)
            list.Add(neighbor.gameObject);

        neighbors = list.ToArray();
    }

    public Dot dotLocation(GameObject loc) {
        return GetComponent<Dot>();
    }
    public float fCost
    {
        get{ return gCost + hCost; }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (hasCollected == false)
            {
                hasCollected = true;
                GetComponent<SpriteRenderer>().enabled = false;
                controller.score += scorePerDot;
                controller.dotsCollected++;
                controller.SetScoreText();
            }
        }
    }
    public int index
    {
        get { return heapIndex; }
        set { heapIndex = value; }
    }

    public int CompareTo(Dot otherDot)
    {
        int compare = fCost.CompareTo(otherDot.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(otherDot.hCost);
        }
        return -compare;
    }
}
