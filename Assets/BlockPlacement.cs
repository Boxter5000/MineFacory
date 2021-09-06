using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPlacement : MonoBehaviour
{
    public new Transform camera;
    private float buildDistance;

    private Vector3 GetVewdirection()
    {
        return camera.forward;
    }
}
