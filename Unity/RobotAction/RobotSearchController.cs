using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RobotSearchController : MonoBehaviour
{
    [SerializeField] TMP_Text searchingText;
    [SerializeField] float timer = 0f;
    [SerializeField] Image radar;
    [SerializeField] float rot = 0f;
    private void Update()
    {
        if (this.gameObject.activeSelf)
        {
            timer += Time.deltaTime;
            if(timer > 0f)
            {
                searchingText.text = "상대 검색 중";
            }
            if (timer > 0.5f)
            {
                searchingText.text = "상대 검색 중.";
            }
            if (timer > 1f)
            {
                searchingText.text = "상대 검색 중..";
            }
            if (timer > 1.5f)
            {
                searchingText.text = "상대 검색 중...";
            }
            if (timer > 2f)
            {
                timer = 0f;
            }
            rot -= (Time.deltaTime * 60f);
            radar.transform.localRotation = Quaternion.Euler(0f, 0f, rot);

        }
    }
}
