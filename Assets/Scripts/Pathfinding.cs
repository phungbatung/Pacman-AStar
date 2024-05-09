using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public enum Heuristic
{
    Manhattan,
    Euclid,
    Chebyshev
};

public enum PathfindingAlgorithm
{
    AStar,
    DFS
};
public class Pathfinding : MonoBehaviour
{
    public PathfindingAlgorithm algorithm;
    public Heuristic heuristic;
    public int colorIndex;
    private Color[] color= {Color.blue, Color.red, Color.green, Color.cyan};
    public GameObject player;
    public GameObject ghost;

    public Dot playerDot;
    private Dot ghostDot;
    private float radius = 0.16f;
    [SerializeField] private LayerMask dotLayer;
    private int dotCount = 217;
    public List<Dot> GhostPath;
    /*private int i = 0;*/

    private void Start()
    {
        playerDot = FindLocation(player);
        ghostDot = FindLocation(ghost);

        if (algorithm == PathfindingAlgorithm.AStar)
            Astar(playerDot, ghostDot);
        else
            DFS(playerDot, ghostDot);
    }
    // Update is called once per frame
    void Update()
    {
        playerDot = FindLocation(player);
        ghostDot =  FindLocation(ghost);

        if (algorithm == PathfindingAlgorithm.AStar)
            Astar(playerDot, ghostDot);
        /*else
            DFS(playerDot, ghostDot);*/
    }
    
    //Finds the shortest route to a object
    void Astar(Dot playerDot, Dot ghostDot) 
    {
        if (playerDot ==null || ghostDot ==null) 
            return;
        Heap<Dot> openSet = new Heap<Dot>(dotCount);
        HashSet<Dot> closedSet = new HashSet<Dot>();
        openSet.Add(ghostDot);
        
        while (openSet.CurrentCapacity > 0)
        {
            
            Dot currentDot = openSet.RemoveFirst(); // lay node co f(n) nho nhat
            /*Debug.Log($"{i}: {currentDot.gameObject.name}");*/
            closedSet.Add(currentDot); //them node vua lay ra vao tap. do'ng

            if (currentDot == playerDot) 
            {
                /*Debug.Log("found player");*/
                RetracePath(ghostDot, playerDot);
                return;
            }

            foreach (var neighbor in currentDot.neighbors)
            {
                Dot neighborDot = neighbor.GetComponent<Dot>();
                if (closedSet.Contains(neighborDot)) // neu diem lan can thuoc tap dong thi bo qua
                    continue;
                float currentGCost = currentDot.gCost + GetDistance(currentDot, neighborDot); // tinh lai gia tri thuc te tu diem bat dau den diem dang xet


                if (currentGCost < neighborDot.gCost || !openSet.Contains(neighborDot))
                {
                    neighborDot.gCost = currentGCost;
                    neighborDot.hCost = GetDistance(neighborDot, playerDot);


                    neighborDot.parent = currentDot;

                    if (!openSet.Contains(neighborDot)) // check cho truong hop (currentGCost < neighborDot.gCost)
                        openSet.Add(neighborDot);
                }
            }
        }
        /*i++;*/
    }
    void DFS(Dot playerDot, Dot ghostDot)
    {
        int i = 0;
        Stack<Dot> stack = new Stack<Dot>();
        List<Dot> visited = new List<Dot>();
        stack.Push(ghostDot);
        visited.Add(ghostDot);
        while(stack.Count > 0)
        {
            Dot currentDot = stack.Pop();
            Debug.Log(currentDot.name);
            if (currentDot == playerDot)
            {
                Debug.Log("found player");
                RetracePath(ghostDot, playerDot);
                return;
            }
            foreach (var neighbor in currentDot.neighbors)
            {
                Dot neighborDot = neighbor.GetComponent<Dot>();
                if (visited.Contains(neighborDot))
                    continue;
                neighborDot.parent = currentDot;
                stack.Push(neighborDot);
                visited.Add(neighborDot);
            }
            i++;/*
            Debug.Log(i);*/
            if (i >= 300)
            {
                Debug.Log("break");
                return;
            }
        }
    }

    //Finds distance between a object and target
    //used to find hcost and gcost
    float GetDistance(Dot start, Dot end)
    {
        if (heuristic == Heuristic.Manhattan)
            return GetDistanceManhattan(start, end);
        else if (heuristic == Heuristic.Euclid)
            return GetDistanceEuclid(start, end);
        else
            return GetDistanceChebyshev(start, end);
    }    
    float GetDistanceManhattan(Dot start, Dot end)
    {
        Vector2 startPos = start.gameObject.transform.position;
        Vector2 endPos = end.gameObject.transform.position;

        float x = Mathf.Abs(startPos.x - endPos.x);
        float y = Mathf.Abs(startPos.y - endPos.y);
        return x + y;
    }
    float GetDistanceEuclid(Dot start, Dot end)
    {
        return Vector2.Distance(start.transform.position, end.transform.position);
    }
    float GetDistanceChebyshev(Dot start, Dot end)
    {
        return Mathf.Max(Mathf.Abs(start.transform.position.x - end.transform.position.x), Mathf.Abs(start.transform.position.y - end.transform.position.y));
    }

    //Finds closest dot to an object
    Dot FindLocation(GameObject gameobject)
    {
        Dot nearestDot = null;
        float minDistance = Mathf.Infinity;
        Vector2 location = gameobject.transform.position;

        Collider2D[] neighbors = Physics2D.OverlapCircleAll(location, radius, dotLayer);
        /*Debug.Log($"{LayerMask.NameToLayer(dotLayer.ToString()) }  {LayerMask.LayerToName(3)}  {neighbors.Length}");*/
        foreach (var neighbor in neighbors)
        {
            Vector2 neighborLocation = neighbor.gameObject.transform.position;

            //Cheaper way to do .Distance();
            Vector2 distance = neighborLocation - location;
            float distSqrTarget = distance.sqrMagnitude;

            if (distSqrTarget < minDistance)
            {
                minDistance = distSqrTarget;
                nearestDot = neighbor.gameObject.GetComponent<Dot>();
            }
        }
        
        return nearestDot;
    }

    //Most optimized path
    void RetracePath(Dot start, Dot end)
    {
        List<Dot> path = new List<Dot>();
        Dot current = end;
        while (current != start)
        {
            Debug.Log("log");
            path.Add(current);
            current = current.parent;
        }
        Debug.Log(path.Count);
        path.Reverse();
        /*string s = "";
        foreach (var dot in path)
        {
            s += dot.gameObject.name + "->";
        }

        Debug.Log(s);*/
        GhostPath = path;
    }



    private void OnDrawGizmos()
    {
        if (playerDot != null )
        {
            Gizmos.color = color[colorIndex];
            Gizmos.DrawLine(player.transform.position,
                playerDot.transform.position);
            if (GhostPath != null)
            {
                foreach (var path in GhostPath)
                {
                    Gizmos.DrawLine(path.transform.position,
                        path.parent.transform.position);
                }
            }
        }
    }
}
