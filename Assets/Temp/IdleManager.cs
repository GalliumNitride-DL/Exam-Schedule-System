using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[Serializable]
public class ExamSubject
{
    //public string name;
    public string fullName;
    public int date;
    public int startHour, startMinute;
    public int endHour, endMinute;
    public int indicatorY;
    [Obsolete]
    public TextAsset IDs;
    [Multiline]
    public string id;

    public int isBeforeOrAfter
    {
        get
        {
            var currentTime = IdleManager.currentTime;
            if (currentTime.Day < date) return -1;
            if (currentTime.Day > date) return 1;
            if (currentTime.Hour < startHour) return -1;
            if (currentTime.Hour == startHour && currentTime.Minute < startMinute) return -1;
            if (currentTime.Hour > endHour) return 1;
            if (currentTime.Hour == endHour && currentTime.Minute > endMinute) return 1;
            // if (currentTime.Minute < startMinute) return -1;
            // if (currentTime.Minute > endMinute) return 1;
            //if (currentTime.Second > 0) return 1;
            return 0;
        }
    }

    public int HoursFromStart(DateTime currentTime)
    {
        //if (MinutesFromStart(currentTime) % 60 == 0) return MinutesFromStart(currentTime) / 60 - 1;
        return MinutesFromStart(currentTime) / 60;
    }

    public int MinutesFromStart(DateTime currentTime)
    {
        return startHour * 60 + startMinute - (currentTime.Hour * 60 + currentTime.Minute) - 1;
    }

    public int HoursFromEnd(DateTime currentTime)
    {
        //if (MinutesFromEnd(currentTime) % 60 == 0) return MinutesFromEnd(currentTime) / 60 - 1;
        return MinutesFromEnd(currentTime) / 60;
    }

    public int MinutesFromEnd(DateTime currentTime)
    {
        var time = endHour * 60 + endMinute - (currentTime.Hour * 60 + currentTime.Minute) - 1;
        //if (time < 0) return time + 1;
        return time;
    }

    public int CompareTo(ExamSubject other)
    {
        if (other.date < date) return 1;
        if (other.date > date) return -1;
        if (other.startHour < startHour) return 1;
        if (other.startHour > startHour) return -1;
        if (other.startHour == startHour && other.startMinute < startMinute) return 1;
        //if (currentTime.Hour > endHour) return 1;
        //if (currentTime.Hour == endHour && currentTime.Minute > endMinute) return 1;
        // if (currentTime.Minute < startMinute) return -1;
        // if (currentTime.Minute > endMinute) return 1;
        //if (currentTime.Second > 0) return 1;
        return 0;
    }
}

public class IdleManager : MonoBehaviour
{
    //public static IdleManager Instance;
    public static ExamSubject CurrentSubject; 
    public ExamSubject[] subjects;
    public Text idleTimeIndicator, idleMinutesIndicator;
    public Text examTimeIndicator, examMinutesIndicator;
    public Text[] subjectListeners;
    public Text BasicInfoHolder;
    public GameObject[] IDLE, EXAM;
    public Image COVER, cover2;
    public int delayMinutes = 2;
    public int duration = 1;
    public int startIndicatorY, perRowHeight;
    //public RowIndicator[] rows;
    public NameLogger logger;
    public Transform indicator;

    int index = 0;
    internal static DateTime currentTime = DateTime.MinValue;


    private void Awake()
    {
        Application.targetFrameRate = 30;
        currentTime = DateTime.Now;
        //Instance = this;
        Init();

        index = 0;
        do
        {
            CurrentSubject = subjects[index];
            Debug.Log($"Check {CurrentSubject.fullName}'s isBefore, got {CurrentSubject.isBeforeOrAfter}");
            index++;
        }
        while (CurrentSubject.isBeforeOrAfter == 1 && index < subjects.Length);
        index--;
        //examNameIndicator.text = "当前科目：" + CurrentSubject.fullName;
        if (CurrentSubject.isBeforeOrAfter == 0) { EnterExam(true); }
        else { EnterExam(false); }
        logger.LogNames(CurrentSubject);
        // for (int i = 0; i < Display.displays.Length; i++)
        // {
        //     //开启存在的屏幕显示，激活显示器
        //     Display.displays[i].Activate();
        //     Screen.SetResolution(Display.displays[i].renderingWidth, Display.displays[i].renderingHeight, true);
        // }
    }

    private void Init()
    {
        BasicInfoHolder.text = DataLoader.LoadBasicInfo();
        subjects =  DataLoader.LoadSubjects(startIndicatorY, perRowHeight, BasicInfoHolder.text);
    }

    void OnDisable()
    {
        this.enabled = true;
    }

    private void Update()
    {
        currentTime = DateTime.Now;
        // if (CurrentSubject.isBeforeOrAfter == 1 && currentTime.Minute - CurrentSubject.endMinute >= delayMinutes && CurrentSubject.date == currentTime.Day) //更新下一科目
        // {
        //     // index++;
        //     // CurrentSubject = subjects[index];
        // }
        if (CurrentSubject.isBeforeOrAfter == -1 && (CurrentSubject.MinutesFromStart(currentTime) >= delayMinutes || CurrentSubject.date != currentTime.Day)) //更新距离开考时间
        {
            // string time = ":";
            // time = currentTime.Hour.ToString() + time; if (currentTime.Hour < 10) time = "0" + time;
            // if (currentTime.Minute < 10) time = time + "0"; time = time + currentTime.Minute.ToString(); 
            // idleTimeIndicator.text = "当前时间：" + time;

            // if (CurrentSubject.date != currentTime.Day)
            //     idleMinutesIndicator.text = "距离开考：NaN";
            // else if (CurrentSubject.MinutesFromStart(currentTime) < 60)
            //     idleMinutesIndicator.text = $"距离开考：{CurrentSubject.MinutesFromStart(currentTime)}min {59 - currentTime.Second}s";
            
            // else
            //     idleMinutesIndicator.text = $"距离开考：{CurrentSubject.HoursFromStart(currentTime)}h {CurrentSubject.MinutesFromStart(currentTime) % 60}min";

            if (CurrentSubject.MinutesFromStart(currentTime) <= delayMinutes && currentTime.Second > 59 - duration && CurrentSubject.date == currentTime.Day) //开考！
            {
                EnterExam(true);
            }
        }
        else if (CurrentSubject.isBeforeOrAfter == 1 && CurrentSubject.MinutesFromEnd(currentTime) < - delayMinutes) //全部考完
        {
            // string time = ":";
            // time = currentTime.Hour.ToString() + time; if (currentTime.Hour < 10) time = "0" + time;
            // if (currentTime.Minute < 10) time = time + "0"; time = time + currentTime.Minute.ToString(); 
            // examTimeIndicator.text = "当前时间：" + time;

            idleMinutesIndicator.text = "距离开考：NaN";
        }
        else //正在考试状态
        {
            if (CurrentSubject.isBeforeOrAfter == -1 && CurrentSubject.MinutesFromStart(currentTime) <= delayMinutes && currentTime.Second > 59 - duration && CurrentSubject.date == currentTime.Day && EXAM[0].activeSelf == false) //开考！
            {
                EnterExam(true);
            }
            // string time = ":";
            // time = currentTime.Hour.ToString() + time; if (currentTime.Hour < 10) time = "0" + time;
            // if (currentTime.Minute < 10) time = time + "0"; time = time + currentTime.Minute.ToString(); 
            // examTimeIndicator.text = "当前时间：" + time;

            // if (CurrentSubject.MinutesFromEnd(currentTime) < 20)
            //     if (CurrentSubject.MinutesFromEnd(currentTime) < 0)
            //         examMinutesIndicator.text = $"距离结束：-{Mathf.Abs(CurrentSubject.MinutesFromEnd(currentTime) + 1)}min {currentTime.Second}s";
            //     else
            //         examMinutesIndicator.text = $"距离结束：{CurrentSubject.MinutesFromEnd(currentTime)}min {59 - currentTime.Second}s";
            // else
            //     examMinutesIndicator.text = $"距离结束：{CurrentSubject.HoursFromEnd(currentTime)}h {CurrentSubject.MinutesFromEnd(currentTime) % 60}min";
            
            if (CurrentSubject.MinutesFromEnd(currentTime) <= -delayMinutes && currentTime.Second > 59 - duration && CurrentSubject.date == currentTime.Day)
            {
                EnterExam(false);
            }
        }
        foreach (var text in subjectListeners)
        {
            text.text = "当前科目：" + CurrentSubject.fullName;
        }

        //Update Time
        //Before Exam
        string time = ":";
        time = currentTime.Hour.ToString() + time; if (currentTime.Hour < 10) time = "0" + time;
        if (currentTime.Minute < 10) time = time + "0"; time = time + currentTime.Minute.ToString(); 
        idleTimeIndicator.text = examTimeIndicator.text = "当前时间：" + time;
        if (CurrentSubject.date != currentTime.Day || CurrentSubject.MinutesFromEnd(currentTime) < 0)
            idleMinutesIndicator.text = "距离开考：NaN";
        else if (CurrentSubject.MinutesFromStart(currentTime) < 60)
            idleMinutesIndicator.text = $"距离开考：{CurrentSubject.MinutesFromStart(currentTime)}min {59 - currentTime.Second}s";
        else
            idleMinutesIndicator.text = $"距离开考：{CurrentSubject.HoursFromStart(currentTime)}h {CurrentSubject.MinutesFromStart(currentTime) % 60}min";

        //In Exam
        Debug.Log(CurrentSubject.MinutesFromEnd(currentTime));
        if (CurrentSubject.MinutesFromEnd(currentTime) < 20)
            if (CurrentSubject.MinutesFromEnd(currentTime) < 0)
                examMinutesIndicator.text = $"距离结束：-{Mathf.Abs(CurrentSubject.MinutesFromEnd(currentTime) + 1)}min {currentTime.Second}s";
            else
                examMinutesIndicator.text = $"距离结束：{CurrentSubject.MinutesFromEnd(currentTime)}min {59 - currentTime.Second}s";
        else
            examMinutesIndicator.text = $"距离结束：{CurrentSubject.HoursFromEnd(currentTime)}h {CurrentSubject.MinutesFromEnd(currentTime) % 60}min";



        //Key Commands
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (index < subjects.Length - 1)
            {
                index++;
                CurrentSubject = subjects[index];
            }
            EnterExam(CurrentSubject.isBeforeOrAfter == 1);
        }
    }

    private void EnterExam(bool enter)
    {
        cover2.DOFade(1f, duration);
        COVER.DOFade(1f, duration).OnComplete(() =>
        {
            foreach (var idle in IDLE) { idle.SetActive(!enter); }
            foreach (var exam in EXAM) { exam.SetActive(enter); }
            if (CurrentSubject.isBeforeOrAfter == 1 && index < subjects.Length - 1)
            {
                index++;
                CurrentSubject = subjects[index];
            }
            logger.LogNames(CurrentSubject);
            indicator.DOLocalMoveY(CurrentSubject.indicatorY, duration);
            COVER.DOFade(0f, duration);
            cover2.DOFade(0f, duration);
        });
    }

}
