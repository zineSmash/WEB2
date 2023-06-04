using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RobotMouseMoveController : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    public RobotBattleSceneController gameCtrl;
    public RobotPosSetupController point;

    public RelativeJointController rjoint;
    public WheelJoint2D wjoint;

    public Vector3 resetPos;
    public Transform resetParentTr;
    public Transform parentTargetTr;  //부품 오브젝트가 조립되면 할당할 부모트랜스폼

    public bool isTrigger = false;
    public bool isMove = false;
    public bool isAssem = false;
    string[] soundName = { "eff_Common_dragstart", "eff_Robot_equip", "eff_Robot_unequip" };

    //[Header("음향")]
    //[SerializeField] AudioSource sfxPlayer;
    //[SerializeField] AudioClip grabSound;
    //[SerializeField] AudioClip releaseSound;
    //[SerializeField] AudioClip asseySound;



    private void Awake()
    {
        gameCtrl = FindObjectOfType<RobotBattleSceneController>();
        if (this.GetComponent<RelativeJointController>() != null) rjoint = this.GetComponent<RelativeJointController>();
        if (this.GetComponent<WheelJoint2D>() != null) wjoint = this.GetComponent<WheelJoint2D>();
    }

    private void Start()
    {
        resetParentTr = this.transform.parent;
        PartsSet();

    }

    void PartsSet()  //부품의 위치 셋업
    {
        if (gameCtrl.isGamePlay) return;
        PartsPositionSetup();
        gameCtrl.PointImageCheck(false);
    }

    private void OnMouseDown()
    {
        MouseDownInputFunc();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        //MouseDownInputFunc();
    }

    public void OnDrag(PointerEventData eventData)
    {
        //MouseDragInputFunc();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //PartsSet();
    }

    void MouseDownInputFunc()
    {
        if (gameCtrl.isDontMove) return;
        SoundManager.instance.PlayEffectSound(soundName[0], 1f);
        //포인트 포지션 리셋, 콜라이더 활성화, 조립되었던 상태에서의 부모포인트 콜라이더 활성화
        RobotPosSetupController[] _points = this.GetComponentsInChildren<RobotPosSetupController>();

        foreach (RobotPosSetupController _posCtrl in _points)
        {
            _posCtrl.transform.localPosition = _posCtrl.resetPos;
            _posCtrl.GetComponent<CircleCollider2D>().enabled = true;
            _posCtrl.isTrigger = false;
            if (_posCtrl.pointTr != null)
            {
                _posCtrl.pointTr.GetComponent<CircleCollider2D>().enabled = true;
                _posCtrl.pointTr.GetComponent<RobotPosSetupController>().isTrigger = false;
            }
            _posCtrl.isMove = isMove;
        }

        //for (int i = 0; i < _points.Length; i++)
        //{
        //    _points[i].transform.localPosition = _points[i].resetPos;
        //    _points[i].GetComponent<CircleCollider2D>().enabled = true;
        //    _points[i].isTrigger = false;
        //    if (_points[i].pointTr != null)
        //    {
        //        _points[i].pointTr.GetComponent<CircleCollider2D>().enabled = true;
        //        _points[i].pointTr.GetComponent<RobotPosSetupController>().isTrigger = false;
        //    }
        //    _points[i].isMove = isMove;
        //}
    }

    void MouseDragInputFunc()  //마우스 드래그 중일 때의 동작
    {
        if (gameCtrl.isDontMove) return;
        if (isAssem)
        {
            //조립 시 UI 의 공격력, 체력 수치 반영
            gameCtrl._healthPoint = (this.transform.GetComponent<RobotHealthController>().maxHp * -1);

            if (this.transform.GetComponent<RobotWeaponFireController>() != null)
            {
                gameCtrl._attackPoint = (this.transform.GetComponent<RobotWeaponFireController>().atkDamage * -1);
            }
        }

        if (gameCtrl.isGamePlay || gameCtrl.isGameOver) return;
        isMove = true;
        isAssem = false;
        this.transform.SetParent(resetParentTr);

        //조인트 connectedBody 비우기, 커넥트바디를 원부모 트랜스폼으로 변경, 위치 변경
        if (rjoint != null)
        {
            rjoint.relJoint.connectedBody = null;
            rjoint.relJoint.enabled = false;
        }
        if (wjoint != null)
        {
            wjoint.connectedBody = null;
            wjoint.enabled = false;
        }

        //포인트 이미지 활성화
        gameCtrl.PointImageCheck(true);

        //포인트 포지션 리셋, 콜라이더 활성화, 조립되었던 상태에서의 부모포인트 콜라이더 활성화
        RobotPosSetupController[] _points = this.GetComponentsInChildren<RobotPosSetupController>();
        for (int i = 0; i < _points.Length; i++)
        {
            _points[i].transform.localPosition = _points[i].resetPos;
            _points[i].GetComponent<CircleCollider2D>().enabled = true;

            if (_points[i].pointTr != null)
            {
                _points[i].pointTr.GetComponent<CircleCollider2D>().enabled = true;
            }
            _points[i].isMove = isMove;
        }

        //부품 오브젝트 마우스 따라다니기
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos) + new Vector3(0f, 0f, 10f);
        this.transform.position = mousePos;
    }


    private void OnMouseDrag()
    {
        MouseDragInputFunc();
    }

    private void OnMouseUp()
    {
        PartsSet();
    }

    public void PartsPositionSetup()   //부품 위치 정렬
    {
        Vector3 _movePos;
        if (isTrigger)  //부품 하위의 포인트가 다른 포인트와 맞닿아있는 상태인 경우 부품을 놓으면, 맞닿은 위치 기준으로 조립
        {
            SoundManager.instance.PlayEffectSound(soundName[1], 1f);
            _movePos = point.PositionSetup();    //포인트에 닿은 상대포인트 위치값 불러와서
            this.transform.position = _movePos;  //부품의 위치 할당



            //point.transform.localPosition = point.resetPos; //부품의 위치에 맞춰 하위포인트의 로컬포지션 할당
            this.transform.SetParent(parentTargetTr);  //상대 포인트 트랜스폼 하위로 부품 오브젝트 할당

            //조립 시 UI 의 공격력, 체력 수치 반영
            gameCtrl._healthPoint = this.transform.GetComponent<RobotHealthController>().maxHp;
            
            if(this.transform.GetComponent<RobotWeaponFireController>() != null)
            {
                gameCtrl._attackPoint = this.transform.GetComponent<RobotWeaponFireController>().atkDamage;
            }
                       
            if(this.transform.GetComponent<RobotCrWeaponController>() != null)
            {
                gameCtrl._attackPoint = this.transform.GetComponent<RobotCrWeaponController>().atkDamage;
            }



            //조인트 부모 오브젝트 할당 (connectedBody)
            this.transform.GetComponent<RobotPartsController>().ConnectBodySetup(parentTargetTr);
            //Debug.Log("포인트가 무엇이나 : "+ point.name);
            //조인트 오프셋 설정
            if (this.rjoint!=null) 
            { 
                rjoint.enabled = true; 
                //Debug.Log("포인트리셋포즈: " + point.resetPos); 
                rjoint.relJoint.linearOffset = point.resetPos; 
            }

            if (this.wjoint!=null)
                wjoint.connectedAnchor = this.transform.parent.localPosition;

            //조립 시 포인트의 콜라이더 비활성화
            CircleCollider2D[] _pointTr = this.GetComponentsInChildren<CircleCollider2D>();
            for (int i = 0; i < _pointTr.Length; i++)
            {
                if (_pointTr[i].transform.GetComponent<WheelJoint2D>() != null) continue;
                else
                {
                   if(_pointTr[i].transform.GetComponent<RobotPosSetupController>().pointTr !=null) _pointTr[i].transform.GetComponent<RobotPosSetupController>().pointTr.GetComponent<CircleCollider2D>().enabled = false;
                    _pointTr[i].transform.localPosition = _pointTr[i].transform.GetComponent<RobotPosSetupController>().resetPos;
                    _pointTr[i].GetComponent<CircleCollider2D>().enabled = false;

                }
            }
            isAssem = true;
            point = null;
            isMove = false;
            isTrigger = false;
        }
        else  //하위의 포인트에 닿은 다른 포인트가 없을 경우 부품을 놓으면, 원래 있던 자리로 복귀
        {
            SoundManager.instance.PlayEffectSound(soundName[2], 1f);
            CircleCollider2D[] _pointTr = this.GetComponentsInChildren<CircleCollider2D>();
            for (int i = 0; i < _pointTr.Length; i++)
            {
                if (_pointTr[i].transform.GetComponent<WheelJoint2D>() != null) continue;
                else
                {
                    _pointTr[i].GetComponent<CircleCollider2D>().enabled = true;
                }
                _pointTr[i].GetComponent<RobotPosSetupController>().isMove = false;
            }
            isMove = false;
            isTrigger = false;
            //this.transform.position = resetParentTr.position;
        }
    }
}
