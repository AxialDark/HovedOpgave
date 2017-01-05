using UnityEngine;
using System.Collections;

/// <summary>
/// For this class to work, it as needed to comment out the movement of the user in the User class Update method.
/// </summary>
public class PhoneDebugMovement : MonoBehaviour
{

    float speed = 200;

    public void ClickMoveRight()
    {
        User.Instance.transform.position += Vector3.right * Time.deltaTime * speed;
    }
    public void ClickMoveLeft()
    {
        User.Instance.transform.position += Vector3.left * Time.deltaTime * speed;
    }
    public void ClickMoveUp()
    {
        User.Instance.transform.position += Vector3.forward * Time.deltaTime * speed;
    }
    public void ClickMoveDown()
    {
        User.Instance.transform.position += Vector3.back * Time.deltaTime * speed;
    }
}
