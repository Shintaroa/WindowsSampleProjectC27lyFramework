using System.Collections.Generic;
using System.Xml;
using UnityEngine;

//一般情况下不再使用都使用systemconfig
public class XMLConfig
{
    private static bool isInit;
    private static Dictionary<string, object> configTable = new Dictionary<string, object>();

    public static bool IsInit
    {
        get
        {
            return isInit;
        }
    }

    public static void Init(string path =  "/config.xml")
    {
        string m_URL = Application.streamingAssetsPath + path;
        if (System.IO.File.Exists(m_URL))
        {
            Debug.Log("=> Config Start Initialize ");
            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.Load(m_URL);
            XmlElement rootElem = XmlDoc.DocumentElement;

            XmlNode firstNode = rootElem.FirstChild;
            XmlNodeList list = rootElem.ChildNodes;

            for (int i = 0; i < list.Count; i++)
            {
                var node = list.Item(i);
                configTable[node.Name] = node.InnerText;
                Debug.Log("=> node.Name " + node.Name + " v " + configTable[node.Name]);
            }

            isInit = true;
            Debug.Log("=> Config Initialized ");
        }
        else
        {
            throw new XmlException("没有 m_URL");
        }    
    }

    public static bool Read(string name, out string value)
    {
        value = "";

        if (!isInit) return false;

        if (configTable.ContainsKey(name))
        {
            value = configTable[name].ToString();
            return true;
        }

        return false;
    }

    public static bool Read(string name, out int value)
    {
        value = 0;

        if (!isInit) return false;
       
        if (configTable.ContainsKey(name))
        {        
            value = int.Parse(configTable[name].ToString());
            return true;
        }

        return false;
    }

    public static bool Read(string name, out float value)
    {
        value = 0f;
        if (!isInit) return false;

        if (configTable.ContainsKey(name))
        {
            value = float.Parse(configTable[name].ToString());
            return true;
        }

        return false;
    }

    public static bool Read(string name, out bool value)
    {
        value = false;

        if (!isInit) return false;

        if (configTable.ContainsKey(name))
        {
            if (configTable[name].ToString() == "true")
            {
                value = true;
            }
            else
            {
                value = false;
            }
            return true;
        }

        return false;
    }
}
