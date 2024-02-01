using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatScene : MonoBehaviour
{
    public static CombatScene Instance { get; set; }

    [SerializeField]
    private Grid grid;
    [SerializeField]
    private Pathfinding pathfinding;

    [SerializeField]
    private Entity entity;
    [SerializeField]
    private Transform cell;

    [SerializeField]
    private GameState gameState;
    [SerializeField]
    private TurnState turnState;
    [SerializeField]
    private MinionState minionState;
    [SerializeField]
    private LeanTweenType leanType;

    private List<Node> actionNodes = new List<Node>();
    private Vector3 pos;

    public enum TurnState
    {
        PLAYER,
        ENEMY
    }

    public enum GameState
    {
        WAIT,
        PLAY
    }

    public enum MinionState
    {
        CHOOSE,
        ATTACK,
        MOVE
    }

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        Entity[] entities = FindObjectsOfType<Entity>();
        foreach (var entity in entities)
        {
            Node node = grid.NodeFromWorldPoint(entity.transform.position);
            node.SetHasEntity(true);
            node.SetEntity(entity);
        }
    }

    void Update()
    {
        switch (turnState)
        {
            case TurnState.PLAYER:
                switch (gameState)
                {
                    case GameState.WAIT:
                        break;
                    case GameState.PLAY:
                        Node node = grid.NodeFromWorldPoint(Mouse3D.GetMouseWorldPosition());
                        if (node != null)
                        {
                            pos = node.GetWorldPosition();
                            if (pos != null)
                            {
                                cell.gameObject.GetComponent<SpriteRenderer>().color = node.GetWalkable() ? Color.white : Color.black;
                                cell.transform.position = pos + new Vector3(.0f, .01f, .0f);
                            }
                        }
                        if (Input.GetMouseButtonDown(0))
                        {
                            switch (minionState)
                            {
                                case MinionState.CHOOSE:
                                    if (node != null && node.GetHasEntity())
                                    {
                                        entity = node.GetEntity();
                                    }
                                    break;
                                case MinionState.MOVE:
                                    if(entity == null)
                                    {
                                        Debug.Log("You must choose entity");
                                        SetMinionState(MinionState.CHOOSE);
                                    }

                                    actionNodes = grid.GetRangeOfNodesForMove(entity);
                                    if (node != null && !node.GetHasEntity() && node.GetWalkable() && actionNodes.Contains(node))
                                    {
                                        if (pos != null)
                                        {
                                            if (entity != null)
                                            {
                                                gameState = GameState.WAIT;
                                                pathfinding.FindPath(grid.NodeFromWorldPoint(entity.transform.position).GetWorldPosition(), node.GetWorldPosition(), entity.GetSpeed()*entity.GetActionPoints());
                                                entity.DecreaseActionPoints(Mathf.CeilToInt((float)grid.GetPath().Count / (float)entity.GetSpeed()));
                                                StartCoroutine(MoveEntity(0.4f));
                                            }
                                            else
                                            {
                                                SetMinionState(MinionState.CHOOSE);
                                            }
                                        }
                                    }
                                    else if(node != null && node.GetHasEntity())
                                    {
                                        entity = node.GetEntity();
                                    }
                                    break;
                                case MinionState.ATTACK:
                                    if (entity == null)
                                    {
                                        Debug.Log("You must choose entity");
                                        SetMinionState(MinionState.CHOOSE);
                                    }

                                    actionNodes = grid.GetRangeOfNodesForAttack(entity);
                                    if (node != null && node.GetHasEntity() && Mathf.Abs(node.GetHeight() - grid.NodeFromWorldPoint(entity.transform.position).GetHeight()) < 2 && actionNodes.Contains(node))
                                    {
                                        float distance = Vector3.Distance(entity.transform.position, node.GetWorldPosition())/grid.GetNodeSize();

                                        LeanTween.move(entity.gameObject, node.GetWorldPosition(), 0.25f*distance)
                                            .setEase(LeanTweenType.punch);
                                        node.GetEntity().SetPref(entity.GetAttack(), Entity.SetType.DECREASE, Entity.PrefType.HEALTH);
                                    }
                                    break;
                            }
                        }
                        break;
                }
                break;
            case TurnState.ENEMY:
                break;
            default:
                break;
        }
       
    }

    private IEnumerator MoveEntity(float stepTime)
    {
        for (int i = 0; i < grid.GetPath().Count; i++)
        {
            LeanTween.move(entity.gameObject, grid.GetPath()[i].GetWorldPosition(), stepTime).setEase(leanType);
            /*LeanTween.moveY(entity.gameObject, entity.transform.position.y * 1.5f, stepTime).setEase(LeanTweenType.punch).setOnComplete(() => 
            { 
                entity.transform.position = new Vector3(entity.transform.position.x, grid.GetPath()[i].GetHeight(), entity.transform.position.z);
            });*/
            grid.NodeFromWorldPoint(entity.transform.position).SetHasEntity(false);
            grid.NodeFromWorldPoint(entity.transform.position).SetEntity(null);
            grid.GetPath()[i].SetHasEntity(true);
            grid.GetPath()[i].SetEntity(entity);
            yield return new WaitForSeconds(stepTime);
        }
        gameState = GameState.PLAY;
    }

    public Entity GetEntity() => entity;
    public Grid GetGrid() => grid;
    public List<Node> GetActionNodes() => actionNodes;

    public void SetMinionState(MinionState minionState)
    {
        this.minionState = minionState;
    }

    public void SetMinionStateToAttack()
    {
        SetMinionState(MinionState.ATTACK);
    }

    public void SetMinionStateToMove()
    {
        SetMinionState(MinionState.MOVE);
    }

    private void OnDrawGizmos()
    {
            foreach (var node in actionNodes)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawCube(node.GetWorldPosition(), new Vector3(1f, 1f, 1f) * 0.9f);
            }
    }
}
