using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// ���ֺ����
/// ���ӸŴ��� ��ũ��Ʈ
/// </summary>

public enum GameMode
{
    NONE=0,
    LauncherMode,
    DockingMode,
}


public class SpaceGameManager : MonoBehaviour
{
    public static SpaceGameManager instance;

    [Header("��ũ��Ʈ��Ʈ�ѷ�")]
    private SpaceLauncherGameController launcherCtrl;
    private SpaceDockingGameController dockingCtrl;
    public SpaceCameraController cameraCtrl;
    public SpaceLight lightCtrl;

    [Header("���ӿ�����Ʈ")]
    public GameMode gameMode = 0;
    public GameObject[] launcherGroups;
    public GameObject[] dockingGroups;
    public GameObject pauseMenuPanel;
    public GameObject helpPanel;
    public GameObject resultPanel;
    public GameObject startPanel;
    public GameObject dockingShip;
    public GameObject dockingStationObject;
    public Image fadeImage;
    public Transform bgTr;
    public GameObject spaceBg;
    public List<GameObject> spaceBgList = new List<GameObject>();

    [Header("���ӵ�����")]
    public bool isLauncherReady = false;
    public bool isDockingReady = false;
    public bool isGamePlay = false;
    public int gameModeCount = 0;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        
        gameMode = GameMode.NONE;
    }

    private void Start()
    {
        GameSetup();
    }

    #region ���� ��� �¾�

    void GameSetup() //���ֺ���� ù ���� ��
    {
        gameModeCount = 0;
        LauncherGameSetup();
        //RendezvousDockingGameSetup();
        FadeEffect();
    }

    public void LauncherGameSetup()  //���ּ� �߻� ��� �¾�
    {
        for (int i = 0; i < dockingGroups.Length; i++)
        {
            dockingGroups[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < launcherGroups.Length; i++)
        {
            launcherGroups[i].gameObject.SetActive(true);
        }
        launcherCtrl = launcherGroups[0].GetComponent<SpaceLauncherGameController>();
        gameMode = GameMode.LauncherMode;
        cameraCtrl.GetGameMode(gameMode, null);
        SoundManager.instance.PlayBGMSound("bg_Space_launch", 1f);
    }


    public void RendezvousDockingGameSetup()  //������/��ŷ ��� �¾�
    {
        //System.GC.Collect();
        FadeEffect();
        for (int i=0;i<dockingGroups.Length;i++)
        {
            dockingGroups[i].gameObject.SetActive(true);
        }

        for (int i = 0; i < launcherGroups.Length; i++)
        {
            launcherGroups[i].gameObject.SetActive(false);
        }

        dockingCtrl = dockingGroups[0].GetComponent<SpaceDockingGameController>();
        gameMode = GameMode.DockingMode;
        cameraCtrl.transform.position = new Vector3(0f, 0f, cameraCtrl.transform.position.z);
        dockingCtrl.RendezvousDockingModeSetup();
        StartCoroutine(RendezvouzModeStartCount());  //����ī��Ʈ

        //���������� �÷���/������ �ʱ�ȭ
        Color _color = Color.white;
        Image _image = dockingStationObject.GetComponent<Image>();
        _image.transform.localScale = Vector3.one;
        _image.color = _color;
        SoundManager.instance.PlayBGMSound("bg_Space_docking", 1f);


        //���� ��� �¾� ( x=9ea, y = 7ea )

        if (spaceBgList.Count > 0) return;
        for(int x = -4; x < 5; ++x)
        {
            for(int y = -3; y < 4; ++y)
            {
                GameObject _bg = Instantiate(spaceBg, new Vector3((x * 8), (y * 8), 0f), Quaternion.identity);
                _bg.transform.SetParent(bgTr);
                spaceBgList.Add(_bg);
            }
        }
    }

    IEnumerator RendezvouzModeStartCount()  //������ ��� ���� ī��Ʈ
    {
        WaitForSeconds wfs = new WaitForSeconds(1);
        if (!startPanel.activeSelf) startPanel.SetActive(true);
        startPanel.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        TextMeshProUGUI text = startPanel.GetComponentInChildren<TextMeshProUGUI>();
        text.transform.localPosition = new Vector3(0f, -60f, 0f);

        yield return wfs;
        for (int i = 3; i > -1; i--)
        {
            text.text = i.ToString();
            if (i == 0) text.text = "";
            yield return wfs;
        }

        startPanel.SetActive(false);
    }

    #endregion

    #region ���̵��ξƿ� ȿ��
    public void FadeEffect()  //���̵��ξƿ� ȿ��
    {
        Color color = fadeImage.color;
        if (!fadeImage.gameObject.activeSelf) fadeImage.gameObject.SetActive(true);
        fadeImage.raycastTarget = true;
        color.a = 1;
        fadeImage.color = color;
        StartCoroutine(FadeInoutEffectPlay(color));
    }

    IEnumerator FadeInoutEffectPlay(Color _color)
    {
        while(fadeImage.color.a < 1f)
        {
            _color.a += Time.deltaTime;
            fadeImage.color = _color;
            yield return null;
        }

        yield return new WaitForSeconds(1);

        while (fadeImage.color.a > 0f)
        {
            _color.a -= Time.deltaTime;
            fadeImage.color = _color;
            yield return null;
        }
        fadeImage.raycastTarget = false;
        fadeImage.gameObject.SetActive(false);

    }


    #endregion



    #region �̼� ���� / ���� / ����
    public TextMeshProUGUI OnStartPanel()  //�߻� ��� ��ŸƮ �г� �ѱ�
    {
        if(!startPanel.activeSelf) startPanel.SetActive(true);
        startPanel.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        TextMeshProUGUI text = startPanel.GetComponentInChildren<TextMeshProUGUI>();
        return text;
    }

    public void ClearLauncherMission(float _record)  //���ּ� �߻� �̼� Ŭ����
    {
        resultPanel.SetActive(true);
        resultPanel.transform.GetChild(0).GetChild(0).gameObject.SetActive(true); //�ؽ�Ʈ
        resultPanel.transform.GetChild(0).GetChild(1).gameObject.SetActive(true); //���� �ؽ�Ʈ �̹���
        resultPanel.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);  //���� �ؽ�Ʈ �̹���
        resultPanel.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);  //�ٽ��ϱ� ��ư
        resultPanel.transform.GetChild(0).GetChild(4).gameObject.SetActive(false);  //������ ��ư
        resultPanel.GetComponentInChildren<TextMeshProUGUI>().text ="���� �Ÿ� : "+ _record.ToString("f1") + "km";
        SoundManager.instance.PlayEffectSound("eff_Common_result", 1f);
    }

    public void FailLauncherMission()  //���ּ� �߻� �̼� ����
    {
        gameModeCount = 0;
        resultPanel.SetActive(true);
        resultPanel.transform.GetChild(0).GetChild(0).gameObject.SetActive(false); //�ؽ�Ʈ
        resultPanel.transform.GetChild(0).GetChild(1).gameObject.SetActive(false); //���� �ؽ�Ʈ �̹���
        resultPanel.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);  //���� �ؽ�Ʈ �̹���
        resultPanel.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);  //�ٽ��ϱ� ��ư
        resultPanel.transform.GetChild(0).GetChild(4).gameObject.SetActive(true);  //������ ��ư
        SoundManager.instance.PlayEffectSound("eff_Common_fail", 1f);
    }

    public void ClearRendezvousMission()  //������ �̼� ����
    {
        gameModeCount = 0;
        resultPanel.SetActive(true);
        resultPanel.transform.GetChild(0).GetChild(0).gameObject.SetActive(true); //�ؽ�Ʈ
        resultPanel.transform.GetChild(0).GetChild(1).gameObject.SetActive(true); //���� �ؽ�Ʈ �̹���
        resultPanel.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);  //���� �ؽ�Ʈ �̹���
        resultPanel.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);  //�ٽ��ϱ� ��ư
        resultPanel.transform.GetChild(0).GetChild(4).gameObject.SetActive(false);  //������ ��ư
        resultPanel.GetComponentInChildren<TextMeshProUGUI>().text = "���ּ� ��ŷ�� �����մϴ�.";
    }
        
    public void FailRendezvousMission()  //������ �̼� ����
    {
        gameModeCount = 1;
        resultPanel.SetActive(true);
        resultPanel.transform.GetChild(0).GetChild(0).gameObject.SetActive(false); //�ؽ�Ʈ
        resultPanel.transform.GetChild(0).GetChild(1).gameObject.SetActive(false); //���� �ؽ�Ʈ �̹���
        resultPanel.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);  //���� �ؽ�Ʈ �̹���
        resultPanel.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);  //�ٽ��ϱ� ��ư
        resultPanel.transform.GetChild(0).GetChild(4).gameObject.SetActive(true);  //������ ��ư
    }

    public void ClearDockingMission()  //��ŷ �̼� Ŭ����
    {
        SoundManager.instance.PlayEffectSound("RobotAsseySound", 1f);
        StartCoroutine(DockingStationZoomin());
    }

    IEnumerator DockingStationZoomin()  //��ŷ �̼� Ŭ���� (��ŷ�����̼� �̹��� Ȯ��)
    {
        yield return new WaitForSeconds(1);
        Image _image = dockingStationObject.GetComponent<Image>();
        Color _color = _image.color;

        while(dockingStationObject.transform.localScale.x < 3f)
        {
            dockingStationObject.transform.localScale += Vector3.one * Time.deltaTime * 0.5f;
            yield return null;
        }

        while(_image.color.r > 0f)
        {
            _color.r -= Time.deltaTime;
            _color.g -= Time.deltaTime;
            _color.b -= Time.deltaTime;
            _image.color = _color;
            yield return null;
        }

        lightCtrl.isLight = false;  //��ŷ�����̼� ����Ʈ ����

        //���� ��� �г� Ȱ��ȭ
        gameModeCount = 0;
        resultPanel.SetActive(true);
        resultPanel.transform.GetChild(0).GetChild(0).gameObject.SetActive(false); //�ؽ�Ʈ
        resultPanel.transform.GetChild(0).GetChild(1).gameObject.SetActive(true); //���� �ؽ�Ʈ �̹���
        resultPanel.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);  //���� �ؽ�Ʈ �̹���
        resultPanel.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);  //�ٽ��ϱ� ��ư
        resultPanel.transform.GetChild(0).GetChild(4).gameObject.SetActive(true);  //������ ��ư
        //resultPanel.GetComponentInChildren<TextMeshProUGUI>().text = "���������� ����!";
    }

    public void FailDockingMission()  //��ŷ �̼� ����
    {
        gameModeCount = 2;
        resultPanel.SetActive(true);
        resultPanel.transform.GetChild(0).GetChild(0).gameObject.SetActive(false); //�ؽ�Ʈ
        resultPanel.transform.GetChild(0).GetChild(1).gameObject.SetActive(false); //���� �ؽ�Ʈ �̹���
        resultPanel.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);  //���� �ؽ�Ʈ �̹���
        resultPanel.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);  //�ٽ��ϱ� ��ư
        resultPanel.transform.GetChild(0).GetChild(4).gameObject.SetActive(true);  //������ ��ư
    }    

    #endregion


    #region �Ͻ�����/�簳/����
    public void OnClickPauseButton()  //�����Ͻ�����
    {
        if (!pauseMenuPanel.activeSelf) pauseMenuPanel.SetActive(true);
        gameModeCount = 0;
        Time.timeScale = 0f;
    }

    public void OnClickResumeButton()  //���Ӱ���ϱ�
    {
        if (pauseMenuPanel.activeSelf) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnClickRetryButton()  //���� �����
    {
        StopAllCoroutines();
        if (pauseMenuPanel.activeSelf) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        switch (gameModeCount)
        {
            case 0:
                GameSetup();
                launcherCtrl.RetryGame();
                break;
            case 1:
                RendezvousDockingGameSetup();
                break;
            case 2:
                dockingCtrl.IntoDockingMode();
                break;
        }
    }

    public void OnClickHelpButton()  //����
    {
        if (!helpPanel.activeSelf) helpPanel.SetActive(true);
    }

    public void OnClickExitButton()  //���ӿ��� ������
    {
        Time.timeScale = 1f;
        SoundManager.instance.StopBGMSound();
        StartCoroutine(ChangeSceneToLobby());
    }

    IEnumerator ChangeSceneToLobby()
    {
        GameObject gameObject = Instantiate(Resources.Load<GameObject>("Utils/ChangeSceneCanvas"));
        yield return gameObject.GetComponent<ChangeSceneManager>().FadeOut(1f);
        GlobalData.nextScene = "LobbyScene";
        SceneManager.LoadScene("LoadScene");
    }
    #endregion

}
