//This Script contains methods that are able to save arrays and lists of data (Not my creation got it form internet as it helps a lot )
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
public static class JsonHelper 
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }

    public static void SaveToJSON<T>(List<T> toSave,string filename)
    {
        string content = JsonHelper.ToJson<T>(toSave.ToArray());

        string printOutPath = GetPath(filename);
        Debug.Log("---------SaveToJSON--List----------Path-: " + printOutPath);
        WriteFile(GetPath(filename), content);
    }

    //Save Single Obj
    public static void SaveToJSON<T>(T toSave, string filename)
    {
        string content = JsonUtility.ToJson(toSave);
        WriteFile(GetPath(filename), content);
    }

    //Convert the json file from Streamer and convert it  back to List<> 
    public static List<T> ReadListFromJSON<T>(string filename)
    {
        string content = ReadFile(GetPath(filename));
        //Check if its not empty
        if(string.IsNullOrEmpty(content) || content == "{}")
        {
            return new List<T>();
        }
        List<T> res = JsonHelper.FromJson<T>(content).ToList();
        return res;
    }

    public static T ReadFromJSON<T>(string filename)
    {
        string content = ReadFile(GetPath(filename));
        //Check if its not empty
        if (string.IsNullOrEmpty(content) || content == "{}")
        {
            return default(T);
        }

          T res = JsonUtility.FromJson<T>(content);

        return res;
    }

    public static string GetPath(string filename)
    {
        return Application.persistentDataPath + "/" + filename;
      
        Debug.Log(Application.persistentDataPath + "/" + filename);
    }

    private static void WriteFile(string path , string content)
    {
        FileStream stream = new FileStream(path, FileMode.Create);
        using(StreamWriter writer = new StreamWriter(stream))
        {
            writer.Write(content);
        }
    }

    //Get the file to the Streamer(Find file)
    private static string ReadFile(string path)
    {
        if (File.Exists(path))
        {
            using(StreamReader reader = new StreamReader(path))
            {
                string content = reader.ReadToEnd();
                return content;
            }
        }else
              Debug.LogError("File Json Not found"); return "";
    }
}