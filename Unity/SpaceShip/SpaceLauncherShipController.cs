using System.Collections;
using UnityEngine;

/// <summary>
/// ���ֺ����
/// �߻� ����� ���ּ� ��Ʈ�ѽ�ũ��Ʈ
/// </summary>

public class SpaceLauncherShipController : MonoBehaviour
{
    private SpaceLauncherGameController launcherCtrl;

    [Header("���ӿ�����Ʈ")]
    private Transform tr;
    private Transform targetTr;
    public GameObject[] shipParts;
    // 0 = ����3�� Ʈ������ƼŬ, 1 = ����3�� �Ҳ� ��ƼŬ, 2 = ����2�� �Ҳ� ��ƼŬ, 3 = ����1�� �Ҳ� ��ƼŬ
    public GameObject[] fireParticles;  //������ ��ƼŬ

    [Header("���ӵ�����")]
    private Vector3 targetPos= Vector3.zero;
    private Vector3 finishPos = Vector3.zero;
    public float moveSpeed = 0.1f;
    public bool[] isLaunchLevel = new bool[3];


    private void Start()
    {
        launcherCtrl = transform.parent.GetComponent<SpaceLauncherGameController>();
        SpaceShipSetup();
    }

    void SpaceShipSetup()  //���ּ� ������ �¾�
    {
        tr = transform;
        targetTr = transform.parent.transform;
        targetPos = new Vector3(targetTr.position.x, (targetTr.position.y - 1f), targetTr.position.z);
        finishPos = new Vector3(targetTr.position.x, (targetTr.position.y + 8f), targetTr.position.z);

        for (int i = 0; i < fireParticles.Length; i++)  //�Ҳ���ƼŬ ��Ȱ��ȭ
        {
            fireParticles[i].SetActive(false);
        }

        for (int i = 0; i < isLaunchLevel.Length; i++)  //���ּ� �и� �ܰ�
        {
            isLaunchLevel[i] = false;
        }
    }


    #region ���ּ� �߻�
    public void LaunchAction()  //�߻� ����
    {
        StartCoroutine(ShakeAction());
    }

    IEnumerator ShakeAction()  //���ּ��� ��鸲
    {
        Vector3 resetPos = transform.position;
        SoundManager.instance.PlayEffectSoundLoop("eff_Space_fly", 1f);

        fireParticles[0].SetActive(true);

        //���ּ� ���� ȿ��
        float coolTime = 3f;
        while (coolTime > 0)
        {
            tr.position = Random.insideUnitSphere * 0.02f + resetPos;
            coolTime -= Time.deltaTime;
            yield return null;
        }

        SoundManager.instance.PlayEffectSound("eff_Space_fire", 1f);
        transform.position = resetPos;
        fireParticles[1].SetActive(true);

        launcherCtrl.isBgMove= true;
        isLaunchLevel[0] = true;
        yield return new WaitForSeconds(8);

        //���ּ� ��ġ �̵�
        while (transform.position != targetPos)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, targetTr.position, moveSpeed * Time.deltaTime);

            if (this.transform.position.y - targetPos.y > -0.2f) break;
            yield return null;
        }
        fireParticles[0].SetActive(false);

    }

    #endregion


    #region ���ּ� �и� ����/����

    public void SeparatePart1()  //���ּ� �Ϻ�ü1 �и�
    {

        if (isLaunchLevel[0] && !isLaunchLevel[1])
        {
            shipParts[2].GetComponent<Rigidbody2D>().gravityScale = 1;
            fireParticles[1].SetActive(false);
            fireParticles[2].SetActive(true);
            isLaunchLevel[1] = true;
            launcherCtrl.separateButtons[0].interactable = false;
        }
        SoundManager.instance.PlayEffectSound("eff_Space_divide", 1f);
    }

    public void SeparatePart2()  //���ּ� �Ϻ�ü2 �и�
    {
        if (isLaunchLevel[1] && !isLaunchLevel[2])
        {
            shipParts[1].GetComponent<Rigidbody2D>().gravityScale = 1;
            fireParticles[2].SetActive(false);
            fireParticles[3].SetActive(true);
            isLaunchLevel[2] = true;
            launcherCtrl.separateButtons[1].interactable = false;
        }
        SoundManager.instance.PlayEffectSound("eff_Space_divide", 1f);
    }


    public void FinishMove()  //�̼� ���� �� ���ּ� ȭ�� ������ ������
    {
        StartCoroutine(ShipFinishMove());
    }

    IEnumerator ShipFinishMove()
    {
        while (transform.position.y < 8f)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, finishPos, Time.deltaTime);

            if (this.transform.position.y > 7f) break;
            yield return null;
        }

        //��ŷ �������� �̵�
        yield return new WaitForSeconds(2f);
        SpaceGameManager.instance.RendezvousDockingGameSetup();
        Destroy(this.gameObject);
    }

    public void FailSeparateParts()  //���ּ� �Ϻ�ü �и� ����
    {
        StopAllCoroutines();
        SoundManager.instance.StopEffSoundLoop();
        StartCoroutine(FailPanelOn());
    }

    IEnumerator FailPanelOn()  //���� ���â Ȱ��ȭ
    {
        yield return new WaitForSeconds(1);
        //�Ҳ���ƼŬ ��Ȱ��ȭ
        for (int i = 0; i < fireParticles.Length; i++)
        {
            fireParticles[i].SetActive(false);
        }
        
        yield return new WaitForSeconds(1);
        //���ּ� ������Ʈ �߷� ����
        this.transform.GetComponent<Rigidbody2D>().gravityScale = 1;
        for(int i = 0; i < shipParts.Length; i++)
        {
            shipParts[i].GetComponent<Rigidbody2D>().gravityScale = 1;
        }

        //ȭ�� �Ʒ��� �������� ���� ���â Ȱ��ȭ
        while (this.transform.position.y > -8f)
        {
            this.transform.Rotate(Vector3.forward, 90f * Time.deltaTime);
            yield return null;
        }

        SpaceGameManager.instance.FailLauncherMission();
    }

    #endregion

    private void OnDisable()  //������Ʈ�� ��Ȱ��ȭ�� �� �ڷ�ƾ ���߱�
    {
        StopAllCoroutines();
    }
}
