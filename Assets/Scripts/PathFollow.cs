using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollow : MonoBehaviour {

    [Header("Path")]
    private List<Vector3Int> currPath;
    private int nextWaypoint;

    [Header("Settings")]
    [SerializeField] private float speed;

    public void StartPathFollow(List<Vector3Int> currPath) {

        this.currPath = currPath;
        nextWaypoint = 1;

        StartCoroutine(HandlePathFollow());

    }

    private IEnumerator HandlePathFollow() {

        while (nextWaypoint < currPath.Count) {

            Vector3Int targetPosition = currPath[nextWaypoint]; // get next waypoint
            Vector3 direction = (targetPosition - transform.position).normalized; // get direction to next waypoint
            transform.position += direction * speed * Time.deltaTime; // move towards next waypoint

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f) // if close enough to waypoint
                nextWaypoint++; // move to next waypoint

            yield return null;

        }
    }
}
