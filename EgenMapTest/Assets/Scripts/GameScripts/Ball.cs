using UnityEngine;
using System.Collections;

/// <summary>
/// The code in this script is made based on this: https://www.youtube.com/watch?v=wavvtztVK3c&index=11&list=FLHFTwh0AzY5XKPTt5P6dUWw
/// </summary>
public class Ball : MonoBehaviour
{
    #region Fields
    [SerializeField]
    private float throwSpeed;

    private float speed;
    private float lastMouseX, lastMouseY;

    private bool thrown, holding;

    private Rigidbody rigidBody;
    private Vector3 newPosition;
    #endregion

    /// <summary>
    /// Unity Method, called in the beginning, after Awake().
    /// </summary>
    protected virtual void Start () 
    {
        rigidBody = GetComponent<Rigidbody>();
        Reset(); // calls the Reset() method here so that the ball is at the right position at the beginning of the game.
    }

    /// <summary>
    /// Unity Method, Update is called once per frame.
    /// </summary>
    protected virtual void Update () {

        if (holding) //Is the ball currently being held by the player?
        {
            OnTouch();
        }

        if (thrown) //Is the ball currently being thrown?
        {
            return;
        }

        SwipeControl();
    }

    /// <summary>
    /// Handles what happens when the player is touching the ball.
    /// </summary>
    private void OnTouch()
    {
        Vector3 mousePosition = Input.mousePosition; // Saves the mouses/fingers current position.
        mousePosition.z = Camera.main.nearClipPlane * 7.5f; // Makes the mouse/finger apear to be a little bit away from the camera.

        newPosition = Camera.main.ScreenToWorldPoint(mousePosition); // Assigns the position which the ball should follow, to the worldcoordinate equivalent of the mouse/finger position on the screen.

        transform.localPosition = Vector3.Lerp(transform.localPosition, newPosition, 50f * Time.deltaTime); //Lerp makes it so the ball follows the finger/mouse in a smooth motion.
    }

    /// <summary>
    /// Handles what happens as the mouse/finger is dragged across the screen.
    /// </summary>
    private void SwipeControl()
    {
        if (Input.GetMouseButtonDown(0)) // Have the "mouse just clicked / finger just touched" the screen.
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f)) // Does the raycast hit anything?
            {
                if (hit.transform == transform) // Is the object hit by the raycast, this object?
                {
                    holding = true; // The player is now holding the ball.
                    transform.SetParent(null); // The ball is no longer locked to the Camera.
                }
            }
        }

        if (Input.GetMouseButtonUp(0)) // When the mouse/finger is released from the screen.
        {
            if (lastMouseY < Input.mousePosition.y) // If the finger/mouse moved up on the screen, as it released. This is so you can't throw down.
            {
                Throwball(Input.mousePosition); //Ball is thrown.
            }
        }

        if (Input.GetMouseButton(0)) // As long as the "mouse is clicked / finger is touching the screen".
        {
            lastMouseX = Input.mousePosition.x; // Used in calculating the direction and speed of the throw.
            lastMouseY = Input.mousePosition.y; // Ditto.
        }
    }

    /// <summary>
    /// Handles what happens when the ball is thrown.
    /// </summary>
    /// <param name="_mousePos">The mouse current position as this method is called.</param>
    protected virtual void Throwball(Vector3 _mousePos)
    {
        rigidBody.useGravity = true; // The ball is now affected by gravity.

        float differenceY = (_mousePos.y - lastMouseY) / Screen.height * 100;
        speed = throwSpeed * differenceY;

        float x = (_mousePos.x / Screen.width) - (lastMouseX / Screen.width);
        x = Mathf.Abs(Input.mousePosition.x - lastMouseX) / Screen.width * 100 * x;

        Vector3 direction = new Vector3(x, 0f, 1f);
        direction = Camera.main.transform.TransformDirection(direction);

        rigidBody.AddForce((direction * speed / 2f) + (Vector3.up * speed)); // Adds force in a direction to the ball.

        holding = false;
        thrown = true;

        //Invoke("Reset", 3.0f); // calls the Reset() method after 3 seconds.
    }

    /// <summary>
    /// Resets the ball to its start values.
    /// </summary>
    private void Reset()
    {
        //CancelInvoke();

        transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.1f, Camera.main.nearClipPlane * 7.5f));
        newPosition = transform.position;

        thrown = holding = false;

        rigidBody.useGravity = false;
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.Euler(0f, 200f, 0f);
        transform.SetParent(Camera.main.transform);

        GameManager.Instance.ThrowDeduct();
    }

    /// <summary>
    /// Handles what happens when the ball collides with a trigger object.
    /// </summary>
    /// <param name="_collider">The colliding object</param>
    private void OnTriggerEnter(Collider _collider)
    {
        if (_collider.gameObject.tag == "Goal") // If the colliding object is the goal.
        {
            GameManager.Instance.CalculateGoalPoints();
            GameManager.Instance.AddConsecutiveGoal();
            Reset();
        }
        else if (_collider.gameObject.tag == "Floor") // If the colliding object is the floorplane.
        {
            GameManager.Instance.ResetConsecutiveGoals();
            Reset();
        }
    }
}
