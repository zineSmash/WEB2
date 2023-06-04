using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���ֺ����
/// ���ּ� �߻� ��� ������Ʈ�� ��ũ��Ʈ
/// </summary>

public class SpaceLauncherGameController : MonoBehaviour
{
    [Header("��������Ʈ/������")]//���
    public GameObject groundBg;
    public GameObject[] skyBgs;
    public Vector3[] bgResetPos = new Vector3[3];
    public bool isBgMove = false;
    public float bottomYposition = -10f;
    public float topYposition = 10f;
    public float moveSpeed = 0f;
    public float maxMoveSpeed = 20f;
    public float gameTimer = 0f;
    public float flyDistance = 0f;
    public TextMeshProUGUI textGameTimer;
    public TextMeshProUGUI textFlyDistance;

    [Header("��ưUI")]//UI
    public Button[] separateButtons;
    public Button launcherButton;
    //public Button readyButton;

    [Header("���ּ�������Ʈ/������")] //���ּ�
    public SpaceLauncherShipController shipCtrl;
    public GameObject spaceShipPrefab;//���ּ�������
    public GameObject spaceShipObj;  //ȭ�鿡 ��µǴ� ���ּ�
    public float[] fuelLevel = new float[3];
    public float maxFuelLevel = 0f;
    public Image[] fuelGaugeBar;

    [Header("�߻�������Ʈ/������")]//�߻��
    public Image powerGauge;
    public float maxPower = 2.5f;
    public float minPower = 0.1f;
    public float changePower = 5f;
    public float currentPower = 0f;
    public float fixPower = 0f;
    public float launchTimeCount = 0f;
    public bool isMax = false;
    public bool isMin = false;
    public bool isReady = false;



    private void Start()
    {
        SetBgPosition();
        LauncherGameStart();
    }

    private void Update()
    {
        FirePowerGaugeControl();  //�Ŀ������� ����
        BgMove();  //��� �����̱�
        UseFuel();  //���ּ� ���� �Ҹ�
    }

    #region ����/���� �Լ�

    void SetBgPosition()  //��� ������Ʈ �ʱ� ��ġ ����
    {
        bgResetPos[0] = groundBg.transform.localPosition;
        bgResetPos[1] = skyBgs[0].transform.localPosition;
        bgResetPos[2] = skyBgs[1].transform.localPosition;
    }

    public void RetryGame()  //��� ����ġ, ���� ���ּ��� ���� ��� ����
    {
        StopAllCoroutines();
        SoundManager.instance.StopAllEffSound();
        System.GC.Collect();

        isReady = false;
        isBgMove = false;

        //���ӽð� + �߻� �� �̵��Ÿ�
        gameTimer = 0f;
        textGameTimer.text = "00:00";
        flyDistance = 0f;
        textFlyDistance.text = "0Km";

        //��� ����ġ
        if (!groundBg.activeSelf) groundBg.SetActive(true);
        groundBg.transform.localPosition = bgResetPos[0];
        skyBgs[0].transform.localPosition = bgResetPos[1];
        skyBgs[1].transform.localPosition = bgResetPos[2];
        for (int i = 3; i < skyBgs.Length; i++)
        {
            skyBgs[i].transform.position = new Vector3(Random.Range(-7f, 6), Random.Range(10f, 15f), skyBgs[i].transform.position.z);
        }

        //���ּ�
        if (spaceShipObj != null)
        {
            Destroy(spaceShipObj.gameObject);
            spaceShipObj = null;
        }
        //UI
        powerGauge.transform.parent.gameObject.SetActive(false);
        separateButtons[0].interactable = true;
        separateButtons[1].interactable = true;
        launcherButton.interactable = true;
        //readyButton.interactable = true;

        for (int i = 0; i < fuelLevel.Length; i++)
        {
            fuelLevel[i] = 0f;
            fuelGaugeBar[i].fillAmount = fuelLevel[i] / maxFuelLevel;
        }

        LauncherGameStart();
    }


    public void LauncherGameStart()  //�߻����� ����
    {
        if (isReady) return;
        isReady = true;

        //���ᷮ �ʱ�ȭ = 0
        maxFuelLevel = 1f;
        for (int i=0;i< fuelLevel.Length;i++)
        {
            fuelLevel[i] = 0f;
            fuelGaugeBar[i].fillAmount = fuelLevel[i] / maxFuelLevel;
        }

        //���ᷮ�� ���� �Ŀ������� Ȱ��ȭ
        powerGauge.transform.parent.gameObject.SetActive(true);
        currentPower= minPower;
        isMin = true;

        //���ּ� Ȱ��ȭ
        SpaceShipSetup();
        //readyButton.interactable = false;
    }


    IEnumerator FinishFunction()  //���� ����
    {
        //���â ����
        SoundManager.instance.StopEffSoundLoop();
        SpaceGameManager.instance.ClearLauncherMission(flyDistance);
        yield return new WaitForSeconds(5);
        //���â ��Ȱ��ȭ �� ���ּ� �̵� (���ּ� �̵��� ���ÿ� ���̵�ƿ� ȿ��)
        SpaceGameManager.instance.resultPanel.SetActive(false);
        shipCtrl.FinishMove();
    }

    #endregion


    #region ���ּ�
    void SpaceShipSetup()  //���ּ� �¾�
    {
        spaceShipObj = Instantiate(spaceShipPrefab);
        spaceShipObj.transform.SetParent(this.transform);
        spaceShipObj.transform.localPosition = new Vector3(0f, 0.7f, 0f);
        shipCtrl = spaceShipObj.GetComponent<SpaceLauncherShipController>();
        separateButtons[0].onClick.AddListener(shipCtrl.SeparatePart1);
        separateButtons[1].onClick.AddListener(shipCtrl.SeparatePart2);
    }

    void UseFuel()   //���� �Ҹ�
    {
        if (!isBgMove) return;

        if(shipCtrl != null)
        {
            if (shipCtrl.isLaunchLevel[0] && !shipCtrl.isLaunchLevel[1])  //����3 ���� �Ҹ�
            {
                fuelLevel[2] -= Time.deltaTime * (moveSpeed * 0.1f);
                fuelGaugeBar[2].fillAmount = fuelLevel[2] / maxFuelLevel;
            }

            if (shipCtrl.isLaunchLevel[1] && !shipCtrl.isLaunchLevel[2])  //����2 ���� �Ҹ�
            {
                fuelLevel[1] -= Time.deltaTime * (moveSpeed * 0.1f);
                fuelGaugeBar[1].fillAmount = fuelLevel[1] / maxFuelLevel;
            }

            if (shipCtrl.isLaunchLevel[2])  //����1 ���� �Ҹ�
            {
                fuelLevel[0] -= Time.deltaTime * (moveSpeed * 0.1f);
                fuelGaugeBar[0].fillAmount = fuelLevel[0] / maxFuelLevel;
            }
        }


        if (fuelLevel[0] < 0)
        {
            isBgMove = false;
            StartCoroutine(FinishFunction());
        }
        else if(!shipCtrl.isLaunchLevel[1] && fuelLevel[2] < 0f)  //����3
        {
            isBgMove = false;
            separateButtons[0].interactable= false;
            spaceShipObj.GetComponent<SpaceLauncherShipController>().FailSeparateParts();
        }
        else if(!shipCtrl.isLaunchLevel[2] && fuelLevel[1] < 0f)  //����2
        {
            isBgMove = false;
            separateButtons[1].interactable = false;
            spaceShipObj.GetComponent<SpaceLauncherShipController>().FailSeparateParts();
        }
    }

    #endregion


    #region ��ó 

    void FirePowerGaugeControl()  //�Ŀ������� ���� ȿ��
    {
        if (isMax && powerGauge.gameObject.activeSelf)
        {
            currentPower -= changePower * Time.deltaTime;
            if(currentPower < minPower )
            {
                currentPower= minPower;
                isMax = false;
                isMin= true;
            }
        }
        else if(isMin&& powerGauge.gameObject.activeSelf)
        {
            currentPower += changePower * Time.deltaTime;
            if (currentPower > maxPower)
            {
                currentPower= maxPower;
                isMin= false;
                isMax= true;
            }
        }
        powerGauge.fillAmount= currentPower / maxPower;
    }

    public void OnClickLauncherButton()  //�߻� ��ư (�߻�)
    {
        if (!isMax && !isMin) return;

        isMin= false;
        isMax= false;
        fixPower = currentPower;
        moveSpeed = maxPower;
        maxFuelLevel = fixPower * 30f;

        //���ᷮ ����
        for (int i = 0; i < fuelLevel.Length; i++)
        {
            fuelLevel[i] = maxFuelLevel;
            fuelGaugeBar[i].fillAmount = fuelLevel[i] / maxFuelLevel;
        }

        launcherButton.interactable = false;
        StartCoroutine(LauncherSpaceShip());
    }

    IEnumerator LauncherSpaceShip()
    {
        //ī��Ʈ �ٿ�
        WaitForSeconds wfs = new WaitForSeconds(0.5f);
        TextMeshProUGUI text = SpaceGameManager.instance.OnStartPanel();
        text.transform.localPosition = Vector3.zero;
        text.text = "T-Minus";
        yield return wfs; 
        yield return wfs;
        SoundManager.instance.PlayEffectSound("eff_Space_count", 1f);
        for(int i = 3; i > -1; i--)
        {
            text.text = i.ToString();

            yield return wfs; //������
            yield return wfs; //������
        }
        text.text = "LIFTOFF";

        yield return wfs; //������
        
        shipCtrl.LaunchAction();
        powerGauge.transform.parent.gameObject.SetActive(false);  //�Ŀ������� ��Ȱ��ȭ
        yield return wfs; //������
        SpaceGameManager.instance.startPanel.SetActive(false);
    }

    #endregion

    #region ���
    void BgMove()
    {
        if (isBgMove)
        {
            gameTimer += Time.deltaTime;
            textGameTimer.text = ((int)gameTimer / 60).ToString("00") + ":" + ((int)(gameTimer % 60)).ToString("00");

            flyDistance += fixPower * Time.deltaTime * (moveSpeed * 0.1f);
            textFlyDistance.text = flyDistance.ToString("f1") + "Km";

            //�� ����� ������Ʈ�� Ȱ��ȭ�Ǿ� �ִ� ��츸 ��ũ�� ó��
            if (groundBg.activeSelf)
            {
                groundBg.transform.position += new Vector3(0, (-moveSpeed) * Time.deltaTime, 0);
            }
            else
            {
                if (moveSpeed < maxMoveSpeed)
                {
                    launchTimeCount += Time.deltaTime;
                    if (launchTimeCount > 0.1f)
                    {
                        moveSpeed += 2f;
                        launchTimeCount = 0f;
                    }
                }
            }

            if (groundBg.transform.position.y < -15f)
            {
                groundBg.SetActive(false);
            }

            //�ϼ� �̹��� ��� ���� (2023.03.24)
            //�ϴ� ����� ���ڸ� �״�� (���߿� ���� ���� ���� �� ������ �װɷ� �ӵ��� ǥ�� ó��)
            //�������� �ӵ����� �츮��,
            //���ᰡ ��� ������ �� ���ַ� ���ư��� ��鿡�� ���ֿ� �ϴù���� ���ļ� ���ַ� ���ư��� ���� ���� ǥ��

            if (gameTimer > 20)
            {
                skyBgs[0].transform.position += new Vector3(0, (-2f) * Time.deltaTime, 0);
                skyBgs[1].transform.position += new Vector3(0, (-2f) * Time.deltaTime, 0);
                skyBgs[2].transform.position += new Vector3(0, (-2f) * Time.deltaTime, 0);
            }


            //���� �̵�
            skyBgs[3].transform.position += new Vector3(0, (-moveSpeed * 0.5f) * Time.deltaTime, 0);
            skyBgs[4].transform.position += new Vector3(0, (-moveSpeed * 0.5f) * Time.deltaTime, 0);
            skyBgs[5].transform.position += new Vector3(0, (-moveSpeed * 0.5f) * Time.deltaTime, 0);

            for (int i = 3; i < skyBgs.Length; i++)
            {
                if (skyBgs[i].transform.position.y < -8f)
                {
                    skyBgs[i].transform.position = new Vector3(Random.Range(-7f, 6), Random.Range(10f, 15f), skyBgs[i].transform.position.z);
                }
            }


            // �Ʒ��� �̹��� �۾� �� �ӽ� �̹����� �۾��� ����

            ////�ϴ�/���� ����� ���� ��ũ�� (2023.03.09 ���� �ӽ� �̹����� ���� ���� �ʿ�)
            //skyBgs[0].transform.position += new Vector3(0, (-moveSpeed) * Time.deltaTime, 0);
            //skyBgs[1].transform.position += new Vector3(0, (-moveSpeed) * Time.deltaTime, 0);

            //if (skyBgs[0].transform.position.y < bottomYposition)
            //{
            //    skyBgs[0].transform.position = new Vector3(0f, topYposition, 0f);
            //}

            //if (skyBgs[1].transform.position.y < bottomYposition)
            //{
            //    skyBgs[1].transform.position = new Vector3(0f, topYposition, 0f);
            //}
        }
    }

    #endregion

}
