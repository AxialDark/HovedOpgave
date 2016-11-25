using UnityEngine;
using System.Collections;

/// <summary>
/// This is a Script made following a online tutorial; link: https://www.youtube.com/watch?v=wavvtztVK3c&index=11&list=FLHFTwh0AzY5XKPTt5P6dUWw
/// It is not used in the project, but the code is used in another script, which is used in the project.
/// </summary>
public class BallTempScript : MonoBehaviour {

    [SerializeField]
    private float throwSpeed;
    private float speed;
    private float lastMouseX, lastMouseY;

    private bool thrown, holding;

    private Rigidbody rigidBody;
    private Vector3 newPosition;

	// Use this for initialization
	void Start () 
    {
        rigidBody = GetComponent<Rigidbody>();
        Reset();
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (holding)
        {
            OnTouch();
        }

        if (thrown)
        {
            return;
        }

        //if(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        //    RaycastHit hit;

        //    if (Physics.Raycast(ray, out hit, 100f))
        //    {
        //        if (hit.transform == transform)
        //        {
        //            holding = true;
        //            transform.SetParent(null);
        //        }
        //    }
        //}

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                if (hit.transform == transform)
                {
                    holding = true;
                    transform.SetParent(null);
                }
            }
        }

        //if(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)
        //{
        //    if (lastMouseY < Input.GetTouch(0).position.y)
        //    {
        //        Throwball(Input.GetTouch(0).position);
        //    }
        //}

        if (Input.GetMouseButtonUp(0))
        {
            if (lastMouseY < Input.mousePosition.y)
            {
                Throwball(Input.mousePosition);
            }
        }

        //if(Input.touchCount == 1)
        //{
        //    lastMouseX = Input.GetTouch(0).position.x;
        //    lastMouseY = Input.GetTouch(0).position.y;
        //}

        if (Input.GetMouseButton(0))
        {
            lastMouseX = Input.mousePosition.x;
            lastMouseY = Input.mousePosition.y;
        }
	}

    private void OnTouch()
    {
        //Vector3 mousePosition = Input.GetTouch(0).position;
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.nearClipPlane * 7.5f;

        newPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        transform.localPosition = Vector3.Lerp(transform.localPosition, newPosition, 50f * Time.deltaTime);
    }

    private void Throwball(Vector3 _mousePos)
    {
        rigidBody.useGravity = true;

        float differenceY = (_mousePos.y - lastMouseY) / Screen.height * 100;
        speed = throwSpeed * differenceY;

        float x = (_mousePos.x / Screen.width) - (lastMouseX / Screen.width);
        //float x = (_mousePos.x) - (lastMouseX);
        //x = Mathf.Abs(Input.GetTouch(0).position.x - lastMouseX) / Screen.width * 100 * x;
        x = Mathf.Abs(Input.mousePosition.x - lastMouseX) / Screen.width * 100 * x;
        //x = (Input.mousePosition.x - lastMouseX)/*/ Screen.width * 100 * x*/;

        Vector3 direction = new Vector3(x, 0f, 1f);
        direction = Camera.main.transform.TransformDirection(direction);

        rigidBody.AddForce((direction * speed / 2f) + (Vector3.up * speed));

        holding = false;
        thrown = true;

        Invoke("Reset", 3.0f);
    }

    private void Reset()
    {
        CancelInvoke();

        transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.1f, Camera.main.nearClipPlane * 7.5f));
        newPosition = transform.position;

        thrown = holding = false;

        rigidBody.useGravity = false;
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.Euler(0f, 200f, 0f);
        transform.SetParent(Camera.main.transform);
    }

    private void OnTriggerEnter(Collider _collider)
    {
        if (_collider.gameObject.tag == "Goal")
        {
            GameManager.Instance.CalculateGoalPoints();
            Invoke("Reset", 0.0f);
        }

        print(_collider.gameObject.name);
    }
}
