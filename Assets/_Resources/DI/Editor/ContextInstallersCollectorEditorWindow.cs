using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Zenject;
using System;


public class ContextInstallersCollectorEditorWindow : EditorWindow
{

    private const string kProjectContextPath = "Assets/Resources/ProjectContext.prefab";

    [MenuItem("Tools/Collect all Context Installers")]
    private static void CollectAllContextInstallers()
    {
        if (!File.Exists(kProjectContextPath))
        {
            throw new System.NullReferenceException(
                string.Format("Zenject's ProjectContext prefab doesn't exists at {0}", kProjectContextPath)
                );
        }

        ProjectContext projectContext = AssetDatabase.LoadAssetAtPath<ProjectContext>(kProjectContextPath);
        Debug.Log("ProjectContext founded");

        var resourcesPath = Application.dataPath;
        var absolutePaths = Directory.GetFiles(resourcesPath, "*.prefab", SearchOption.AllDirectories);
        Debug.LogFormat("{0} objects founded at {1}", absolutePaths.Length, resourcesPath);
        List<MonoInstaller> availableContextInstallers = new List<MonoInstaller>();
        foreach (var absolutePath in absolutePaths)
        {
            string path = absolutePath.Replace(Application.dataPath, "Assets");
            var prefab = AssetDatabase.LoadAssetAtPath< GameObject>(path);

            if (prefab != null && prefab.TryGetComponent(out MonoInstaller monoInstaller))
            {
                Debug.LogFormat("MonoInstaller detected at path: {0}", path);
                Type monoObject = monoInstaller.GetType();
                var att = monoObject.GetCustomAttributes(typeof(ContextInstallerAttribute), true);
                if (att != null && att.Length > 0)
                {
                    availableContextInstallers.Add(monoInstaller);
                }
            }
        }
        projectContext.Installers = availableContextInstallers;
    }
}
