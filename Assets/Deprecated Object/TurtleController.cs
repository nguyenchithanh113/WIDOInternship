using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class TurtleController : MonoBehaviour
{
    public enum PatrolDirection
    {
        LEFT,
        RIGHT
    }
    public SkeletonAnimation skeletonBone;
    public AnimationReferenceAsset idle, walking, attack;

    public Rigidbody2D rb;

    float walkSpeed = 3f;
    bool readyToPatrol;
    bool isPatrolling;
    Vector2 patrolDestination;
    PatrolDirection nextPatrol;

    bool followingPlayer;
    public bool isAttacking;
    float attackRange = 6f;


    string currentAnimation;

    public Transform BodySprite;
    public Transform Player;
    bool faceLeft;

    public GameObject TurtleSpit;
    public Transform SpitSpawnPos;

    float delta = 0.1f;
    void Start()
    {
        SetState(BossTurtle.IDLE);
        nextPatrol = PatrolDirection.LEFT;
        Spine.AnimationState state = skeletonBone.AnimationState;
        state.Complete += HandleEvent;
        faceLeft = true;
    }
    void HandleEvent(Spine.TrackEntry trackEntry)
    {
        if(trackEntry.Animation.Name == BossTurtle.ATTACK)
        {
            FireBall();
            
        }
    }
    void FireBall()
    {
        Debug.Log("woo fireball");
        //currentAnimation = BossTurtle.IDLE;
        SetState(BossTurtle.IDLE);
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
        yield return new WaitForSeconds(2f);
        isAttacking = false;
    }
    private void OnDestroy()
    {
        
    }
    void SetAnimation(AnimationReferenceAsset animation, bool loop, float timescale)
    {
        if (currentAnimation != animation.name)
        {
            skeletonBone.state.SetAnimation(0, animation, loop).TimeScale = timescale;
            currentAnimation = animation.name;
        }
        
    }
    void SetState(string animName)
    {
        if (animName == BossTurtle.IDLE)
        {
            SetAnimation(idle, true, 1f);
        }else if(animName == BossTurtle.WALKING)
        {
            SetAnimation(walking, true, 1f);
        }else if(animName == BossTurtle.ATTACK)
        {
            SetAnimation(attack, false, 1f);
            
        }
    }
    // Update is called once per frame
    void Update()
    {
        Attacking();
        if (!isAttacking)
        {
            FlippingToDirection();
            if (Mathf.Abs(rb.velocity.x) > delta)
            {
                SetState(BossTurtle.WALKING);
            }
            else
            {
                SetState(BossTurtle.IDLE);
            }
            Patrolling();
        }
        DectectionRange();
        if (followingPlayer)
        {
            FollowingPlayer();
        }
    }
    void Moving()
    {

    }
    void Attacking()
    {
        if (!isAttacking && Player != null)
        {
            if ((Player.position - transform.position).sqrMagnitude < attackRange * attackRange)
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
                isAttacking = true;
                SetState(BossTurtle.ATTACK);
            }
        }
    }
    void FollowingPlayer()
    {
        if (Player != null && !isAttacking)
        {
            if(Player.position.x - transform.position.x < 0)
            {
                rb.velocity = new Vector2(-walkSpeed, rb.velocity.y);

            }
            else if(Player.position.x - transform.position.x > 0)
            {
                rb.velocity = new Vector2(walkSpeed, rb.velocity.y);
            }
        }
    }
    void DectectionRange()
    {
            Collider2D collider = Physics2D.OverlapBox(transform.position + Vector3.up * 2.25f, Vector2.one * 5 + Vector2.right * 15, 0f,LayerMask.GetMask("Player"));
            if (collider != null)
            {
                if (collider.CompareTag("Player"))
                {
                    followingPlayer = true;
                    Player = collider.transform;
                }
            }
    }
    void FlippingToDirection()
    {
        if(rb.velocity.x < -delta && faceLeft == false)
        {
            BodySprite.localScale = new Vector3(BodySprite.localScale.x*-1, BodySprite.localScale.y, BodySprite.localScale.z);
            faceLeft = true;
        }else if(rb.velocity.x > delta && faceLeft == true)
        {
            BodySprite.localScale = new Vector3(BodySprite.localScale.x * -1, BodySprite.localScale.y, BodySprite.localScale.z);
            faceLeft = false;
        }
    }
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
        if(Mathf.Abs(transform.position.x - patrolDestination.x ) < delta)
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
        if(nextPatrol == PatrolDirection.LEFT)
        {
            patrolDestination = new Vector2(transform.position.x - 5f, transform.position.y);
        }
        else
        {
            patrolDestination = new Vector2(transform.position.x + 5f, transform.position.y);
        }
        readyToPatrol = true;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 pos = transform.position;
        pos.y += 2.25f;
        Gizmos.DrawWireCube(pos, Vector3.one * 5 + Vector3.right *15);

    }
}
