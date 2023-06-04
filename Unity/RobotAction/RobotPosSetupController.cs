using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotPosSetupController : MonoBehaviour
{
    [SerializeField] RobotMouseMoveController mouseCtrl;

    public Vector3 resetPos;  //포인트의 로컬포지션값
    [SerializeField] Vector3 addPosition;  //포인트가 위치할 벡터좌표값
    
    [SerializeField] Sprite[] pointSprite;  //포인트의 활성비활성 유무에 따른 조립가능포인트 이미지 할당
    [SerializeField] SpriteRenderer pointImage;  //화면에 표시할 포인트 이미지
    [SerializeField] CircleCollider2D cColl;     //포인트의 콜라이더
    public Transform pointTr;    //포인트의 트랜스폼 변수

    public bool isTrigger = false; //포인트끼리 닿았는지를 확인할 변수
    public bool isMove = false;    //부품 오브젝트 조작 여부 (마우스 또는 터치 후 드래그 중일 때)


    public CircleCollider2D otherColl = new CircleCollider2D();

    private void Awake()
    {
        mouseCtrl = this.transform.parent.GetComponent<RobotMouseMoveController>();
        cColl = GetComponent<CircleCollider2D>();
        cColl.enabled = true;
        resetPos = this.transform.localPosition;
        pointImage = GetComponentInChildren<SpriteRenderer>();
        isTrigger = false;
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {


        if (collision.gameObject.layer == LayerMask.NameToLayer("POINT") && isMove)  //포인트끼리 닿을 경우 (부품 조작 시 + 포인트 외에는 무시)
        {
            isTrigger = true;
            //Debug.Log("포인트끼리 부딪힘");
            addPosition = collision.transform.position;  //포인트가 위치할 벡터값을 할당 (닿은 포인트의 위치값)
            pointTr = collision.transform;               //닿은 포인트의 트랜스폼 할당
            if (mouseCtrl != null)
            {
                mouseCtrl.isTrigger = true;
                mouseCtrl.point = this;
            }
        }
        else return;
    }

    private void Update()
    {
        if (this.transform.parent.gameObject.layer == LayerMask.NameToLayer("ENEMY"))
        {
            pointImage.enabled = false;
        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("POINT") && isMove)
        { isTrigger = true; pointImage.enabled = true; pointImage.sprite = pointSprite[1]; }
        else return;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("POINT")) isTrigger = false;
        else return;

        pointImage.enabled = true; 
        pointImage.sprite = pointSprite[0]; 
        if (mouseCtrl != null)
        {
            mouseCtrl.isTrigger = false;
            //mouseCtrl.point = null;
        }
    }

    public Vector3 PositionSetup()  //부품을 프레임에 조립할 경우, 포인트의 위치와 부품몸체의 위치를 보간 처리
    {
        Vector3 _distance = new Vector3(this.transform.position.x - this.transform.parent.position.x, this.transform.position.y - this.transform.parent.position.y, 0f);

        this.transform.position = addPosition;
        _distance = addPosition - _distance;
        isMove = false;
        cColl.enabled = false;
        mouseCtrl.parentTargetTr = pointTr;
        return _distance;
    }


    public void PointImageOn()  //포인트 이미지 활성화
    {
        pointImage.enabled = true;
        if (cColl.enabled)
        {
            pointImage.sprite = pointSprite[0];
        }
        else if (!cColl.enabled)
        {
            pointImage.sprite = pointSprite[1];
        }
    }

    public void PointImageOff()  //포인트 이미지 비활성화
    {
        pointImage.enabled = false;
    }

}
