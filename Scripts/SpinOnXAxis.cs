using UnityEngine;

public class SpinOnXAxis : MonoBehaviour
{
    public float speed;
    void Start()
    {
        
    }

    void Update()
    {
        transform.Rotate(speed * Time.deltaTime * Vector3.right);
    }
}
