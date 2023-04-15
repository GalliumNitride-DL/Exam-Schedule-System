using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;
using System;

public class NameLogger : MonoBehaviour
{
    public RowIndicator[] rows;
    public NameIndicator[,] indicators = new NameIndicator[6,7];
    private float suppressionTime;
    public float fuck = 2f;

    private void Awake()
    {
        for (int i = 0; i < rows.Length; i++)
        {
            for (int j = 0; j < rows[i].sons.Length; j++)
            {
                indicators[i,j] = rows[i].sons[j];
            }
        }
        NameIndicator.status = ShowStatus.name;
    }

    public void LogNames(ExamSubject subject)
    {
        foreach (var indicator in indicators)
        {
            indicator.ClearName();
        }

        var IDs = subject.id;
        int r = 0, c = 0;
        //Debug.Log(IDs);
        string[] lineArrays = IDs.Split("\n");
        //Debug.Log(lineArrays[0]);
        for (int i = 0; i < lineArrays.Length; i++)
        {
            string[] nameIDPair = lineArrays[i].Split(",");
            if (nameIDPair.Length != 2) continue;
            //Debug.Log($"{r} and {c}");
            if (indicators[r,c].isNull)
            {
                //indicators[r,c].ConductAnimation();
                c++;
                if (c >= 7) { c = 0; r++; }
                if (r >= 6) { break; }
            }
            if (nameIDPair.Length == 2)
            {
                indicators[r,c].name1 = nameIDPair[0];
                indicators[r,c].examID = nameIDPair[1];
            }
            
            //indicators[r,c].ConductAnimation();
            c++;
            if (c >= 7) { c = 0; r++; }
            if (r >= 6) { break; }

            
            
            //await UniTask.Delay(TimeSpan.FromSeconds(0.2));
        }

        foreach (var indicator in indicators)
        {
            indicator.ConductAnimation();
        }

    }

    public async void Update()
    {
        if (Time.time < suppressionTime) return;
        suppressionTime = Time.time + fuck;
        ZhengHuo();
    }

    private async void ZhengHuo()
    {
        //int az = UnityEngine.Random.Range(1,6);
        NameIndicator.status = NameIndicator.status == ShowStatus.name ? ShowStatus.ID : ShowStatus.name;
        // switch (az)
        // {
        //     case 1:
                for (int i = 0; i < 6; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        indicators[i,j].ConductAnimation();
                        await UniTask.Delay(TimeSpan.FromSeconds(0.075));
                    }
                }
            //     break;
            // case 2:
            // case 3:
            // default:
            // for (int i = 0; i < 6; i++)
            //     {
            //         for (int j = 0; j < 7; j++)
            //         {
            //             indicators[i,j].ConductAnimation();
            //             await UniTask.Delay(TimeSpan.FromSeconds(0.05));
            //         }
            //     }
            // break;
        
    }
}
