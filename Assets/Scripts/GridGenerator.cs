using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour {

    [Header("References")]
    [SerializeField] private GameObject gridCube;
    private Grid grid;

    [Header("Grid Settings")]
    [SerializeField][Tooltip("Must be odd")] private int width;
    [SerializeField][Tooltip("Must be odd")] private int length;
    private Dictionary<Vector3Int, GameObject> gridCubes;

    private void Start() {

        grid = GetComponent<Grid>();
        gridCubes = new Dictionary<Vector3Int, GameObject>();

        GenerateGrid();

    }

    private void GenerateGrid() {

        for (int x = -width / 2; x < width / 2f; x++) {

            for (int z = -length / 2; z < length / 2f; z++) {

                Vector3Int position = new Vector3Int(x, 0, z);
                gridCubes.Add(position, Instantiate(gridCube, position, Quaternion.identity, transform));

            }
        }
    }

    public bool IsPositionValid(Vector3Int position) => gridCubes.ContainsKey(position);

    public Grid GetGrid() => grid;

}
