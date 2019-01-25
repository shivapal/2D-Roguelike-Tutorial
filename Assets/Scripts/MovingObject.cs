using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    public float moveTime;
    public LayerMask blockingLayer;  //layer for the blockingLayer

    private BoxCollider2D boxCollider;  //collider for this object
    private Rigidbody2D rb2d;  //rigidbody (physics) for this object
    private float inverseMoveTime; //makes some calculations more efficient instead of repeatedly doing divisions

    // Start is called before the first frame update
    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2d = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;
    }

    protected bool move(int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);  
        boxCollider.enabled = false;  //disable for the linecast
        hit = Physics2D.Linecast(start, end, blockingLayer); //null if nothing in the way
        boxCollider.enabled = true;
        if (hit.transform == null)
        {
            StartCoroutine(smoothMovement(end));
            return true;
        }
        return false;  //otherwise something is in the way
    }
    //try to move to the spot given, and if not run onCantMove()
    protected virtual void attemptedMove<T>(int xDir, int yDir)  //T is the type that the unit is seeking, i.e. wall for player
        where T : Component
    {
        RaycastHit2D hit;
        bool canMove = move(xDir, yDir,out hit); //moved successfully
        if (hit.transform == null)
        {
            return;
        }

        T hitComponent = hit.transform.GetComponent<T>();
        if (!canMove && hitComponent !=null)  //something in the way so take the appropriate action
        {
            onCantMove<T> (hitComponent);
        }
    }
    

    protected IEnumerator smoothMovement(Vector3 end)
    {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2d.position, end, inverseMoveTime *Time.deltaTime);
            rb2d.MovePosition(newPosition);  //change the position of the rigidbody
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
    }

    protected abstract void onCantMove<T>(T component)
        where T : Component;
}
