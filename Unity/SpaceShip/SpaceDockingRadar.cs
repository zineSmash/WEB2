using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���ֺ����
/// ��ŷ ����� ��ŷ ���̴� ��ũ��Ʈ
/// ���ּ��� �������� ������ �� ��ŷ ��ư�� ������ ���̴��� Ȱ��ȭ�Ǿ�
/// ��ŷ�� ���� ���� ����
/// </summary>

public class SpaceDockingRadar : MonoBehaviour
{
    [Header("��ũ��Ʈ��Ʈ�ѷ�")]
    public SpaceDockingMoveRadar moveRadar;

    [Header("���ӿ�����Ʈ")]
    public Rigidbody2D radarRb;
    public Image dockingGaugeBar;

    [Header("���ӵ�����")]
    public float moveSpeed = 2f;
    public float controlPower = 60f;
    public float moveXRange = 2.5f;
    public float moveYRange = 2f;
    private Vector2[] movePosition= new Vector2[6];
    public Vector2 targetPos= Vector2.zero;
    public Vector2 controlMoveDirection = Vector2.zero;
    private float coolTime = 3f;
    public float radarMoveCoolTime = 5f;


    private void Start()
    {
        movePosition[0] = new Vector2(moveXRange, (-moveYRange+1));
        movePosition[1] = new Vector2(-moveXRange, moveYRange);
        movePosition[2] = new Vector2(moveXRange, moveYRange);
        movePosition[3] = new Vector2(-moveXRange, (-moveYRange+1));
        movePosition[4] = new Vector2(-moveXRange, 0f);
        movePosition[5] = new Vector2(moveXRange, 0f);
        radarRb.drag = 1;
        TargetPositionSetup();
    }

    void TargetPositionSetup()   //Ÿ�� ������ ����
    {
        int randomIndex = Random.Range(0, 6);
        targetPos = movePosition[randomIndex];
    }

    private void FixedUpdate()
    {
        if (moveRadar.isMove)  //Ÿ���������� ������ ���¿��� ���̴��� isMove = true �� ���
        {
            if ((Vector2)moveRadar.transform.position != targetPos)
            {
                moveRadar.transform.position = Vector2.MoveTowards(moveRadar.transform.position, targetPos, moveSpeed * Time.deltaTime);
            }
            else
            {
                TargetPositionSetup();
            }

            if (((Vector2)moveRadar.transform.position - targetPos).magnitude <= 0.1f)
            {
                TargetPositionSetup();
            }

            MoveRadar();  //���̽�ƽ���� ���̴� ����
        }

        //Ÿ���������� ��� ��� ���Ⱚ �ӵ��� �ʱ�ȭ
        if(moveRadar.transform.position.x > moveXRange || moveRadar.transform.position.x < -moveXRange)
        {
            controlMoveDirection = Vector2.zero;
            radarRb.velocity = Vector2.zero;
            TargetPositionSetup();
        }
        if (moveRadar.transform.position.y > moveYRange || moveRadar.transform.position.y < -moveYRange)
        {
            controlMoveDirection = Vector2.zero;
            radarRb.velocity = Vector2.zero;
            TargetPositionSetup();
        }
    }

    public void GetMoveDirection(Vector2 _dir)  //���̴� ���� �̵� ����
    {
        //�������ᰡ �Ǹ� ���̴� �������� �ʵ���
        if (!moveRadar.isMove)
        {
            controlMoveDirection = Vector2.zero;
            radarRb.velocity = Vector2.zero; 
            return;
        }

        //��ƽ ���ۿ� ���� ��ġ �̵�
        controlMoveDirection = _dir;
        coolTime -= Time.deltaTime;
        if (coolTime < 0)
        {
            TargetPositionSetup();
            coolTime = radarMoveCoolTime;
        }
    }


    void MoveRadar()  //���̴� �̵�
    {
        radarRb.velocity = controlMoveDirection * (moveSpeed* controlPower) * Time.deltaTime;
    }


    public void SetDockingGauge(float _currentValue, float _maxValue)  //��ŷ������ ����
    {
        dockingGaugeBar.fillAmount = _currentValue / _maxValue;
    }

}
