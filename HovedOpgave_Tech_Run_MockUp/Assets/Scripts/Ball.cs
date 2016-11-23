using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour
{

    #region Fields
    private bool isMoving;
    private Vector3 startPos;
    protected FlingSetting flingSetting;
    protected float uniqueSpeed;
    private Vector3 startBallPos;
    private GameObject parent;
    #endregion

    // Use this for initialization
	protected virtual void Start () 
    {
        //gameObject.GetComponent<Rigidbody>().useGravity = false;

        //this.transform.position = new Vector3(Camera.main.transform.position.x + 1, Camera.main.transform.position.y - 1, Camera.main.transform.position.z + 2);

        //startBallPos = this.transform.localPosition;
        //parent = gameObject.transform.parent.gameObject;
	}
	
	// Update is called once per frame
	protected virtual void Update () {
	
	}

    protected virtual void SwipeControl()
    {

    }

    //private IEnumerator ResetPosition()
    //{
    //    yield return new WaitForSeconds(4.0f);
    //    gameObject.GetComponent<Rigidbody>().useGravity = false;
    //    gameObject.transform.parent = parent.transform;
    //    //transform.rotation = new Quaternion(0, 0, 0, 0);
    //    transform.localPosition = startBallPos;
    //    gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    //}

    private void OnTriggerEnter(Collider _collider)
    {

    }

    //float factor = 100.0f;

    //private float startTime;

    //void OnMouseDown()
    //{
    //    startTime = Time.time;
    //    startPos = Input.mousePosition;
    //    startPos.z = transform.position.z - Camera.main.transform.position.z;
    //    startPos = Camera.main.ScreenToWorldPoint(startPos);
    //}

    //void OnMouseUp()
    //{
    //    Vector3 endPos = Input.mousePosition;
    //    endPos.z = transform.position.z - Camera.main.transform.position.z;
    //    endPos = Camera.main.ScreenToWorldPoint(endPos);

    //    Vector3 force = endPos - startPos;
    //    force.z = force.magnitude;
    //    force /= (Time.time - startTime);

    //    gameObject.GetComponent<Rigidbody>().useGravity = true;

    //    gameObject.transform.parent = null;

    //    gameObject.GetComponent<Rigidbody>().AddForce(force * factor);
    //    StartCoroutine(ResetPosition());
    //}
}
