using UnityEngine;
using System.Collections;

public class FlingSetting
{

    #region Fields
    private Vector3 flingDirection;
    private float speed;
    #endregion

    #region Proberties
    public Vector3 FlingDirection
    {
        get { return flingDirection; }
    }

    public float Speed
    {
        get { return speed; }
    }
    #endregion

    public FlingSetting(Vector3 _flingDirection, float _speed)
    {
        flingDirection = _flingDirection;
        _speed = speed;
    }
}
