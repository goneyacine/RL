using System.Collections.Generic;
using System.Transactions;
using UnityEngine;
using Random = UnityEngine.Random;

//Implementation of REINFORCE policy gradient algorithm
public class RInforcePG : MonoBehaviour
{
    public int max_steps = 100;
    public float startTime;
    public float speed = 10;
    public float rotationRate = 10;
    public Transform forward;
    public float learningFactor = 0.001f;
    public Transform startPosition;
    Rigidbody2D rb;
    public double[,] weights;
    const int num_of_acitons = 3;
    const int num_of_state_features = 15;
    List<Step> steps = new List<Step>();
    bool isCollided = false;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        //randonmly initialize the wieghts
        weights = new double[num_of_acitons, num_of_state_features];
        for (int i = 0; i < num_of_acitons; i++)
            for (int j = 0; j < num_of_state_features; j++)
                weights[i, j] = Random.Range(-1f, 1f);
        //set start time
        startTime = Time.time;
    }

    private void FixedUpdate()
    {

        Vector2 forwardDirection = (forward.position - transform.position) / Vector2.Distance(forward.position, transform.position);
        rb.velocity = forwardDirection * speed;

        if (steps.Count + 1 <= max_steps)
        {
            Step step = new Step();
            step.state = new double[num_of_state_features];
            step.state[0] = transform.position.x;
            step.state[1] = transform.position.y;
            step.state[2] = transform.eulerAngles.z / 360;
            //find the hit points
            for (int i = 0; i < 12; i++)
            {
                Vector2 hitPoint = Physics2D.Raycast(transform.position, new Vector2(Mathf.Cos(3.14f / 6 * i), Mathf.Sin(3.14f / 6 * i))).point;
                step.state[i + 3] = Vector2.Distance(transform.position, hitPoint) / 5;
                Debug.DrawRay(transform.position, new Vector2(Mathf.Cos(3.14f / 6 * i), Mathf.Sin(3.14f / 6 * i)) * Vector2.Distance(transform.position, hitPoint), Color.red);
            }
             step.action = TakeAction(step.state);
             step.reward = GetReward(step.state,Time.time - startTime);
             steps.Add(step);
        }
        else
        {
            UpdateWieghts();
            steps = new List<Step>();
            if (isCollided)
            {
                startTime = Time.time;
                transform.position = startPosition.position;
                transform.eulerAngles = Vector3.zero;
            }
        }
    }

    float GetReward(double[] state,float time)
    {
        float reward = time;
        for (int i = 3; i < 15; i++)
        {
            if (state[i] <= 0.1d)
                reward -= 4;
            else if (state[i] <= 0.3d && state[i] > 0.1d)
                reward -= 2f;
            else if (0.3d <= state[i] && state[i] < 0.5f)
                reward -= 0.1f;
 
        }

        return time;
    }
    int TakeAction(double[] state)
    {
        double right = Softmax(weights, 0,state);
        double left = Softmax(weights, 1,state);
        double none = Softmax(weights, 2,state);
        Debug.Log(right + " " + left + " " + none);
        if(right > none && right > left)
        {
            Right();
            return 0;
        }else if (left > none && left > right)
        {
            Left();
            return 1;
        }else 
        return 2;
    }
    void UpdateWieghts()
    {
        float[] discountedRewards = new float[steps.Count];
        float cumulativeReward = 0;
        for (int t = steps.Count - 1; t >= 0; t--)
        {
            cumulativeReward = steps[t].reward + 0.99f * cumulativeReward;
            discountedRewards[t] = cumulativeReward;
        }

        // Calculate gradients and update policy weights
        for (int t = 0; t < steps.Count; t++)
        {
            for(int i = 0; i < 3;i++)
            {
                for(int j =0;j < 15;j++)
                {   
                    weights[i, j] +=learningFactor * discountedRewards[t] * gradient(weights, i, steps[t].state)[j];
                }
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        isCollided = true;
    }
    public void OnCollisionExit2D(Collision2D collision)
    {
        isCollided = false;
    }
    public void Right()
    {
        transform.eulerAngles += Vector3.forward * rotationRate * Time.fixedDeltaTime;
    }
    public void Left()
    {
        transform.eulerAngles -= Vector3.forward * rotationRate * Time.fixedDeltaTime;
    }

    public double Softmax(double[,] wieghts, int i, double[] state)
    {
        double a = 0;
        double b = 0;
        double c = 0;
        int l, m;
        if (i == 0)
        {
            l = 1;
            m = 2;
        }
        else if (i == 1)
        {
            l = 0;
            m = 2;
        }
        else
        {
            l = 0;
            m = 1;
        }
        for (int j = 0; j < num_of_state_features; j++)
        {
            a += wieghts[i,j] * state[j];
            b += wieghts[l,j] * state[j];
            c += wieghts[m,j] * state[j];
        }
        float[] abc = { (float)a, (float)b, (float)c };
        float max = Mathf.Max(abc);
        return Mathf.Exp((float)a - max) / (Mathf.Exp((float)a - max) + Mathf.Exp((float)b  - max) + Mathf.Exp((float)c - max));
    }


    public double[] gradient(double[,] wieghts, int i, double[] state)
    {
        double softmax = Softmax(wieghts, i, state);
        double[] result = new double[state.Length];
        for (int j = 0; j < state.Length; j++)
        {
            result[j] = softmax * (1 - softmax) * state[j];
          //  Debug.LogWarning(result[j]);
        }
        return result;
    }
}
public class Step
{
    public double[] state;

    public float reward { get; set; }
    public int action { get; set; }
}

