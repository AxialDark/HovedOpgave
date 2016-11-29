using UnityEngine;
using System.Collections;

/// <summary>
/// Script for the basketball, is a subclass of the Ball class.
/// </summary>
public class BasketBall : Ball {

	// Use this for initialization
	protected override void Start () 
    {
        base.Start(); //Calls the superclass version of the Start() method.
	}
	
	// Update is called once per frame
	protected override void Update () 
    {
        base.Update();
	}
}
