using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotWeaponFireController : MonoBehaviour
{
    [SerializeField] RobotBattleSceneController gameCtrl;
    [SerializeField] RobotWeaponController weaponCtrl;
    public float fireDelay = 0f;
    public float coolTime = 0f;
    public GameObject bulletPrefab;
    [SerializeField] Transform firePosTr;
    [SerializeField] Transform fireResetPosTr;
    public int atkDamage;
    [SerializeField] string weaponName;
    [SerializeField] int weaponType;

    private void Awake()
    {
        WeaponSetup();
        FirePosSetup();
    }

    void WeaponSetup()
    {
        gameCtrl = FindObjectOfType<RobotBattleSceneController>();
        weaponCtrl = this.transform.GetComponent<RobotWeaponController>();

        switch (this.gameObject.name)
        {
            case "AutoGun(Clone)": weaponName = "140002"; weaponType = 1; break;
            case "Cannon(Clone)": weaponName = "140003"; weaponType = 2; break;
        }
    }

    void FirePosSetup()  //총구 위치 셋업
    {
        firePosTr = transform.Find("FirePos").transform;
    }

    private void Start()
    {
        DataSetup();
    }

    void DataSetup()
    {
        if (weaponCtrl != null)
        {
            atkDamage = weaponCtrl.DamageSetup(weaponName);
            coolTime = weaponCtrl.CoolTimeSetup(weaponName);
        }
    }


    private void Update()
    {
        CoolTimeSetup();
        if(Input.GetKeyDown(KeyCode.K)) GunFire();

        if (Input.GetKeyDown(KeyCode.L)) CannonFire();

        if (this.gameObject.layer == LayerMask.NameToLayer("ENEMY") && gameCtrl.isGamePlay) AiAttack();
    }

    void CoolTimeSetup()  //기관총 사용 준비
    {
        if(fireDelay < coolTime)
        {
            fireDelay += Time.deltaTime;
        }
    }

    public void GunFire()   //기관총 사용
    {
        if (this.gameObject.layer == LayerMask.NameToLayer("ENEMY")) return;
        Vector3 _dir = this.transform.position;
        if (weaponType != 1) return;
        if (fireDelay < coolTime || !gameCtrl.isGamePlay) return;  //딜레이타임 부족 또는 게임플레이 중이 아닐 때에는 무시

        GameObject _bullet = Instantiate(bulletPrefab, firePosTr.position, Quaternion.identity);
        _bullet.gameObject.layer = LayerMask.NameToLayer("PLAYER");
        _bullet.GetComponent<RobotBulletController>().atkDamage = atkDamage;
        //_bullet.GetComponent<RobotBulletController>().fireForce = 2000f;
        _bullet.GetComponent<RobotBulletController>().fireDir = new Vector3(firePosTr.position.x - _dir.x, 0f, 0f);
        //Debug.Log("총알의 방향 : " + _bullet.GetComponent<RobotBulletController>().fireDir);
        _bullet.GetComponent<RobotBulletController>().weaponType = RobotBulletController.WeaponType.bullet;
        fireDelay = 0f;
    }


    public void CannonFire()  //박격포 사용
    {
        if (this.gameObject.layer == LayerMask.NameToLayer("ENEMY")) return;
        if (weaponType != 2) return;
        if (fireDelay < coolTime || !gameCtrl.isGamePlay) return;  //딜레이타임 부족 또는 게임플레이 중이 아닐 때에는 무시

        GameObject _cannon = Instantiate(bulletPrefab, firePosTr.position, Quaternion.identity);
        _cannon.gameObject.layer = LayerMask.NameToLayer("PLAYER");
        _cannon.GetComponent<RobotBulletController>().atkDamage = atkDamage;
        //_cannon.GetComponent<RobotBulletController>().fireForce = 100f;
        _cannon.GetComponent<RobotBulletController>().fireDir = CannonDirectSetup();
        _cannon.GetComponent<RobotBulletController>().weaponType = RobotBulletController.WeaponType.cannon;
        fireDelay = 0f;
    }

    Vector3 CannonDirectSetup()
    {
        Vector3 _dir;
        _dir = firePosTr.position - fireResetPosTr.position;
        _dir.Normalize();
        return _dir;
    }


    public void AIGunFire()   // AI 의 기관총 사용
    {
        Vector3 _dir = this.transform.position;
        if (weaponType != 1) return;
        if (fireDelay < coolTime || !gameCtrl.isGamePlay) return;  //딜레이타임 부족 또는 게임플레이 중이 아닐 때에는 무시

        GameObject _bullet = Instantiate(bulletPrefab, firePosTr.position, Quaternion.Euler(0f,0f,180f));
        _bullet.gameObject.layer = this.gameObject.layer;//  LayerMask.NameToLayer("PLAYER");
        _bullet.GetComponent<RobotBulletController>().atkDamage = atkDamage;
        //_bullet.GetComponent<RobotBulletController>().fireForce = 2000f;
        _bullet.GetComponent<RobotBulletController>().fireDir = new Vector3(firePosTr.position.x - _dir.x, 0f, 0f);
        //Debug.Log("총알의 방향 : " + _bullet.GetComponent<RobotBulletController>().fireDir);
        _bullet.GetComponent<RobotBulletController>().weaponType = RobotBulletController.WeaponType.bullet;
        fireDelay = 0f;
    }

    public void AICannonFire()  // AI 의 박격포 사용
    {
        if (weaponType != 2) return;
        if (fireDelay < coolTime || !gameCtrl.isGamePlay) return;  //딜레이타임 부족 또는 게임플레이 중이 아닐 때에는 무시

        GameObject _cannon = Instantiate(bulletPrefab, firePosTr.position, Quaternion.identity);
        _cannon.gameObject.layer = this.gameObject.layer;// LayerMask.NameToLayer("PLAYER");
        _cannon.GetComponent<RobotBulletController>().atkDamage = atkDamage;
        //_cannon.GetComponent<RobotBulletController>().fireForce = 100f;
        _cannon.GetComponent<RobotBulletController>().fireDir = CannonDirectSetup();
        _cannon.GetComponent<RobotBulletController>().weaponType = RobotBulletController.WeaponType.cannon;
        fireDelay = 0f;
    }

    // ai 공격 딜레이
    [SerializeField] float aiDelay = 0.2f;
    void AiAttack()
    {
        aiDelay -= Time.deltaTime;
        if(aiDelay <= 0f)
        {
          if(this.gameObject.name == "AutoGun(Clone)")  AIGunFire();
            if (this.gameObject.name == "Cannon(Clone)") AICannonFire();

            aiDelay = Random.Range(0.2f, 1f);
        }
    }
}
