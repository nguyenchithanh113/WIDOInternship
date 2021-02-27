using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpitBomb : MonoBehaviour
{
    Rigidbody2D rb;
    public float force = 200f;
    public System.Action<Vector2> OnFlying = delegate { };
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void Fly(Vector2 direction)
    {
        //rb.AddForce(direction * force, ForceMode2D.Force);
        OnFlying(direction * force);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
