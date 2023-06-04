using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotPlayerMoveController : MonoBehaviour
{
    [SerializeField] WheelJointController[] wheelCtrl;

    public void WheelControllerSetup()
    {
        wheelCtrl = this.transform.GetComponentsInChildren<WheelJointController>();
    }

    public void MoveLeft()
    {
        foreach(WheelJointController c in wheelCtrl)
        {
            c.h = -1f;
        }
    }

    public void MoveRight()
    {
        foreach (WheelJointController c in wheelCtrl)
        {
            c.h = 1f;
        }
    }

    public void MoveStop()
    {
        foreach (WheelJointController c in wheelCtrl)
        {
            c.h = 0f;
        }
    }
}
