using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TFramework
{
    public class VitalPath
    {
        public static string StreamingAssetsPath
        {
            get
            {
                if (string.IsNullOrEmpty(streamingAssetsPath))
                {
                    streamingAssetsPath = Application.streamingAssetsPath;
                }
                return streamingAssetsPath;
            }
        }

        public static string PersistentDataPath
        {
            get
            {
                if (string.IsNullOrEmpty(persistentDataPath))
                {
                    persistentDataPath = Application.persistentDataPath;
                }
                return persistentDataPath;
            }
        }
        
        public static string CurrentDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(currentDirectory))
                {
                    currentDirectory = Application.dataPath;
                }
                return currentDirectory;
            }
        }
        
        private static string streamingAssetsPath;
        private static string persistentDataPath;
        private static string currentDirectory;

        /// <summary>
        /// 获取文件地址，根据名称、格式 : ***.cs
        /// </summary>
        #if UNITY_EDITOR
        public static string GetFilePath(string fileName,string fileFormat)
        {
            string filePath = "";
            
            string[] guidArray = UnityEditor.AssetDatabase.FindAssets(fileName);
            foreach (string guid in guidArray) {
                string scriptFullPath = AssetDatabase.GUIDToAssetPath(guid);
                if (scriptFullPath.EndsWith(fileName + "." + fileFormat))
                {
                    filePath = scriptFullPath;
                }
            }

            return filePath;
        }
        #endif
    }
}
