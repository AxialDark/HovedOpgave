using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


/// <summary>
/// Used for exstracting data from route API response
/// </summary>
public class APIDataExtractor
{
    private APIData data;
    private static char[] numberArray = new char[]
    {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
    };

    /// <summary>
    /// The resulting data from API
    /// </summary>
    public APIData Data { get { return data; } }

    /// <summary>
    /// Constructor for APIDataExtractor class
    /// </summary>
    /// <param name="_text">API data as a string</param>
    public APIDataExtractor(string _text)
    {
        List<string> extractedData = new List<string>(); //Instantiates a list

        string[] splitData = _text.Split('\n'); //Split the data on newline character

        for (int i = 0; i < splitData.Length; i++) //Run through all lines
        {
            extractedData.Add(splitData[i]); //Add line to list
        }

        ConvertToData(extractedData); //Convert the raw data to APIData
    }

    /// <summary>
    /// Constructor for APIDataExtractor class
    /// </summary>
    /// <param name="_dataStream">API data as a stream</param>
    public APIDataExtractor(Stream _dataStream)
    {
        List<string> extractedData = new List<string>(); //Instantiates a list

        StreamReader sr = new StreamReader(_dataStream); //Creates a StreamReader from stream

        while (!sr.EndOfStream) //While the reader hasn't reached the end of the stream
        {
            extractedData.Add(sr.ReadLine()); //Add the data line to list
        }

        ConvertToData(extractedData); //Convert the raw data to APIData
    }

    /// <summary>
    /// Converts from raw data to APIData class
    /// </summary>
    /// <param name="_dataToConvert">The response from API as a list of strings</param>
    private void ConvertToData(List<string> _dataToConvert)
    {
        string time = _dataToConvert[6]; //Time is the 7th string in list
        string distance = _dataToConvert[7]; //Distance is the 8th string in list

        List<string> allPos = ExtractPositions(_dataToConvert); //Exstract all the long/lat positions from data

        data = new APIData(time, distance, allPos); //Convert data using APIData class
    }

    /// <summary>
    /// Extracts all the long/lat positions from response data
    /// </summary>
    /// <param name="_dataToConvert">The raw data from route API</param>
    /// <returns>All the routes long/lat positions as a list</returns>
    private List<string> ExtractPositions(List<string> _dataToConvert)
    {
        List<string> temp = new List<string>(); //Creates a tempoary list of strings
        string allData = ""; //Creates empty string

        for (int i = 0; i < _dataToConvert.Count; i++) //Put all data back to a string
        {
            allData += _dataToConvert[i];
        }

        allData = allData.Trim('\n'); //Removes all the newline charaters

        allData = allData.Replace("<gml:pos>", ";"); //Replaces start position tag with ';'
        allData = allData.Replace("</gml:pos>", ";"); //Replaces end position tag with ';'

        string[] splitResult = allData.Split(';'); //Splits the data

        for (int i = 0; i < splitResult.Length; i++) //Runs through array
        {
            if (!string.IsNullOrEmpty(splitResult[i]) && numberArray.Contains(splitResult[i][0])) //If string is not null or empty and the first char in the string is a number
            {
                temp.Add(splitResult[i]); //Add long/lat to list
            }
        }

        return temp;
    }
}

