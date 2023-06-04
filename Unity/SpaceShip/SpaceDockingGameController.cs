using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���ֺ����
/// ��ŷ ��Ʈ ������Ʈ�� ��ũ��Ʈ
/// </summary>

public class SpaceDockingGameController : MonoBehaviour
{
    private SpaceDockingShipController shipCtrl;
    private SpaceDockingStationController stationCtrl;
    private SpaceDockingRadar radarCtrl;
    public SpaceObjectPool objPool;

    [Header("���ӿ�����Ʈ")]
    public GameObject dockingShip;
    public GameObject dockingStation;
    public GameObject obstacle;
    public GameObject[] currentShipStation = new GameObject[2];
    public GameObject dockingRadar;
    public Button[] rotateButtons;
    public Button dockingButton;

    [Header("���ӵ�����")]
    public bool isDocking = false;
    public bool isTryDocking = false;
    public bool isDockingMode = false;
    float obstacleStartTimer = 6;
    public float obstacleDelayTimer = 0.25f;
    float obstacleTimer = 0;
    public int obstacleGenerateCount = 2;
    List<GameObject> obstacles = new List<GameObject>();

    [Header("�ؽ�ƮUI")]
    public TextMeshProUGUI textTimer;
    public TextMeshProUGUI textDistance;


    public void RendezvousDockingModeSetup()  //�ʱ� ���Ӽ¾�
    {
        textTimer.text = "";
        textDistance.text = "";

        isDocking = false;
        isDockingMode = false;
        isTryDocking = false;
        dockingRadar.SetActive(false);
        dockingButton.interactable = false;

        //����� �� ������ �����ϴ� ���ּ�/������ ���� �� �����
        for (int i = 0; i < currentShipStation.Length; i++)
        {
            if (currentShipStation[i] != null)
            {
                Destroy(currentShipStation[i].gameObject);
                currentShipStation[i] = null;
            }
        }

        currentShipStation[0] = Instantiate(dockingShip, transform.position, Quaternion.identity);
        currentShipStation[1] = Instantiate(dockingStation, dockingStation.transform.position, Quaternion.identity);
        for (int i = 0; i < currentShipStation.Length; i++)
        {
            currentShipStation[i].transform.SetParent(transform, false);
        }
        shipCtrl = currentShipStation[0].GetComponent<SpaceDockingShipController>();
        stationCtrl = currentShipStation[1].GetComponent<SpaceDockingStationController>();

        //���� ������ ��ֹ� ������Ʈ�� �ִٸ� ����
        if (objPool.transform.childCount > 0)
        {
            List<Transform> list = new List<Transform>();
            list.AddRange(objPool.transform.GetComponentsInChildren<Transform>());
            list.RemoveAt(0);
            foreach(Transform obj in list)
            {
              if(obj.gameObject.activeSelf) obj.gameObject.SetActive(false);
            }
        }

        //foreach (GameObject go in obstacles)
        //{
        //    if (go != null)
        //    {
        //        Destroy(go);
        //    }
        //    else continue;
        //}
        //obstacles.Clear();

        //��ֹ� ������ƮǮ�� �¾�
        //SpaceObjectPooler.inst.SetupObstacle();
        obstacleTimer = obstacleStartTimer;

        //ī�޶� ����
        GameObject.Find("Main Camera").GetComponent<SpaceCameraController>().GetGameMode(GameMode.DockingMode, currentShipStation[0].transform);
    }

    #region ���ּ� / ��ŷ���̴� ����

    public void SetShipMoveDirection(Vector2 _dir)    //���ּ� �̵� ���Ⱚ ���� 
    {
        shipCtrl.GetMoveValue(_dir);
    }

    public void OnClickRotate(float _value)  //���ּ� ȸ�� (��ư �̺�ƮƮ���ſ� �¿찢�� -1, 1 �Ҵ�)
    {
        shipCtrl.GetRotateValue(_value);

        //ȸ�� ���� ��� (�ݺ�/����)
        if (_value != 0)
        {
            SoundManager.instance.PlayEffectSoundLoop("eff_Space_rotate", 0.7f);
        }
        else
        {
            SoundManager.instance.StopEffSoundLoop();
        }
    }


    public void SetRadarMoveDirection(Vector2 _dir)  //��ŷ ���̴� �̵� ���Ⱚ ����
    {
        if (isDockingMode)
        {
            radarCtrl.GetMoveDirection(_dir);
        }
    }

    #endregion

    #region ������ ���

    //private void Update()
    //{
    //    if (currentShipStation[0] && !isDocking)
    //    {
    //        GenerateObstacle();
    //    }
    //}

    //void GenerateObstacle()  //��ֹ� ����
    //{
    //    obstacleTimer -= Time.deltaTime;
    //    if (obstacleTimer < 0)
    //    {
    //        if (currentShipStation[0] != null)
    //        {
    //            SpaceObjectPooler.SpawnFromPool<SpaceDockingObstacle>("DockingObstacle", Vector3.zero).targetTr = currentShipStation[0].transform;
    //        }
    //        else
    //        {
    //            SpaceObjectPooler.SpawnFromPool<SpaceDockingObstacle>("DockingObstacle", Vector3.zero).targetTr = currentShipStation[1].transform;
    //        }

    //        obstacleTimer = obstacleDelayTimer;
    //    }
    //}

    #endregion


    #region ��ŷ ���

    public void IntoDockingMode()  //��ŷ ��� ����
    {
        System.GC.Collect();
        SpaceGameManager.instance.resultPanel.SetActive(false);
        SpaceGameManager.instance.cameraCtrl.transform.position = new Vector3(0f, 0f, SpaceGameManager.instance.cameraCtrl.transform.position.z);
        isDocking = true;

        //foreach (GameObject go in obstacles)
        //{
        //    if (go != null)
        //    {
        //        Destroy(go);
        //    }
        //    else continue;
        //}
        //obstacles.Clear();

        shipCtrl.IntoDockingMode();
        stationCtrl.IntoDockingMode();
        SpaceGameManager.instance.FadeEffect();
        StartCoroutine(DockingModeOn());
    }

    IEnumerator DockingModeOn()  //��ŷ ��� ���� ������, ȭ�� ����
    {
        dockingRadar.SetActive(true);
        yield return new WaitForSeconds(2);
        isDockingMode = true;
        radarCtrl = dockingRadar.GetComponent<SpaceDockingRadar>();
    }

    public void SetToDockingZone(bool _value)
    {
        isTryDocking = _value;
        dockingButton.interactable = _value;
    }

    public void OnClickDockingButton()  //��ŷ ��ư ����
    {
        if (isTryDocking)
        {
            IntoDockingMode();
            SoundManager.instance.PlayEffectSound("RobotAsseySound", 1f);
        }
    }

    #endregion

}
