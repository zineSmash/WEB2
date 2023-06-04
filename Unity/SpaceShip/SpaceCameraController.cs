using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ֺ����
/// ī�޶� ��ũ��Ʈ
/// ������ ��忡�� ���ּ��� ���� �̵��ϴ� ��ũ��Ʈ
/// �ٸ� ��忡���� ���߾����� ��ġ ����
/// </summary>

public class SpaceCameraController : MonoBehaviour
{
    GameMode gameMode;

    public Transform targetTr;
    public float verticalRange = 15f;
    public float horizontalRange = 20f;

    private void LateUpdate()
    {
        if(targetTr != null && gameMode == GameMode.DockingMode)
        {
            transform.position = new Vector3(targetTr.position.x, targetTr.position.y, transform.position.z);
            Vector3 _cameraPos = transform.position;
            _cameraPos.x = Mathf.Clamp(_cameraPos.x, -horizontalRange, horizontalRange);
            _cameraPos.y = Mathf.Clamp(_cameraPos.y, -verticalRange, verticalRange);

            transform.position = _cameraPos;
        }

        if(gameMode == GameMode.LauncherMode)
        {
            transform.position = new Vector3(0f, 0f, transform.position.z);
        }
    }

    public void GetGameMode(GameMode _mode, Transform _target)
    {
        gameMode = _mode;
        targetTr = _target;
    }
}
