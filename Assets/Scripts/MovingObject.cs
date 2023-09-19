using System.Collections;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    public float moveTime = 0.1f; // move time in seconds
    public LayerMask blockingLayer; // for collision checking

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float inverseMoveTime;
    protected bool isMoving; // fix bug with food
    // Start is called before the first frame update
    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;
    }
    /// <summary>
    /// Move units from one space to another
    /// </summary>
    /// <param name="end">point to move to</param>
    /// <returns></returns>
    protected IEnumerator SmoothMovement(Vector3 end)
    {
        isMoving = true;
        float sqrRemDist = (transform.position - end).sqrMagnitude;
        while (sqrRemDist > float.Epsilon)
        {
            var newPos = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            // rb2D.MovePosition(newPos); // move collider to match sprite
            rb2D.position = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime); // collision fix
            sqrRemDist = (transform.position - end).sqrMagnitude;
            yield return null;
        }
        isMoving = false;
    }

    /// <summary>
    /// Try to move to position and return status
    /// </summary>
    /// <param name="xDir">x direction of move</param>
    /// <param name="yDir">y direction of move</param>
    /// <param name="hit">ray hit position</param>
    /// <returns>whether move was successful or not</returns>
    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position; // current transform pos
        Vector2 end = start + new Vector2(xDir, yDir); // end pos based on dir params

        boxCollider.enabled = false; // to not hit own collider
        hit = Physics2D.Linecast(start, end, blockingLayer); // cast a line from start to end checking collision on blockingLayer
        boxCollider.enabled = true;

        if (hit.transform == null && !isMoving) // no collision => space open
        {
            StartCoroutine(SmoothMovement(end));
            return true; // move successful
        }
        return false; // move unsuccessful
    }

    /// <summary>
    /// Attempt to move and handle object interaction
    /// </summary>
    /// <typeparam name="T">component type</typeparam>
    /// <param name="xDir">x direction of move</param>
    /// <param name="yDir">y direction of move</param>
    protected virtual void AttemptMove<T>(int xDir, int yDir) where T: Component
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        if (hit.transform == null) // no interaction
            return; // no need to execute following code

        T hitComponent = hit.transform.GetComponent<T>(); // get reference to the object that was hit
        if (!canMove && hitComponent != null) // moving object is blocked and has hit something it can interact with
            OnCantMove(hitComponent);
    }

    /// <summary>
    /// Handle object interaction
    /// </summary>
    /// <typeparam name="T">component type</typeparam>
    /// <param name="component">component to interact with</param>
    protected abstract void OnCantMove<T>(T component) where T : Component;

}
