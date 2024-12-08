using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public float maxTurnAngle = 30f;

    private Vector3 localWheelRotation = new Vector3(0f, 0f, 0f);

    private void Update() {
        TurnWheel();
        transform.localRotation = Quaternion.Euler(localWheelRotation);
    }

    void TurnWheel(){
        float horizontalInput = Input.GetAxis("Horizontal");
        localWheelRotation.z = -horizontalInput * maxTurnAngle;
    }
}
