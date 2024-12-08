using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public Rigidbody carRb;
    public float wheelRadius = 0.8f;
    public float maxTurnAngle = 30f;
    public WheelType wheelType;

    private Vector3 localWheelRotation = new Vector3(0f, 0f, 0f);
    
    public void AssignValues(Rigidbody carRb, float wheelRadius, float maxTurnAngle){
        this.carRb = carRb;
        this.wheelRadius = wheelRadius;
        this.maxTurnAngle = maxTurnAngle;
    }
    private void Update() {
        Move();
    }

    void TurnWheel(){
        float horizontalInput = Input.GetAxis("Horizontal");
        localWheelRotation.y = horizontalInput * maxTurnAngle;
    }

    public void Move(){
        if(wheelType == WheelType.frontWheel) TurnWheel();
        transform.localRotation = Quaternion.Euler(localWheelRotation);
    }
}

public enum WheelType{
    frontWheel,
    backWheel
}
