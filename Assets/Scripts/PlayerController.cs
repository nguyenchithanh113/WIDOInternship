using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Moving();
        Jumping();
    }
    void Moving()
    {
        if (Input.GetKey(KeyCode.A))
        {
            rb.velocity = new Vector2(-9f, rb.velocity.y);
        }
        if (Input.GetKey(KeyCode.D))
        {
            rb.velocity = new Vector2(9f, rb.velocity.y);
        }
    }
    void Jumping()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector2.up * 20,ForceMode2D.Impulse);
        }
    }
}
