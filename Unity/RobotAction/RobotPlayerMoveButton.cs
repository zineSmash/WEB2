using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.EventSystems;

public class RobotPlayerMoveButton : MonoBehaviour//, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] RobotBattleSceneController gameCtrl;
    //[SerializeField] WheelJointController wheelCtrl;
    [SerializeField] RobotPlayerMoveController moveCtrl;
    [SerializeField] string goName;
    [SerializeField] bool isMoveLeft;
    [SerializeField] bool isMoveRight;

    private void OnEnable()
    {
        gameCtrl = FindObjectOfType<RobotBattleSceneController>();
        goName = this.gameObject.name;
        moveCtrl = FindObjectOfType<RobotPlayerMoveController>();
        //WheelJointController[] _allWheelCtrl = FindObjectsOfType<WheelJointController>();
        //for(int i = 0; i < _allWheelCtrl.Length; i++)
        //{
        //    if(_allWheelCtrl[i].gameObject.layer == LayerMask.NameToLayer("PLAYER"))
        //    {
        //        wheelCtrl = _allWheelCtrl[i];
        //        return;
        //    }
        //}
        isMoveLeft = false;
        isMoveRight = false;
    }

    private void Update()
    {
        if (gameCtrl.isGamePlay)
        {
            if (isMoveLeft) moveCtrl.MoveLeft();
            if (isMoveRight) moveCtrl.MoveRight();
        }
    }

    public void OnPointerDown()
    {
        if (!gameCtrl.isGamePlay) return;

        switch (goName)
        {
            case "MoveLeft": isMoveLeft = true; Debug.Log("왼쪽"); break;
            case "MoveRight": isMoveRight = true; Debug.Log("오른쪽"); break;
        }
    }

    public void OnPointerUp()
    {
        isMoveLeft = false;
        isMoveRight = false;
    }

}
