using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ֺ����
/// ������ ����� ���������� ��ũ��Ʈ
/// </summary>

public class SpaceDockingStationController : MonoBehaviour
{
    [Header("���ӿ�����Ʈ")]
    public Transform targetTr; //�����̼��� ȸ���� �߽� Ʈ������

    [Header("���ӵ�����")]
    public float moveSpeed = 1f;
    public float rotSpeed = 2f;
    public float distanceFromCenter = 16f;
    public bool isDocking = false;

    private void Start()
    {
        targetTr = transform.parent;
        int dirIndex = Random.Range(0, 4);
        float xPos = 0, yPos = 0;
        switch (dirIndex)
        {
            case 0: xPos = distanceFromCenter; yPos = 0; break;
            case 1: xPos = -distanceFromCenter; yPos = 0; break;
            case 2: xPos = 0f; yPos = distanceFromCenter; break;
            case 3: xPos = 0f; yPos = -distanceFromCenter; break;
        }
        this.transform.position = new Vector2(xPos, yPos);
    }

    private void FixedUpdate()
    {
        if (!isDocking)
        {
            MoveAround();
            StationRotate();
        }
    }

    void MoveAround()  //Vector(0,0) �� ���� �������� �ֺ� ����
    {
        transform.RotateAround(targetTr.position, Vector3.forward, moveSpeed * Time.deltaTime);
    }

    void StationRotate()  //���������� ��ü ȸ��
    {
        transform.Rotate(new Vector3(0, 0, -rotSpeed * Time.deltaTime));
    }

    public void IntoDockingMode()  //��ŷ ��� ����
    {
        isDocking = true;
        this.transform.position = new Vector3(0f, 2f, 0f);
        this.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
    }

}
