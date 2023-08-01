using UnityEngine;
using UnityEngine.SceneManagement;
public class AgentController : MonoBehaviour
{
    public Transform stick;
    public float movingForce = 10;

    private Rigidbody2D rb; 
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

  
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.D))
            Right();
        else if (Input.GetKey(KeyCode.A))
            Left();

       // if (!(stick.eulerAngles.z < 60 && stick.eulerAngles.z > -60))
          //  SceneManager.LoadScene(SceneManager.GetActiveScene().name);


    }

    private void Right()
    {
        rb.AddForce(Vector2.right * movingForce);
    }
    private void Left()
    {
        rb.AddForce(Vector2.right * -movingForce);

    }
}
