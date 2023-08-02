using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody2D rb;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(-1,0.5f);
    }

    // Update is called once per frame

    public void OnCollisionEnter2D(Collision2D collision)
    {
        rb.velocity = -rb.velocity;
        Debug.Log(rb.velocity);
    }
}
