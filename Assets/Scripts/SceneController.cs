using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

    bool hasNextScene = false;

    private void OnEnable()
    {
        //if (MyBundleManager.Instance != null) 
        //{
        //    MyBundleManager.Instance.UnloadAssetBundle(SceneManager.GetActiveScene().name.ToLower());
        //}
        
        if (SceneManager.GetActiveScene().name.CompareTo("Demo") == 0)
            hasNextScene = true;
    }

    private void OnGUI()
    {
        if(hasNextScene)
        {
            GUILayout.Space(10);
            if (GUILayout.Button("Next Scene")) 
            {
                MyBundleManager.Instance.LoadScene("http://10.86.34.119:8080/app/AssetBundle/creepybird", "CreepyBird");
            }

        }
        else
        {
            GUILayout.BeginHorizontal();
            string[] bundleNames = { "prefabs", "coin", "chest" };
            for (int i = 0; i < bundleNames.Length; i++) 
            {
                string s = bundleNames[i];
                GUILayout.Space(10);
                if (GUILayout.Button(s))
                {
                    if (i > 0)
                        MyBundleManager.Instance.RemoveAsset(s);

                    MyBundleManager.Instance.UnloadAssetBundle(s);
                    MyBundleManager.Instance.GetAsset(s);
                }
            }
            GUILayout.EndHorizontal();
           

            //GUILayout.Space(10);
            //if (GUILayout.Button("GenerateSceneBundls"))
            //{

            //    MyBundleManager.Instance.UnloadAssetBundle("creepybird");
            //    MyBundleManager.Instance.GetAsset("creepybird");
            //}
        }
    }

}
