using UnityEngine;
using UnityEngine.SceneManagement;

public class Car : MonoBehaviour
{
    public float speed = 10;
    public Transform forward;
    private Rigidbody2D rb;
    private Vector2[] hitPoints = new Vector2[12];
    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        Vector2 forwardDirection = (forward.position - transform.position) / Vector2.Distance(forward.position, transform.position);
        rb.velocity = forwardDirection * speed;

        for(int i = 0; i < hitPoints.Length; i++)
        {
            hitPoints[i] = Physics2D.Raycast(transform.position, new Vector2(Mathf.Cos(3.14f / 6 * i), Mathf.Sin(3.14f / 6 * i))).point;
            Debug.DrawRay(transform.position, new Vector2(Mathf.Cos(3.14f / 6 * i), Mathf.Sin(3.14f / 6 * i)) * Vector2.Distance(transform.position, hitPoints[i]), Color.red);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.gameObject.tag == "Obsticall")
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);


    }
}
