using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instacne;
    public Camera Camera;

    private void Awake()
    {
        if (Instacne == null)
        {
            Instacne = this;
        }

        Camera = GetComponent<Camera>();
    }
}
