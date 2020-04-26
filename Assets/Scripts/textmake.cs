using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class textmake : MonoBehaviour
{
    void CreateText()
    {
        //Path of the file
        string path = Application.dataPath + "/Log.txt";
        //Create File if it doesn't exist
        if (!File.Exists(path))
        {
            File.WriteAllText(path, "Login log \n\n");
        }
        //Content of the file
        string content = "Login date: " + System.DateTime.Now + "\n";
        //Add some to text to it
        File.AppendAllText(path, content);
    }

    // Use this for initialization
    void Start()
    {
        CreateText();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
