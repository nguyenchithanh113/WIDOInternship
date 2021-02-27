using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class BlackRollController : MonoBehaviour
{
    public enum Direction
    {
        LEFT,
        RIGHT
    }

    public SkeletonAnimation skeletonAnimation;
    public AnimationReferenceAsset idle;

    string currentAnimation;

    float step = 9;
    float speed = 0.03f;

    bool canRoll = true;
    Vector2 _leftOffset = new Vector2(-2.5f*0.2f, -2.5f*0.2f);
    Vector2 _rightOffset = new Vector2(2.5f*0.2f, -2.5f*0.2f);
    Vector2 leftOffset;
    Vector2 rightOffset;
    BoxCollider2D boxCollider;

    Direction currentDirection;

    public bool collideToBoundary;

    public Transform PivotPoint;
    void Start()
    {
        SetState("Idle");
        
        rightOffset = (Vector2)transform.position + _rightOffset;

        currentDirection = Direction.RIGHT;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            if (canRoll)
            {
                StartCoroutine(Rolling(Direction.LEFT));                            
            }
        }
        if (Input.GetKey(KeyCode.D))
        {
            if (canRoll)
            {
                StartCoroutine(Rolling(Direction.RIGHT));
            }
        }
        
    }
    private void FixedUpdate()
    {
        if (canRoll)
        {
            StartCoroutine(Rolling(currentDirection));
            Debug.Log(currentDirection);
        }
    }
    void SetAnimation(AnimationReferenceAsset animation, bool loop, float timescale)
    {
        if(currentAnimation != animation.name)
        {
            skeletonAnimation.state.SetAnimation(0, animation, loop).TimeScale = timescale;
            currentAnimation = animation.name;
        }
    }
    void SetState(string state)
    {
        if(state == "Idle")
        {
            SetAnimation(idle, true, 1f);
        }
    }
    IEnumerator Rolling(Direction direction)
    {
        float angle = 0;
        canRoll = false;
        if(direction == Direction.LEFT)
        {
            leftOffset = (Vector2)PivotPoint.position + _leftOffset;
            for (int i = 0; i < (90 / step); i++)
            {
                if (collideToBoundary)
                {
                    currentDirection = Direction.RIGHT;
                    break;
                }
                transform.RotateAround(leftOffset, Vector3.forward, step);
                angle += step;              
                yield return new WaitForSeconds(speed);
            }
            if (collideToBoundary)
            {
                //angle = 180 - angle;
                for (int i = 0; i < (angle / step); i++)
                {
                    transform.RotateAround(leftOffset, Vector3.forward, -step);
                    yield return new WaitForSeconds(speed);
                }
                collideToBoundary = false;
            }
        }else if(direction == Direction.RIGHT)
        {
            rightOffset = (Vector2)PivotPoint.position + _rightOffset;
            for (int i = 0; i < (90 / step); i++)
            {
                if (collideToBoundary)
                {
                    currentDirection = Direction.LEFT;
                    break;
                }
                transform.RotateAround(rightOffset, Vector3.forward, -step);
                angle += step;
                yield return new WaitForSeconds(speed);
            }
            if (collideToBoundary)
            {
                for (int i = 0; i < (angle / step); i++)
                {
                    transform.RotateAround(rightOffset, Vector3.forward, step);
                    yield return new WaitForSeconds(speed);
                }
                collideToBoundary = false;
            }
        }    
        canRoll = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(leftOffset, Vector2.one*0.1f);
        Gizmos.DrawCube(rightOffset, Vector2.one*0.1f);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Boundary"))
        {
            collideToBoundary = true;
        }
    }
}
