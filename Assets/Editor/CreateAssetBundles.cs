using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEditor;
using UnityEngine;

public class CreateAssetBundles
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        Debug.Log("Yeah, I'm clicking");
        string assetBundleDirectory = Application.dataPath+"/AssetBundles";
        try
        {
            BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);

        }
        catch(Exception e)
        {
            Debug.LogWarning(e.ToString());
        }
        /*if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory,
        BuildAssetBundleOptions.None,
        BuildTarget.StandaloneWindows);*/
    }
}
