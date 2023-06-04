using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelJointController : MonoBehaviour
{
    //바퀴 오브젝트의 조작을 담당하는 스크립트
    //좌우 이동 조작 인자를 받아 좌우 이동 및 멈추는 역할을 담당

    [SerializeField] RobotBattleSceneController gameCtrl;
    public Transform robotTr; //부모오브젝트 (로봇의 본체 // 로봇 본체의 위치에 따라 좌우 이동 입력의 동작 여부 결정)
    [SerializeField] WheelJoint2D wJoint;
    [SerializeField] JointMotor2D jMotor;
    [SerializeField] float moveSpeed = 100f;
    [SerializeField] float spd;

    public float h = 0;
    [SerializeField] bool isMove = false; //브레이크 작동 컨트롤

    private void Awake()
    {
        gameCtrl = FindObjectOfType<RobotBattleSceneController>();
        robotTr = null;
        wJoint = GetComponent<WheelJoint2D>();
        spd = moveSpeed;
    }


    private void Update()
    {
        if (robotTr != null) MoveAction();
        MoveToKeyboard();
    }

    void MoveToKeyboard()
    {
        h = Input.GetAxisRaw("Horizontal");
        if (!gameCtrl.isGamePlay) h = 0f;
    }


    public void InputMoveDirection(string _direction)
    {
        switch (_direction)
        {
            case "Left": h = -1f; break;
            case "Right": h = 1f; break;
        }
    }


    void MoveAction()
    {
        if (h > 0 && robotTr.position.x < 20f)  //오른쪽 이동
        {
            isMove = true;
            if (wJoint != null) wJoint.useMotor = true;
            spd = moveSpeed;
            jMotor.motorSpeed = spd;
            jMotor.maxMotorTorque = 10000f;

            if (wJoint != null) wJoint.motor = jMotor;
        }
        else if (h < 0 && robotTr.position.x > -20f)  //왼쪽 이동
        {
            isMove = true;
            if (wJoint != null) wJoint.useMotor = true;
            spd = -moveSpeed;
            jMotor.motorSpeed = spd;
            jMotor.maxMotorTorque = 10000f;

            if (wJoint != null) wJoint.motor = jMotor;
        }

        if (h == 0f && isMove)
        {
            isMove = false;
            BreakControl();
        }
    }

    void BreakControl()
    {
        wJoint.useMotor = true;
        jMotor.motorSpeed = 0f;
        jMotor.maxMotorTorque = 1000f;
        wJoint.motor = jMotor;
    }
}
