using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtalControllerV2 : MonoBehaviour
{
    public enum PatrolDirection
    {
        LEFT,
        RIGHT
    }
    public enum State
    {
        ATTACK,
        FOLLOWING,
        PATROL
    }
    public Animator animator;
    bool isAttacking;
    bool isWalking;

    State turtleState;
    bool followingPlayer;
    bool faceLeft;
    Vector2 patrolDestination;
    PatrolDirection nextPatrol;
    bool isPatrolling;
    bool readyToPatrol;

    float walkSpeed = 3f;
    float attackRange = 6f;

    Transform Player = null;
    public Rigidbody2D rb;
    public Transform BodySprite;
    public GameObject TurtleSpit;
    public Transform SpitSpawnPos;

    float delta = 0.1f;
    void Start()
    {
        faceLeft = true;
        turtleState = State.PATROL;
    }

    // Update is called once per frame
    void Update()
    {
        DetectionRange();
        PlayerInRange();
        SetState();
    }
    void PlayerInRange()
    {
        if(Player!= null)
        {
            if ((Player.position - transform.position).sqrMagnitude < attackRange * attackRange)
            {
                turtleState = State.ATTACK;
            }
        }
    }
    void SetState()
    {
        
        if(turtleState == State.PATROL)
        {
            Patrolling();
        }
        else if(turtleState == State.FOLLOWING)
        {
            if (!isAttacking)
            {
                FollowingPlayer();
            }
        }
        else if(turtleState == State.ATTACK)
        {
            if (!isAttacking)
            {
                AttackPlayer();
            }
            animator.SetBool("isWalking", false);
        }
        if(turtleState != State.ATTACK)
        {
            FlippingToDirection();
            if(Mathf.Abs(rb.velocity.x) > delta && !isAttacking)
            {
                animator.SetBool("isWalking", true);
            }
            else
            {
                animator.SetBool("isWalking", false);
            }
        }
    }
    void AttackPlayer()
    {
        if (Player.position.x - transform.position.x < -delta && faceLeft == false)
        {
            BodySprite.localScale = new Vector3(BodySprite.localScale.x * -1, BodySprite.localScale.y, BodySprite.localScale.z);
            faceLeft = true;
        }
        else if (Player.position.x - transform.position.x > delta && faceLeft == true)
        {
            BodySprite.localScale = new Vector3(BodySprite.localScale.x * -1, BodySprite.localScale.y, BodySprite.localScale.z);
            faceLeft = false;
        }
        animator.SetBool("isAttacking", true);
        animator.SetBool("isWalking", false);
        isAttacking = true;
    }
    public void OnFinishAttack()
    {
        Debug.Log("woo fireball");
        //currentAnimation = BossTurtle.IDLE;
        GameObject turtleSpit = Instantiate(TurtleSpit);
        turtleSpit.transform.position = SpitSpawnPos.position;
        SpitBomb spit = turtleSpit.GetComponent<SpitBomb>();
        if (faceLeft)
        {
            spit.Fly(-Vector2.right);
        }
        else
        {
            spit.Fly(Vector2.right);
        }
        StartCoroutine(OnFinishedAttacking());
    }
    IEnumerator OnFinishedAttacking()
    {
        animator.SetBool("isAttacking", false);
        yield return new WaitForSeconds(2f);
        isAttacking = false;
    }
    void FollowingPlayer()
    {
        if (Player != null && turtleState != State.ATTACK)
        {
            if (Player.position.x - transform.position.x < 0)
            {
                rb.velocity = new Vector2(-walkSpeed, rb.velocity.y);

            }
            else if (Player.position.x - transform.position.x > 0)
            {
                rb.velocity = new Vector2(walkSpeed, rb.velocity.y);
            }
        }
    }
    void DetectionRange()
    {
        Collider2D collider = Physics2D.OverlapBox(transform.position + Vector3.up * 2.25f, Vector2.one * 5 + Vector2.right * 15, 0f, LayerMask.GetMask("Player"));
        if (collider != null)
        {
            if (collider.CompareTag("Player"))
            {
                turtleState = State.FOLLOWING;
                followingPlayer = true;
                Player = collider.transform;
            }
        }
    }
    void FlippingToDirection()
    {
        if (rb.velocity.x < -delta && faceLeft == false)
        {
            BodySprite.localScale = new Vector3(BodySprite.localScale.x * -1, BodySprite.localScale.y, BodySprite.localScale.z);
            faceLeft = true;
        }
        else if (rb.velocity.x > delta && faceLeft == true)
        {
            BodySprite.localScale = new Vector3(BodySprite.localScale.x * -1, BodySprite.localScale.y, BodySprite.localScale.z);
            faceLeft = false;
        }
    }
    
    #region Patrolling 
    void Patrolling()
    {
        if (!isPatrolling)
        {
            StartCoroutine(NewPatrolDestination());
        }
        if (readyToPatrol && patrolDestination != null)
        {
            MoveToPatrolDes();
        }
    }
    void MoveToPatrolDes()
    {
        if (nextPatrol == PatrolDirection.LEFT)
        {
            rb.velocity = new Vector2(-walkSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(walkSpeed, rb.velocity.y);
        }
        if (Mathf.Abs(transform.position.x - patrolDestination.x) < delta)
        {
            readyToPatrol = false;
            isPatrolling = false;
            if (nextPatrol == PatrolDirection.LEFT)
            {
                nextPatrol = PatrolDirection.RIGHT;
            }
            else
            {
                nextPatrol = PatrolDirection.LEFT;
            }
        }
    }
    IEnumerator NewPatrolDestination()
    {
        isPatrolling = true;
        yield return new WaitForSeconds(2f);
        if (nextPatrol == PatrolDirection.LEFT)
        {
            patrolDestination = new Vector2(transform.position.x - 5f, transform.position.y);
        }
        else
        {
            patrolDestination = new Vector2(transform.position.x + 5f, transform.position.y);
        }
        readyToPatrol = true;
    }
    #endregion
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 pos = transform.position;
        pos.y += 2.25f;
        Gizmos.DrawWireCube(pos, Vector3.one * 5 + Vector3.right * 15);

    }
}
