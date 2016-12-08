using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Enumeration to test different throw heights
/// </summary>
public enum BallUpSpeedTest { EQUAL, LIMIT_400, PROPERTIONAL_TO_LIMIT_400, SIXTY_DEGREES}


/// <summary>
/// The code in this script is made based on this: https://www.youtube.com/watch?v=wavvtztVK3c&index=11&list=FLHFTwh0AzY5XKPTt5P6dUWw
/// </summary>
public class Ball : MonoBehaviour
{

    [SerializeField]
    private BallUpSpeedTest test;
    private const float LIMIT = 400f;

    private void SpeedTest(Vector3 _direction)
    {
        print("Holdtime: " + holdTime);

        if (test == BallUpSpeedTest.LIMIT_400)
        {
            float upSpeed = (speed >= LIMIT) ? LIMIT : speed;

            print("Speed: " + speed + "\n" + "Up Speed: " + upSpeed);

            rigidBody.AddForce((_direction * speed / 2f) + (Vector3.up * upSpeed)); // Adds force in a direction to the ball.
        }
        else if (test == BallUpSpeedTest.EQUAL)
        {
            rigidBody.AddForce((_direction * speed / 2f) + (Vector3.up * speed)); // Adds force in a direction to the ball.

            print("Speed: " + speed + "\n" + "Up Speed: " + speed);
        }
        else if (test == BallUpSpeedTest.PROPERTIONAL_TO_LIMIT_400)
        {

            float diffFromLimit = (speed > LIMIT) ? speed - LIMIT : -1;
            float upSpeed;

            if (diffFromLimit == -1)
                upSpeed = speed;
            else
            {
                upSpeed = (((diffFromLimit / 10f) / 100f) + 1) * LIMIT;
            }

            print("Speed: " + speed + "\n" + "Up Speed: " + upSpeed);

            rigidBody.AddForce((_direction * speed / 2f) + (Vector3.up * upSpeed)); // Adds force in a direction to the ball.

        }
        else if (test == BallUpSpeedTest.SIXTY_DEGREES)
        {
            speed = (speed > 1300 ? 1300 : speed);

            float speedX = speed * 0.6f;
            float speedY = speed * 0.4f;

            print("Speed: " + speed + "\nSpeed X: " + speedX + "\n" + "Speed Y: " + speedY);

            rigidBody.AddForce((_direction * speedX / 2f) + (Vector3.up * speedY)); // Adds force in a direction to the ball.


        }
    }

    public void TestOnValue(int newVal)
    {
        test = (BallUpSpeedTest)newVal;
    }

    private Vector2 startHoldPos;
    private Vector2 lastCheckStillPos;
    private Vector2 lastStillPos;
    private float mouseStillTime = 0;
    private float delay = 0;
    private float holdTime = 0;

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
                    startHoldPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    lastStillPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    holdTime = 0;

                    holding = true; // The player is now holding the ball.
                    transform.SetParent(null); // The ball is no longer locked to the Camera.
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && holding) // When the mouse/finger is released from the screen.
        {
            if (lastMouseY < Input.mousePosition.y) // If the finger/mouse moved up on the screen, as it released. This is so you can't throw down.
            {
                Throwball(Input.mousePosition); //Ball is thrown.

                startHoldPos = Vector2.zero;
            }
        }

        if (Input.GetMouseButton(0) && holding) // As long as the "mouse is clicked / finger is touching the screen".
        {
            if (lastMouseX == Input.mousePosition.x && lastMouseY == Input.mousePosition.y && delay > 0.5f)
            {
                print("In place");
                delay = 0;

                lastCheckStillPos = new Vector2(lastMouseX, lastMouseY);
            }

            lastMouseX = Input.mousePosition.x; // Used in calculating the direction and speed of the throw.
            lastMouseY = Input.mousePosition.y; // Ditto.

            delay += Time.deltaTime;
            holdTime += Time.deltaTime;

            if (new Vector2(lastMouseX, lastMouseY) == lastCheckStillPos)
            {
                print("It checks out");
                mouseStillTime += Time.deltaTime;
            }

            if (mouseStillTime > 0.5f)
            {
                print("New start pos");
                startHoldPos = lastCheckStillPos;
                lastStillPos = lastCheckStillPos;
                lastCheckStillPos = Vector2.zero;
                mouseStillTime = 0;
                holdTime = 0;

                print("StartPos: " + startHoldPos + " - " + "lastStillPos: " + lastStillPos);
            }
        }
    }

    /// <summary>
    /// Handles what happens when the ball is thrown.
    /// </summary>
    /// <param name="_mousePos">The mouse current position as this method is called.</param>
    protected virtual void Throwball(Vector3 _mousePos)
    {
        #region old
        //rigidBody.useGravity = true; // The ball is now affected by gravity.

        //float differenceY = (_mousePos.y - lastMouseY) / Screen.height * 100;
        //speed = throwSpeed * differenceY;

        //float x = (_mousePos.x / Screen.width) - (lastMouseX  / Screen.width);
        //x = Mathf.Abs(Input.mousePosition.x - lastMouseX) / Screen.width * 100 * x;

        //Vector3 direction = new Vector3(x, 0f, 1f);
        //direction = Camera.main.transform.TransformDirection(direction);

        ////Debug
        //SpeedTest(direction);
        ////rigidBody.AddForce((direction * speed / 2f) + (Vector3.up * upSpeed)); // Adds force in a direction to the ball.

        //holding = false;
        //thrown = true;

        ////Invoke("Reset", 3.0f); // calls the Reset() method after 3 seconds.
        #endregion

        rigidBody.useGravity = true; // The ball is now affected by gravity.

        float differenceY = (_mousePos.y - startHoldPos.y) / Screen.height * 100;
        //speed = throwSpeed * differenceY;
        speed = throwSpeed * differenceY * (1 + (1 - (holdTime > 1 ? 1 : holdTime)));

        float x = (_mousePos.x / Screen.width) - (startHoldPos.x / Screen.width);
        x = Mathf.Abs(Input.mousePosition.x - startHoldPos.x) / Screen.width * 100 * x;

        Vector3 direction = new Vector3(x, 0f, 1f);
        direction = Camera.main.transform.TransformDirection(direction);

        //Debug
        SpeedTest(direction);
        //rigidBody.AddForce((direction * speed / 2f) + (Vector3.up * upSpeed)); // Adds force in a direction to the ball.

        holding = false;
        thrown = true;
    }

    /// <summary>
    /// Resets the ball to its start values.
    /// </summary>
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
            Invoke("Reset", 0.5f);
        }
        else if (_collider.gameObject.tag == "Floor") // If the colliding object is the floorplane.
        {
            GameManager.Instance.ResetConsecutiveGoals();
            Reset();
        }
    }
}
