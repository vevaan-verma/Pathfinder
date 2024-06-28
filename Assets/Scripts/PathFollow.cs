using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollow : MonoBehaviour {

    [Header("References")]
    private Rigidbody rb;

    [Header("Path")]
    private List<Vector3Int> currPath;
    private int nextWaypoint;

    [Header("Settings")]
    [SerializeField] private float speed;
    [SerializeField] private float waypointLeniency;

    private void Start() {

        rb = GetComponent<Rigidbody>();

    }

    public void StartPathFollow(List<Vector3Int> currPath) {

        this.currPath = currPath;
        nextWaypoint = 1;

        StartCoroutine(HandlePathFollow());

    }

    private IEnumerator HandlePathFollow() {

        while (nextWaypoint < currPath.Count) {

            print(nextWaypoint);
            Vector3Int targetPosition = currPath[nextWaypoint]; // get next waypoint
            Vector3 direction = (targetPosition - transform.position).normalized; // get direction to next waypoint
            rb.velocity = direction * speed; // move towards next waypoint

            if (Vector3.Distance(transform.position, targetPosition) < waypointLeniency) { // if close enough to waypoint

                rb.velocity = Vector3.zero; // stop moving
                rb.position = targetPosition; // snap to waypoint
                nextWaypoint++; // move to next waypoint

            }

            yield return null;

        }
    }
}
