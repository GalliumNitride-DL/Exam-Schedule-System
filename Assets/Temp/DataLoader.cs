using System.IO;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;

public static class DataLoader
{
    private const string BasicInfoFile = "BasicInfo.txt";
    public static string LoadBasicInfo()
    {
        var path = "file:///" + Application.streamingAssetsPath + '/' + BasicInfoFile;
        var request = UnityWebRequest.Get(path);
        var operation = request.SendWebRequest();
        while (!operation.isDone);

        if (request.result != UnityWebRequest.Result.Success) return string.Empty;

        else return request.downloadHandler.text;
    }

    public static ExamSubject[] LoadSubjects(int startIndicatorY, int perRowHeight, string basicInfo)
    {
        var path = Application.streamingAssetsPath;
        var files = new DirectoryInfo(path).GetFiles("*.txt", SearchOption.AllDirectories);
        var output = new List<ExamSubject>();

        foreach (var file in files)
        {
            if (file.Name == BasicInfoFile) continue;
            var data = File.ReadAllText(path + '/' + file.Name);
            var subject = new ExamSubject();

            subject.id = data;
            
            var lineArrays = data.Split("\n");
            //Apply Basic Info
            var shortName = lineArrays[0].Split('=')[1];
            var date = int.Parse(lineArrays[1].Split('=')[1]);
            
            var start = lineArrays[2].Split('=')[1].Split(' ');
            var end = lineArrays[3].Split('=')[1].Split(' ');

            (subject.startHour, subject.startMinute, subject.endHour, subject.endMinute) = 
            (int.Parse(start[0]), int.Parse(start[1]), int.Parse(end[0]), int.Parse(end[1]));

            subject.date =date;
            subject.fullName = shortName + ' ' + STDTime(start[0], start[1]) + " - " + STDTime(end[0], end[1]);

            output.Add(subject);

            Debug.Log(data);
        }

        output.Sort((lhs, rhs) => lhs.CompareTo(rhs));
        var rows = basicInfo.Split('\n');
        int i = 0;

        foreach (var sbj in output)
        {
            while (i < rows.Length && !rows[i].Contains('-'))
            {
                i++;
            }
            sbj.indicatorY = startIndicatorY - i * perRowHeight;
            i++;
        }

        return output.ToArray();
    }

    private static string STDTime(string hour, string minute) => hour + ":" + minute;
}
