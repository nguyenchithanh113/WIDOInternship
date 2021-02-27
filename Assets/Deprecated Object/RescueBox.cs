using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RescueBox : MonoBehaviour
{

    public GameObject BreakBox;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject breakbox = Instantiate(BreakBox, transform.position, Quaternion.identity);
            breakbox.GetComponent<BoxBreak>().Breaking(collision.transform.position);
            Destroy(gameObject);
        }
    }
}
