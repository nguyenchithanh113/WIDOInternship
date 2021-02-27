using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RescueBoxV2 : MonoBehaviour
{
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(800f * Vector2.up);
            animator.Play("rescue_box_breaking");
        }
    }
}
