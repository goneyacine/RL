using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class Car : MonoBehaviour
{
    public float speed = 10;
    public float rotationRate = 10;
    public Transform forward;
    public Transform cam;
    private Rigidbody2D rb;
    private Vector2[] hitPoints = new Vector2[12];
    private List<Frame> data = new List<Frame>();
    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        if (!File.Exists("C:\\Users\\TMAX27\\Desktop\\traindata.txt"))
        {
            string json = JsonConvert.SerializeObject(data);
            File.WriteAllText("C:\\Users\\TMAX27\\Desktop\\traindata.txt", json);
        }
    }
    void FixedUpdate()
    {
        Frame frame = new Frame();
        cam.position = new Vector3 (transform.position.x, transform.position.y, cam.position.z);
        Vector2 forwardDirection = (forward.position - transform.position) / Vector2.Distance(forward.position, transform.position);
        rb.velocity = forwardDirection * speed;
        float[] position = { transform.position.x, transform.position.y };
        frame.position = position;
        frame.angle = transform.eulerAngles.z;
        float[,] hitPointsData = new float[12,2];
        for(int i = 0; i < hitPoints.Length; i++)
        {
            hitPoints[i] = Physics2D.Raycast(transform.position, new Vector2(Mathf.Cos(3.14f / 6 * i), Mathf.Sin(3.14f / 6 * i))).point;
            hitPointsData[i,0] = hitPoints[i].x;
            hitPointsData[i,1] = hitPoints[i].y;
            Debug.DrawRay(transform.position, new Vector2(Mathf.Cos(3.14f / 6 * i), Mathf.Sin(3.14f / 6 * i)) * Vector2.Distance(transform.position, hitPoints[i]), Color.red);
        }
        frame.hitPoints = hitPointsData;
        frame.action = 0;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Right();
            frame.action = 1;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            Left();
            frame.action = -1;
        }
        data.Add(frame);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.tag == "Obsticall")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            string existingJson = File.ReadAllText("C:\\Users\\TMAX27\\Desktop\\traindata.txt");
            List<Frame> existingData = new List<Frame>();
            try
            {
                 existingData = JsonConvert.DeserializeObject<List<Frame>>("C:\\Users\\TMAX27\\Desktop\\traindata.txt");
            }catch(Newtonsoft.Json.JsonReaderException e)
            { 
                
            }
            string json = JsonConvert.SerializeObject(existingData.Concat(data).ToList());
            File.WriteAllText("C:\\Users\\TMAX27\\Desktop\\traindata.txt", json);
        }

    }
    public void Right()
    {
        transform.eulerAngles += Vector3.forward * rotationRate * Time.fixedDeltaTime;
    }
    public void Left()
    {
        transform.eulerAngles -= Vector3.forward * rotationRate * Time.fixedDeltaTime;
    }

}

public class Frame
{
    //state features
    public float[] position { get; set; }
    public float[,] hitPoints { get; set; }
    public float angle { get; set; }
    //-1 for left, 0 for nothing, 1 for right
    public int action { get; set; }
}

