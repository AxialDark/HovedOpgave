using UnityEngine;
using System.Collections;

public class DebugMovement : MonoBehaviour
{

    [SerializeField]
    private float speed = 200;


    // Update is called once per frame
    void Update()
    {
        if (User.Instance.RouteIsActive)
            Move();
    }


    private void Move()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += Vector3.right * Time.deltaTime * speed;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position += Vector3.left * Time.deltaTime * speed;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += Vector3.forward * Time.deltaTime * speed;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position += Vector3.back * Time.deltaTime * speed;
        }
    }
}
