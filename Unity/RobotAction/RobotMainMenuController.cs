using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RobotMainMenuController : MonoBehaviour
{
    //JobGameSceneManager sceneManager;
    RobotCanvas robotCtrl;
    public Text textJobDescription; //직업 설명 텍스트
    public string strJobDescription;//직업 설명 내용
    public Text textJobDescriptionTitle; //직업 설명 제목 텍스트
    public Text textJobButton;      //직업 버튼 텍스트
    public string strJobButton;     //직업 버튼 내용
    public Text textVideoTitle;     //비디오영상 제목 텍스트
    public string strVideoTitle;    //비디오영상 제목 내용

    public List<Dictionary<string, object>> stMain;  //csv 읽기

    private void Awake()
    {
        //sceneManager = FindObjectOfType<JobGameSceneManager>();
        robotCtrl = FindObjectOfType<RobotCanvas>();
    }

    private void Start()
    {
        stMain = robotCtrl.stMain;
        TextSetup();
    }

    //private void OnEnable()
    //{
    //   RobotCanvas.robotInstance.BGMPlay(0);
    //}


    void TextSetup()  //텍스트 내용 준비
    {
        strJobButton = "SUI010681";
        strJobDescription= "job90028";
        strVideoTitle = "로봇 공학자가 하는 일";
        
        int count = 0;
        for(int i = 0; i < stMain.Count; i++)
        {
           if(strJobButton == stMain[i]["String_ID"].ToString())
            {
                textJobButton.text = stMain[i]["KO"].ToString();
                textJobDescriptionTitle.text = stMain[i]["KO"].ToString();
                count++;
                if (count == 2) break;
            }

            if (strJobDescription == stMain[i]["String_ID"].ToString())
            {
                textJobDescription.text = stMain[i]["KO"].ToString();
                count++;
                if (count == 2) break;
            }
        }
        
        textVideoTitle.text = strVideoTitle.ToString();
    }

    public void PlayVideo()  //비디오 재생
    {
        //GameObject _button = EventSystem.current.currentSelectedGameObject;
        string _videofile = null;
        string _videoTitle = null;

        //_videoTitle = jobVideo[0]; _videofile = jobVideo[0] + ".mp4"; /* sceneManager.PlayVideo("/data/video/FutureCar.mp4", "미래자동차란?");*/

        _videofile = "RobotVideo.mp4";
        _videoTitle = "로봇 공학자가 하는 일";
        //sceneManager.PlayVideo("/video/" + _videofile, _videoTitle);
    }
}
