using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum ShowStatus
{
    name,
    ID
}

public class NameIndicator : MonoBehaviour
{
    public string name1 = "N/A";
    public string examID = "0000000";
    public bool isNull = false;
    [HideInInspector] public Text text;
    private Vector3 originalPos;
    public static float animOffset = 20;
    public static float duration = 0.75f;
    public static ShowStatus status;

    private void Awake()
    {
        text = this.GetComponent<Text>();
        originalPos = this.transform.localPosition;
    }

    public void ClearName()
    {
        name1 = "N/A";
        examID = "0000000";
    }

    public void ConductAnimation()
    {
        
        
            text.DOFade(0f, duration).SetEase(Ease.InQuad);
            this.transform.DOLocalMoveY(this.transform.localPosition.y - animOffset, duration).SetEase(Ease.InQuad).OnComplete(() =>
            {
                if (status == ShowStatus.name) { text.text = name1; }
                else { text.text = examID; }
                text.DOFade(1f, duration).SetEase(Ease.OutQuad);
                this.transform.localPosition = originalPos + Vector3.up * animOffset;
                this.transform.DOLocalMoveY(originalPos.y, duration).SetEase(Ease.OutQuad);
            });
        
    }
}
