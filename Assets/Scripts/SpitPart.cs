using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpitPart : MonoBehaviour
{
    public Rigidbody2D rb;
    public SpitBomb spitBomb;
    void Awake()
    {
        
    }
    private void OnEnable()
    {
        spitBomb.OnFlying += OnFlying;
    }
    private void OnDisable()
    {
        spitBomb.OnFlying -= OnFlying;
    }
    void OnFlying(Vector2 force)
    {
        rb.AddForce(force);
    }
}
