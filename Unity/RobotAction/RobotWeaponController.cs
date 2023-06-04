using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotWeaponController : MonoBehaviour
{
    //무기의 기본 데이터를 초기화하는 스크립트
    //공격력, 쿨타임 등 설정
    //각 무기에 본 스크립트를 컴포넌트로 할당해줄 것

    public List<Dictionary<string, object>> dataTable;  //csv 읽기
    //public TextAsset textAsset;
    RobotCanvas robotCanvas;

    private void Awake()
    {
        //dataTable = CSVReader.Read("gMiniGame/DataTable/RobotPartsData_csv");
        //dataTable = CSVReader.Read(textAsset);
        
    }

    private void Start()
    {
        robotCanvas = FindObjectOfType<RobotCanvas>();

        dataTable = robotCanvas.robotData;
    }

    public int DamageSetup(string _weaponName)
    {
        int _damage = 0;

        for(int i = 0; i < dataTable.Count; i++)
        {
            if(dataTable[i]["Parts_ID"].ToString() == _weaponName)
            {
                _damage = int.Parse(dataTable[i]["add_Atk"].ToString());
            }
        }



        return _damage;
    }

    public float CoolTimeSetup(string _weaponName)
    {
        float _delay = 0;

        for (int i = 0; i < dataTable.Count; i++)
        {
            if (dataTable[i]["Parts_ID"].ToString() == _weaponName)
            {
                _delay = float.Parse(dataTable[i]["add_ASpd"].ToString()) * 0.001f;
            }
        }

        return _delay;
    }
}
