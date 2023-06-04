using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ֺ����
/// ������ ����� ���ּ� ��ũ��Ʈ
/// </summary>

public class SpaceDockingShipController : MonoBehaviour
{
    private SpaceDockingGameController dockingGameCtrl;

    [Header("���ӿ�����Ʈ")]
    private Rigidbody2D rb;
    public Transform targetTr;
    public Transform[] shipsTr;  //��ŷ����, ���ּ���ü
    public Transform radarTr;    //���̴�
    public GameObject[] effectParticles;
    private ParticleSystem particleSystem;
    public Transform[] spawnTrArray;

    [Header("���ӵ�����")]
    public float startDelay = 5f;
    public float obstacleTimer = 0.25f;
    float timer = 0.25f;
    private Vector3 dir = Vector3.zero;
    public float moveSpeed = 100f;
    public float rotSpeed = 100f;
    private float controlRotSpeed = 0f;
    public bool isMove = false;
    public bool isDocking = false;
    public float angle;
    public float distance;
    public bool isTryDocking = false;
    int obstacleCount = 0;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        dockingGameCtrl = transform.parent.GetComponent<SpaceDockingGameController>();
        obstacleCount = dockingGameCtrl.objPool.obstaclePrefabs.Length;
        particleSystem = effectParticles[0].GetComponent<ParticleSystem>();
        GetAngle();
    }

    private void Update()
    {
        KeyboardMoveControl();  //Ű���� ���� (��ŷ ��忡���� ���ּ�)

        startDelay -= Time.deltaTime;
        timer -= Time.deltaTime;
        if (timer < 0 && startDelay <0 && !isDocking)
        {
            timer = obstacleTimer;
            GenerateObstacle();
        }
    }


    private void FixedUpdate()
    {
        if (!isDocking)
        {
            if (isMove)
            {
                MoveShip();  //���̽�ƽ ���ۿ� ���� ���ּ� �̵� ����
            }

            if (controlRotSpeed != 0)
            {
                RotateShip(-controlRotSpeed);  //��ư ���ۿ� ���� ���ּ� ȸ�� ����
            }

            isMove = false;
            GetAngle();  //��ŷ ���� Ȯ��
        }
    }

    #region ���ּ� ����

    void KeyboardMoveControl()  //Ű���� ���ּ� �̵� ����
    {
        float moveY = Input.GetAxis("Vertical");
        float moveX = Input.GetAxis("Horizontal");

        if (moveY != 0 || moveX != 0)
        {
            isMove = true;
            dir = new Vector3(moveX, moveY, 0).normalized;
        }
    }

    public void GetMoveValue(Vector2 _dir)  //�̵� ���Ⱚ �Ҵ�
    {
        isMove = true;
        dir = _dir;
    }


    void MoveShip()  //���ּ� �̵�
    {
        rb.velocity = dir * moveSpeed * Time.deltaTime;
    }

    public void GetRotateValue(float _rotValue)  //�¿� ȸ���� �Ҵ�
    {
        controlRotSpeed = _rotValue;
    }

    void RotateShip(float _rotValue)  //�¿�ȸ��
    {
        transform.Rotate(new Vector3(0, 0, _rotValue * rotSpeed * Time.deltaTime));
    }

    public void IntoDockingMode()  //��ŷ ��� ����
    {
        isDocking= true;
        SpaceGameManager.instance.lightCtrl.isLight = true;
        rb.velocity = Vector3.zero;
        this.transform.position = Vector3.zero;

        //SpaceGameManager.instance.cameraCtrl.targetTr = null;
        GameObject.Find("Main Camera").GetComponent<SpaceCameraController>().targetTr = null;

        this.transform.position = new Vector3(0f,-2f,0f);
        this.transform.rotation = Quaternion.identity;
        radarTr.gameObject.SetActive(false);
    }

    void GetAngle()   //��ŷ ���� Ȯ�� - ���̴��� ��ŷ���� ������ ���� ���ϱ�
    {
        //��ŷ �� �Ÿ��� �ޱ��� ����
        //�Ÿ� : 0.5f �̳�
        //���� : 0.3f ~ -0.3f
        //������ �Ÿ��� �� ������ ������ �ÿ� ��ŷ ȿ�� ���

        float value1 = Mathf.Atan((shipsTr[0].position.y - this.transform.position.y) / (shipsTr[0].position.x - this.transform.position.x));
        float value2 = Mathf.Atan((radarTr.position.y - this.transform.position.y) / (radarTr.position.x - this.transform.position.x));

        angle = value1 - value2;
        //Debug.Log("�ޱ� : " + angle);
        distance = Vector2.Distance(shipsTr[0].position, radarTr.position);
        //Debug.Log("�Ÿ� : " + distance);
    }

    #endregion


    #region ���̴�
    private void LateUpdate()
    {
        TargetDirectionUpdate();  //���̴� ���� ������Ʈ
    }

    private void TargetDirectionUpdate()  //���̴� ���� ������Ʈ (������������ ��ġ Ȯ�ο� ���̴�)
    {
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, 20f);
        if (colls.Length > 0)
        {
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].CompareTag("UNIT"))
                {
                    targetTr = colls[i].transform;
                    break;
                }
            }
        }

        if (targetTr != null)  //���� ���̴� ����/��ġ ����
        {
            float angle = Mathf.Atan2(targetTr.position.y - radarTr.position.y, targetTr.position.x - radarTr.position.x) * Mathf.Rad2Deg;
            radarTr.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

            Vector3 radarPos = (targetTr.position - this.transform.position).normalized;
            radarTr.position = radarPos + shipsTr[1].position;
        }
    }
    #endregion


    #region �ݶ��̴�

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDocking) return;

        if (collision.name == "Station" || collision.CompareTag("OBSTACLE")) //���� ���� (���ּ��� ���������忡 ���� ���)
        {
            //���ּ� �̹��� ��Ȱ��ȭ
            for (int i = 0; i < 3; i++)  
            {
                this.transform.GetChild(i).gameObject.SetActive(false);
            }

            //���� ����Ʈ ���
            GameObject explosionEffect = Instantiate(effectParticles[1], transform.position, Quaternion.identity);
            explosionEffect.transform.SetParent(shipsTr[1].transform.parent);
            explosionEffect.transform.localScale = Vector3.one;
            explosionEffect.transform.localPosition = Vector3.zero;
            explosionEffect.SetActive(true);
            isMove = false;

            StartCoroutine(RendezvousFail());  //������ ����
        }

        if (collision.name == "DockPos")  //��ŷ ����Ʈ ȿ�� (��ŷ�� ������ ���°� �Ǹ� ��ŷ ������ ����Ʈ ���)
        {
            if (distance < 0.5f)
            {
                if (angle > -0.3f && angle < 0.3f)
                {
                    dockingGameCtrl.SetToDockingZone(true);
                    effectParticles[0].SetActive(true);
                    if (!particleSystem.isPlaying)
                    {
                        particleSystem.Play();                        
                    }
                    //GameObject hitEffect = Instantiate(effectParticles[0], transform.position, Quaternion.identity);
                    //hitEffect.transform.SetParent(shipsTr[0].transform.parent);
                    //hitEffect.transform.localScale = Vector3.one;
                    //hitEffect.transform.localPosition = new Vector3(0, 0.7f, 0);
                    //hitEffect.SetActive(true);
                }
            }
        }
    }

    IEnumerator RendezvousFail()  //������ ����
    {
        yield return new WaitForSeconds(1);
        SpaceGameManager.instance.FailRendezvousMission();
        Destroy(this.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isDocking) return;

        if (collision.name == "DockPos")  //��ŷ ������ ���� ����ִ� ����
        {
            if (distance < 0.5f)
            {
                if (angle > -0.3f && angle < 0.3f)
                {
                    dockingGameCtrl.isTryDocking = true;
                }
            }
            else
            {
                dockingGameCtrl.isTryDocking = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isDocking) return;

        if (collision.name == "DockPos")  //��ŷ ������ ��������,
        {
            dockingGameCtrl.SetToDockingZone(false);
        }
    }

    #endregion



    #region ��ֹ� ����

    void GenerateObstacle()
    {
        dockingGameCtrl.objPool.GetObstacle(Random.Range(0, obstacleCount), spawnTrArray[Random.Range(0, spawnTrArray.Length)]);
    }

    #endregion


}
