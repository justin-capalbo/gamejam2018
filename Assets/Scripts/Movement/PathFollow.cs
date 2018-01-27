using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// Add this class to an object to make it follow a path
/// </summary>
public class PathFollow : MonoBehaviour
{
    /// the type of follow behavior	
    public enum FollowType
    {
        MoveTowards,
        Lerp
    }
    /// the follow behavior
    public FollowType followType = FollowType.MoveTowards;
    /// the path to follow
    public Path path;
    /// the movement speed
    public float speed = 1;
    /// the maximum distance to goal
    public float maxDistanceToGoal = .1f;
    public Vector3 currentSpeed;

    private IEnumerator<Transform> currentPoint;
    public BoxCollider2D boxCollider { get; private set; }

    /// <summary>
    /// Initialization
    /// </summary>
    public void Start()
    {
        // if the path is null we trigger an error and exit
        if (path == null)
        {
            Debug.LogError("Path Cannot be null", gameObject);
            return;
        }

        if (transform.GetComponent<BoxCollider2D>() != null)
            boxCollider = transform.GetComponent<BoxCollider2D>();

        // storage
        currentPoint = path.GetPathEnumerator();
        currentPoint.MoveNext();

        if (currentPoint.Current == null)
            return;

        // initial positioning
        transform.position = currentPoint.Current.position;
    }

    /// <summary>
    /// Every frame, we make the object follow its path
    /// </summary>
    public void Update()
    {
        if (currentPoint == null || currentPoint.Current == null)
            return;

        Vector3 initialPosition = transform.position;

        if (followType == FollowType.MoveTowards)
            transform.position = Vector3.MoveTowards(transform.position, currentPoint.Current.position, Time.deltaTime * speed);
        else if (followType == FollowType.Lerp)
            transform.position = Vector3.Lerp(transform.position, currentPoint.Current.position, Time.deltaTime * speed);

        var distanceSquared = (transform.position - currentPoint.Current.position).sqrMagnitude;
        if (distanceSquared < maxDistanceToGoal * maxDistanceToGoal)
            currentPoint.MoveNext();

        Vector3 finalPosition = transform.position;
        currentSpeed = (finalPosition - initialPosition) / Time.deltaTime;
    }
}
