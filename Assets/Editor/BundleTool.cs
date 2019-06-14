using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class BundleTool : MonoBehaviour {

    [MenuItem("Assets/BuildAssetBundle")]
    public static void BuildBundle()
    {
        Debug.Log(Application.persistentDataPath);
        string path = Application.persistentDataPath + "/AssetBundle";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        
        AssetBundleManifest manif = BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.WebGL);
   
        foreach (var f in manif.GetAllAssetBundles())
        {
            Debug.Log(f);
        }
    }
}
