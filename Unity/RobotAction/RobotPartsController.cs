using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotPartsController : MonoBehaviour
{
    [SerializeField] Transform parentTr;   //부모가 될 트랜스폼

    public void ConnectBodySetup(Transform _tr)
    {
        parentTr = _tr;
        if (this.transform.GetComponent<RelativeJoint2D>() != null)
        {
            //this.transform.GetComponent<RelativeJointController>().ConnectBodySetup(parentTr.GetComponent<Rigidbody2D>());
            this.transform.GetComponent<RelativeJoint2D>().connectedBody = parentTr.GetComponent<Rigidbody2D>();
            this.transform.GetComponent<RelativeJoint2D>().enabled = true;
        }

        if (this.transform.GetComponent<WheelJoint2D>() != null)
        {
            this.transform.GetComponent<WheelJoint2D>().connectedBody = parentTr.parent.GetComponent<Rigidbody2D>();
            this.transform.GetComponent<WheelJoint2D>().enabled = true;
            if (this.transform.GetComponent<WheelJointController>() != null) this.transform.GetComponent<WheelJointController>().robotTr = _tr.parent;

        }
    }


    public string weaponName;

    private void Awake()
    {
        WeaponNameSetup();
    }

    void WeaponNameSetup()
    {
        switch (this.gameObject.name)
        {
            case "Armor02(Clone)": weaponName = "110003"; break;
            case "Armor03(Clone)": weaponName = "110002"; break;
            case "Armor04(Clone)": weaponName = "110001"; break;
            case "AutoGun(Clone)": weaponName = "140002"; break;
            case "Cannon(Clone)": weaponName = "140003"; break;
            case "Chainsaw(Clone)": weaponName = "140001"; break;
            case "WheelS(Clone)": weaponName = "130001"; break;
            case "WheelL(Clone)": weaponName = "130002"; break;
            case "BodyFrame": weaponName = "110000"; break;
        }
    }
}
