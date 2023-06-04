using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotUICanvasController : MonoBehaviour
{
    public static RobotUICanvasController uiCtrl;
    [SerializeField] RobotBattleSceneController gameCtrl;

    [SerializeField] Image playerHpGauge;
    [SerializeField] Image enemyHpGauge;

    public int maxPlayerHp;
    public int currentPlayerHp;
    public int maxEnemyHp;
    public int currentEnemyHp;

    [SerializeField] RobotHealthController[] _allHpCtrl;

    [SerializeField] GameObject cannonBtn;
    [SerializeField] GameObject autogunBtn;


    private void Awake()
    {
        
    }

    private void Start()
    {
        maxEnemyHp = 0;
        maxPlayerHp = 0;
        if (uiCtrl == null) uiCtrl = this;    //UI캔버스 컨트롤러 인스턴스화
        else if (uiCtrl != this) uiCtrl = this;

        GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        GetComponent<Canvas>().worldCamera = Camera.main;
        this.GetComponent<Canvas>().sortingLayerName = "UI";
    }

    private void OnEnable()
    {
        maxEnemyHp = 0;
        maxPlayerHp = 0; 
        //HealthBarSetup();
    }

    public void AttackButtonSetup()
    {
        if (!cannonBtn.activeSelf) cannonBtn.SetActive(true);
        if (!autogunBtn.activeSelf) autogunBtn.SetActive(true);
    }


    public void HealthBarSetup()
    {
        _allHpCtrl = FindObjectsOfType<RobotHealthController>();


        //foreach(RobotHealthController h in _allHpCtrl)
        //{
        //    if(h.gameObject.layer == LayerMask.NameToLayer("PLAYER"))
        //    {
        //        maxPlayerHp += h.maxHp;
        //    }
        //}


        foreach (RobotHealthController h in _allHpCtrl)
        {
            if (h.gameObject.layer == LayerMask.NameToLayer("ENEMY"))
            {
                maxEnemyHp += h.maxHp;
            }
        }

        maxPlayerHp = gameCtrl.healthPoint;

        currentPlayerHp = maxPlayerHp;
        currentEnemyHp = maxEnemyHp;
        //Debug.Log("플레이어체력 : " + currentPlayerHp + " 적체력 : " + currentEnemyHp);
        playerHpGauge.fillAmount = (float)currentPlayerHp / maxPlayerHp;
        enemyHpGauge.fillAmount = (float)currentEnemyHp / maxEnemyHp;
    }

    public void ChangeHP(LayerMask _layer, int _damage)
    {
        int _prevHp = 0;
        if (_layer == LayerMask.NameToLayer("PLAYER"))
        {
            _prevHp = currentPlayerHp;
            currentPlayerHp -= _damage;
            StartCoroutine(DescreaseHP(_prevHp, currentPlayerHp, _layer));
        }
        else if (_layer == LayerMask.NameToLayer("ENEMY"))
        {
            _prevHp = currentEnemyHp;
            currentEnemyHp -= _damage;
            StartCoroutine(DescreaseHP(_prevHp, currentEnemyHp, _layer));
        }
        //Debug.Log("플레이어체력 : " + currentPlayerHp + " 적체력 : " + currentEnemyHp);
    }

    IEnumerator DescreaseHP(int _prevHp, int _currenHp, LayerMask _layer)
    {
        while (_prevHp >= _currenHp)
        {
            _prevHp -= 10;
            if (_prevHp <= _currenHp) _prevHp = _currenHp;

            if (_layer == LayerMask.NameToLayer("PLAYER")) playerHpGauge.fillAmount = (float)_prevHp / maxPlayerHp;
            else if (_layer == LayerMask.NameToLayer("ENEMY")) enemyHpGauge.fillAmount = (float)_prevHp / maxEnemyHp;

            yield return new WaitForFixedUpdate();
        }
    }
}
