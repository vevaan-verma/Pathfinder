using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathGenerator : MonoBehaviour {

    // A* Pathfinding Algorithm
    [Header("References")]
    private GridGenerator gridGenerator;
    private PathFollow pathFollow;
    private Grid grid;

    [Header("Pathfinder")]
    [SerializeField] private Transform target;
    private Vector3Int targetPos;
    private Vector3Int lastTargetPos;
    private List<Vector3Int> path;

    private void Start() {

        gridGenerator = FindObjectOfType<GridGenerator>();
        pathFollow = GetComponent<PathFollow>();

        grid = gridGenerator.GetGrid();
        path = new List<Vector3Int>();

        // set values equal to avoid calculating path twice
        targetPos = grid.WorldToCell(target.position);
        lastTargetPos = targetPos;

        StartCoroutine(CalculatePath());

    }

    private void Update() {

        targetPos = grid.WorldToCell(target.position);

        if (targetPos != lastTargetPos) {

            lastTargetPos = targetPos;
            StopAllCoroutines();
            StartCoroutine(CalculatePath());

        }
    }

    private IEnumerator CalculatePath() {

        Vector3Int currPosition = grid.WorldToCell(transform.position);

        int steps = 0;
        Frontier frontier = new Frontier();
        List<Node> explored = new List<Node>();
        Node node = new Node(currPosition, null, steps, Vector3Int.Distance(currPosition, grid.WorldToCell(target.position)));

        frontier.Add(node);

        while (!frontier.IsEmpty()) { // while path exists

            node = frontier.Pop();
            steps++;

            if (node.GetPosition() == targetPos) { // goal check passed

                path.Clear();
                path.Add(node.GetPosition());

                while (node.GetParent() != null) {

                    node = node.GetParent();
                    path.Add(node.GetPosition());

                }

                path.Reverse();
                pathFollow.StartPathFollow(path);
                print("Path Found");
                yield break;

            }

            explored.Add(node);

            Vector3Int[] neighbors = GetNeighboringCells(node.GetPosition());
            Node least = null;

            for (int i = 0; i < neighbors.Length; i++) {

                Node neighbor = new Node(neighbors[i], node, steps, Vector3Int.Distance(neighbors[i], targetPos));

                if ((least == null || neighbor.GetFullCost() < least.GetFullCost()) && !explored.Contains(neighbor))
                    least = neighbor;

            }

            if (least != null)
                frontier.Add(least);

            yield return null;

        }

        path.Clear();
        print("No Path Found");

    }

    private Vector3Int[] GetNeighboringCells(Vector3Int currCell) {

        List<Vector3Int> neighbors = new List<Vector3Int>();

        Vector3Int forward = currCell + Vector3Int.forward;
        Vector3Int left = currCell + Vector3Int.left;
        Vector3Int back = currCell + Vector3Int.back;
        Vector3Int right = currCell + Vector3Int.right;

        if (gridGenerator.IsPositionValid(forward)) neighbors.Add(forward);
        if (gridGenerator.IsPositionValid(left)) neighbors.Add(left);
        if (gridGenerator.IsPositionValid(back)) neighbors.Add(back);
        if (gridGenerator.IsPositionValid(right)) neighbors.Add(right);

        return neighbors.ToArray();

    }

    private void OnDrawGizmos() {

        if (path == null) return;

        for (int i = 0; i < path.Count; i++) {

            Vector3 position = grid.WorldToCell(path[i]);
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(position, Vector3.one * 0.5f);

            if (i > 0) {

                Vector3 prevPosition = grid.WorldToCell(path[i - 1]);
                Gizmos.DrawLine(prevPosition, position);

            }
        }
    }

    public List<Vector3Int> GetPath() => path;

}

class Frontier {

    private List<Node> nodes = new List<Node>();

    public void Add(Node node) => nodes.Add(node);

    public Node Pop() {

        Node node = nodes[0];
        nodes.RemoveAt(0);
        return node;

    }

    public bool Contains(Node node) => nodes.Contains(node);

    public bool IsEmpty() => nodes.Count == 0;

    public int Count() => nodes.Count;

    public void Clear() { nodes.Clear(); }

}

class Node {

    private Vector3Int position;
    private Node parent;
    private int gCost;
    private float hCost;

    public Node(Vector3Int position, Node parent, int gCost, float hCost) {

        this.position = position;
        this.parent = parent;
        this.gCost = gCost;
        this.hCost = hCost;

    }

    public Vector3Int GetPosition() => position;

    public Node GetParent() => parent;

    public int GetGCost() => gCost;

    public float GetHCost() => hCost;

    public float GetFullCost() => gCost + hCost;

    public override bool Equals(object obj) {

        if (obj is not Node) return false;

        Node node = (Node) obj;
        return position == node.GetPosition();

    }

    public override int GetHashCode() => base.GetHashCode();

}