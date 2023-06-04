using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// ���ֺ����
/// ���ּ��� �����̱� ���� �������̽�ƽ ���� ��ũ��Ʈ
/// </summary>

public class SpaceDockingStickController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("��ũ��Ʈ��Ʈ�ѷ�")]
    public SpaceDockingGameController dockingGameCtrl;

    [Header("���ӿ�����Ʈ")]
    public RectTransform rectBackground;
    public RectTransform rectJoystick;

    [Header("���ӵ�����")]
    public float radius;
    public float leverRange = 50f;
    private Vector2 inputVector;
    private bool isInput = false;

    private void Start()
    {
        radius = rectBackground.rect.width * 0.5f;
        leverRange = 50f;
    }

    private void OnEnable()
    {
        rectJoystick.localPosition = Vector2.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //��ƽ�� ��ġ ����
        SetJoystickLeverDirection(eventData);
        isInput= true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //��ƽ ��ġ ����
        rectJoystick.localPosition = Vector2.zero;
        isInput= false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //��ƽ ��ġ ��
        SetJoystickLeverDirection(eventData);
    }

    public void SetJoystickLeverDirection(PointerEventData eventData)  //��ƽ ���Ͱ� ����
    {
        float screenScale = (float)Screen.width / 1280;
        Vector2 newPos = new Vector2(eventData.position.x / screenScale, eventData.position.y / screenScale);
        var inputDir = newPos - rectBackground.anchoredPosition;
        var clampedDir = inputDir.magnitude < leverRange ? inputDir : inputDir.normalized * leverRange;

        rectJoystick.anchoredPosition = clampedDir;
        inputVector = clampedDir / leverRange;
    }


    private void Update()
    {
        if (isInput)
        {
            InputControlVector();  
        }
    }

    private void InputControlVector() //��ƽ ���Ͱ� ����
    {
        dockingGameCtrl.SetShipMoveDirection(inputVector);
        dockingGameCtrl.SetRadarMoveDirection(inputVector);
    }

    
}
