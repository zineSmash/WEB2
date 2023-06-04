using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ֺ���� ��ֹ� ������ƮǮ�� ��ũ��Ʈ
/// ��Ʃ�� ����Ż ����
/// https://www.youtube.com/watch?v=A7mfPH8jyBE
/// </summary>
public class SpaceObjectPool : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;
    List<GameObject>[] obstaclePools;

    List<GameObject> childList = new List<GameObject>();

    private void Awake()
    {
        obstaclePools = new List<GameObject>[obstaclePrefabs.Length];
        for(int i=0;i<obstaclePools.Length;i++)
        {
            obstaclePools[i] = new List<GameObject>();
        }
    }

    public GameObject GetObstacle(int index, Transform tr)
    {
        GameObject obstacle = null;

        foreach(GameObject obstaclePool in obstaclePools[index])
        {
            if (!obstaclePool.activeSelf)
            {
                obstacle = obstaclePool;
                obstacle.SetActive(true);
                break;
            }
        }

        //Ȱ��ȭ�� �� �ִ� ������Ʈ�� ���� ��� ���� �����ϰ� Ǯ ����Ʈ�� �߰��ϱ�
        if (!obstacle)  //GameObject �� null ���� �ƴ��� ������ ! �ε� Ȯ�� ���� (!obstacle ������Ʈ�� null �̶��.. �̶�� ��)
        {
            obstacle = Instantiate(obstaclePrefabs[index], transform);
            obstaclePools[index].Add(obstacle);
        }

        return obstacle;
    }
}
