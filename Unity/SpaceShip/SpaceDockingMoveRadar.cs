using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ֺ����
/// ��ŷ ��忡���� ��ŷ�� ���̴� ��Ʈ�ѽ�ũ��Ʈ
/// </summary>

public class SpaceDockingMoveRadar : MonoBehaviour
{
    [Header("���ӿ�����Ʈ")]
    private SpaceDockingRadar radarCtrl;
    private CircleCollider2D coll;
    public Transform dockingTr;
    public GameObject joystick;
    public GameObject dockingGauge;

    [Header("���ӵ�����")]
    public bool isMatch = false;
    public bool isMove = true;
    public float maxMatchCount = 5f;
    public float currentMatchCount = 0f;
    private float delay = 1f;
    private float soundDelay = 0f;

    private void Start()
    {
        radarCtrl = transform.parent.parent.GetComponent<SpaceDockingRadar>();
    }

    private void OnEnable()
    {
        joystick.SetActive(true);
        dockingGauge.SetActive(true);
        isMove = true;
        coll = GetComponent<CircleCollider2D>();
        coll.enabled = true;
        maxMatchCount = 5f;
        currentMatchCount = 0f;
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.name == "DockingStation")  //���̴��� ��ŷ �ڸ��� ��ġ�ϸ� ��ŷ������ ����
        {
            currentMatchCount += Time.deltaTime;
            radarCtrl.SetDockingGauge(currentMatchCount, maxMatchCount);

            soundDelay -= Time.deltaTime;
            if(soundDelay < 0f)
            {
                SoundManager.instance.PlayEffectSound("eff_Robot_matching", 1f);
                soundDelay = 1f;
            }

            if (currentMatchCount > maxMatchCount)
            {
                isMove = false;
                coll.enabled = false;
                joystick.SetActive(false);
                //dockingGauge.SetActive(false);
                SpaceGameManager.instance.ClearDockingMission();
                this.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (delay > 0f)
        {
            delay -= Time.deltaTime;
        }
        
        if (currentMatchCount > 0 && delay < 0 && isMove)  //��ŷ������ ����
        {
            currentMatchCount -= Time.deltaTime * 0.5f;
            radarCtrl.SetDockingGauge(currentMatchCount, maxMatchCount);
        }

        if (!isMove)  //��ŷ �Ϸ� �� ���̴� ��ġ ��ŷ ��ġ�� �̵�
        {
            this.transform.position
                = new Vector3(dockingTr.position.x, dockingTr.position.y, this.transform.position.z);
        }
    }
}
