using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField]
    private LayerMask unwalkableMask;
    [SerializeField]
    private LayerMask whatIsGround;
    [SerializeField]
    private Vector2 gridWorldSize;
    [SerializeField]
    private float nodeRadius;

    [SerializeField]
    private Pathfinding pathfinding;

    private float nodeDiameter;
    private int gridSizeX, gridSizeY;
    private Node[,] grid;
    private List<Node> path = new List<Node>();
    private Vector3 originPosition;

    public GameObject cellPrefab;

    private void Awake()
    {
        originPosition = transform.position - ((Vector3.right * gridWorldSize.x) / 2) - ((Vector3.forward * gridWorldSize.y) / 2);
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    private void Update()
    {

    }

    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        float height;

        for (int x = 0; x<gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = originPosition + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                Ray groundCheckRay = new Ray(new Vector3(worldPoint.x, 100f, worldPoint.z), Vector3.down);
                bool thereIsGround = Physics.Raycast(groundCheckRay, out RaycastHit raycastHit, 999f, whatIsGround);
                if (!thereIsGround)
                    continue;
                else
                {
                    height = raycastHit.point.y;
                    worldPoint.y = height;

                    bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius - .1f, unwalkableMask));
                    grid[x, y] = new Node(worldPoint, x, y, height, walkable);
                }

            }
        }
    }

    /// <summary>
    /// Returns given node's neighbours for 4 direction.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public List<Node> GetNeighbourNodes4(Node node)
    {
        List<Node> neighbours = new List<Node>();

        int x, y;
        node.GetCoordinates(out x, out y);

        if (x - 1 < gridSizeX && x - 1 >= 0)
        {
            neighbours.Add(grid[x - 1, y]);
        }

        if (x + 1 < gridSizeX)
        {
            neighbours.Add(grid[x + 1, y]);
        }

        if (y - 1 < gridSizeY && y - 1 >= 0)
        {
            neighbours.Add(grid[x, y - 1]);
        }

        if (y + 1 < gridSizeY)
        {
            neighbours.Add(grid[x, y + 1]);
        }

        return neighbours;
    }

    /// <summary>
    /// Returns given node's neighbours for 8 direction.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public List<Node> GetNeighbourNodes8(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                int gridX, gridY;
                node.GetCoordinates(out gridX, out gridY);
                int checkX = gridX + x;
                int checkY = gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public List<Node> GetPath() => path;
    public void SetPath(List<Node> path)
    {
        this.path = path;
    }

    public List<Node> GetRangeOfNodesForMove(Entity entity)
    {
        GameObject[] cells = GameObject.FindGameObjectsWithTag("Finish");
        for (int i = 0; i < cells.Length; i++)
        {
            Destroy(cells[i]);
        }

        List<Node> nodes = new List<Node>();
        Node centerNode = NodeFromWorldPoint(entity.transform.position);
        int centerNodeX, centerNodeY;
        centerNode.GetCoordinates(out centerNodeX, out centerNodeY);

        int actionRange = entity.GetSpeed() * entity.GetActionPoints();

        for (int x = -actionRange; x <= actionRange; x++)
        {
            for (int y = -actionRange; y <= actionRange; y++)
            {
                if(x == 0 && y == 0)
                {
                    continue;
                }

                if (Mathf.Abs(x) + Mathf.Abs(y) <= actionRange)
                {
                    int checkX = centerNodeX + x;
                    int checkY = centerNodeY + y;

                    if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    {
                        if(grid[checkX, checkY] != null && grid[checkX, checkY].GetWalkable())
                        {
                            pathfinding.FindPath(centerNode.GetWorldPosition(), grid[checkX, checkY].GetWorldPosition(), actionRange);
                            if(path.Contains(grid[checkX, checkY]))
                            {
                                nodes.Add(grid[checkX, checkY]);
                                GameObject cell = Instantiate(cellPrefab, grid[checkX, checkY].GetWorldPosition() + new Vector3(0.0f, 0.05f, 0.0f), cellPrefab.transform.rotation);
                                cell.GetComponent<SpriteRenderer>().color = Color.green;
                            }
                        }
                    }
                }
            }
        }

        return nodes;
    }

    public List<Node> GetRangeOfNodesForAttack(Entity entity)
    {
        GameObject[] cells = GameObject.FindGameObjectsWithTag("Finish");
        for (int i = 0; i < cells.Length; i++)
        {
            Destroy(cells[i]);
        }

        List<Node> nodes = new List<Node>();
        Node centerNode = NodeFromWorldPoint(entity.transform.position);
        int centerNodeX, centerNodeY;
        centerNode.GetCoordinates(out centerNodeX, out centerNodeY);

        int actionRange = entity.GetRange();

        Vector3 rayOrigin = centerNode.GetWorldPosition() + new Vector3(0.0f, GetNodeSize(), 0.0f);

        for (int x = -actionRange; x <= actionRange; x++)
        {
            for (int y = -actionRange; y <= actionRange; y++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                if (Mathf.Abs(x) + Mathf.Abs(y) <= actionRange)
                {
                    int checkX = centerNodeX + x;
                    int checkY = centerNodeY + y;

                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    {
                        if(grid[checkX, checkY] != null)
                        {
                            Vector3 target = grid[checkX, checkY].GetWorldPosition() + new Vector3(0.0f, GetNodeSize(), 0.0f);
                            Vector3 dir = (target - rayOrigin).normalized;
                            Ray ray = new Ray(rayOrigin, dir);

                            bool thereIsWall = Physics.Raycast(ray, out RaycastHit raycastHit, Vector3.Distance(rayOrigin, target), whatIsGround);
                            if (thereIsWall)
                            {
                                continue;
                            }

                            if (grid[checkX, checkY].GetWalkable())
                            {
                                nodes.Add(grid[checkX, checkY]);

                                GameObject cell = Instantiate(cellPrefab, grid[checkX, checkY].GetWorldPosition() + new Vector3(0.0f, 0.05f, 0.0f), cellPrefab.transform.rotation);
                                cell.GetComponent<SpriteRenderer>().color = Color.red;
                            }
                        }
                    }
                }
            }
        }

        return nodes;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        
        /*
        float percentX = Mathf.Clamp01((worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x);
        float percentY = Mathf.Clamp01((worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        */

        int x = Mathf.FloorToInt((worldPosition - originPosition).x / nodeDiameter);
        int y = Mathf.FloorToInt((worldPosition - originPosition).z / nodeDiameter);

        //int x = Mathf.RoundToInt(worldPosition.x + gridWorldSize.x / 2);
        //int y = Mathf.RoundToInt(worldPosition.z + gridWorldSize.y / 2);

        if (x >= 0 && y >= 0 && x < gridSizeX && y < gridSizeY)
        {
            return grid[x, y];
        }
        return null;
    }

    public float GetNodeSize() => nodeRadius * 2;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        /*if (grid != null)
        {
            //Node playerNode = NodeFromWorldPoint(CombatScene.Instance.GetEntity().transform.position);
            foreach (Node n in grid)
            {
                if(n != null)
                {
                    Gizmos.color = (n.GetWalkable()) ? Color.white : Color.red;
                    if (path.Contains(n))
                    {
                        Gizmos.color = Color.blue;
                    }
                    if (n.GetHasEntity())
                    {
                        Gizmos.color = Color.cyan;
                    }
                    //if (playerNode == n && playerNode != null)
                    //{
                    //    Gizmos.color = Color.green;
                    //}
                    //Gizmos.DrawCube(n.GetWorldPosition(), Vector3.one * nodeDiameter);
                    Gizmos.DrawCube(n.GetWorldPosition(), new Vector3(1f, 0.05f, 1f) * nodeDiameter);
                }
            }
        }*/
    }
}
