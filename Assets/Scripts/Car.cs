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
       if (!File.Exists("C:\\Users\\TMAX27\\Desktop\\traindata.json"))
       {
           string json = JsonConvert.SerializeObject(data);
           File.WriteAllText("C:\\Users\\TMAX27\\Desktop\\traindata.json", json);
       }
    }
    void FixedUpdate()
    {
          Frame frame = new Frame();
        cam.position = new Vector3(transform.position.x, transform.position.y, cam.position.z);
        Vector2 forwardDirection = (forward.position - transform.position) / Vector2.Distance(forward.position, transform.position);
        rb.velocity = forwardDirection * speed;
        float[] position = { transform.position.x, transform.position.y };
         frame.position = position;
         frame.angle = transform.eulerAngles.z;
       float[,] hitPointsData = new float[12, 2];
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


        float[] slopes = new float[] {
    8.89215611e+03f, 1.84295013e+03f, 8.53961711e-04f, 3.55646525e-03f,
    -6.40890912e+01f, 6.61378113e+02f, -1.14625326e+03f, -9.52341879e+02f,
    5.50510392e+02f, -1.62963711e+02f, 1.38555305e-01f, -6.87682074e+02f,
    -3.96057243e+02f, 2.33905862e+02f, 4.03904965e+02f, -5.27636043e-01f,
    -3.23939741e+02f, -3.86561619e+02f, 6.72427727e+02f, 8.75795442e+02f,
    -5.08120606e+02f, -6.51388783e+03f, 1.55525833e+01f, -2.02663177e+03f,
    -1.16290592e+03f, 6.73577558e+01f, 1.15881188e+02f
};
          float inter = -0.13994918679163945f;

         float action = transform.position.x * slopes[0] + transform.position.y * slopes[1] + transform.eulerAngles.z * slopes[2] + inter;

        for (int i = 0; i < hitPoints.Length; i++)
        {
            hitPoints[i] = Physics2D.Raycast(transform.position, new Vector2(Mathf.Cos(3.14f / 6 * i), Mathf.Sin(3.14f / 6 * i))).point;
            action += hitPoints[i].x * slopes[3 + (i * 2)] + hitPoints[i].y * slopes[3 + (i*2) + 1];
          hitPointsData[i, 0] = hitPoints[i].x;
         hitPointsData[i, 1] = hitPoints[i].y;
            Debug.DrawRay(transform.position, new Vector2(Mathf.Cos(3.14f / 6 * i), Mathf.Sin(3.14f / 6 * i)) * Vector2.Distance(transform.position, hitPoints[i]), Color.red);
        }

        Debug.Log(action);
        if (action >= 0.3f)
            Right();
        else if (action <= -0.3f)
            Left();
         //data.Add(frame);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.tag == "Obsticall")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
           // if (data.Count > 1000)
           // {
           //
           //     for (int i = 0; i < 200; i++)
           //     {
           //         data.Remove(data[data.Count - i -1]);
           //     }
           //     string existingJson = File.ReadAllText("C:\\Users\\TMAX27\\Desktop\\traindata.json");
           //     List<Frame> existingData = JsonConvert.DeserializeObject<List<Frame>>(existingJson);
           //     string json = JsonConvert.SerializeObject(existingData.Concat(data).ToList());
           //     File.WriteAllText("C:\\Users\\TMAX27\\Desktop\\traindata.json", json);
           //     Debug.Log(existingData.Count);
           // }
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

