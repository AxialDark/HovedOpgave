using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;



public class APIDataExtractor
{
    private APIData data;
    private static char[] numberArray = new char[]
    {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
    };

    public APIData Data { get { return data; } }


    public APIDataExtractor(string _text)
    {
        List<string> extractedData = new List<string>();

        string[] splitData = _text.Split('\n');

        for (int i = 0; i < splitData.Length; i++)
        {
            extractedData.Add(splitData[i]);
        }

        ConvertToData(extractedData);
    }

    public APIDataExtractor(Stream _dataStream)
    {
        List<string> extractedData = new List<string>();

        StreamReader sr = new StreamReader(_dataStream);

        while (!sr.EndOfStream)
        {
            extractedData.Add(sr.ReadLine());
        }

        ConvertToData(extractedData);
    }

    private void ConvertToData(List<string> _dataToConvert)
    {
        string time = _dataToConvert[6];
        string distance = _dataToConvert[7];

        List<string> allPos = ExtractPositions(_dataToConvert);

        data = new APIData(time, distance, allPos);
    }

    private List<string> ExtractPositions(List<string> _dataToConvert)
    {
        List<string> temp = new List<string>();
        string allData = "";

        for (int i = 0; i < _dataToConvert.Count; i++)
        {
            allData += _dataToConvert[i];
        }

        allData = allData.Trim('\n');

        allData = allData.Replace("<gml:pos>", ";");
        allData = allData.Replace("</gml:pos>", ";");

        string[] splitResult = allData.Split(';');

        for (int i = 0; i < splitResult.Length; i++)
        {
            if (!string.IsNullOrEmpty(splitResult[i]) && numberArray.Contains(splitResult[i][0]))
            {
                temp.Add(splitResult[i]);
            }
        }

        return temp;
    }
}

