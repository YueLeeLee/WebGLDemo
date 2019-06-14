using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.IO;

public class MyBundleManager : MonoBehaviour {

    public static MyBundleManager Instance;

    const string serverPath = "http://10.86.34.119:8080/app/AssetBundle/";

    string m_state = "Waiting";

    string sceneUrl;
    string sceneName;

    //切换场景之后释放释放bundle
    bool m_useCache;

    AssetBundleManifest m_rootManifest;

    Dictionary<string, GameObject> m_assetList = new Dictionary<string, GameObject>();
    Dictionary<string, AssetBundle> m_bundleList = new Dictionary<string, AssetBundle>();

    List<string> m_loadingUrls = new List<string>();
    
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);

        StartCoroutine("LoadRootManifest");
    }
    
    IEnumerator LoadRootManifest()
    {
        using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(serverPath + "AssetBundle")) 
        {
            yield return uwr.SendWebRequest();

            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(uwr);

            AssetBundleManifest manifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            //if (manifest != null)
            //{
            //    foreach(var o in manifest.GetAllAssetBundles())
            //    {
            //        Debug.Log(o);
            //    }
            //}

            m_rootManifest = manifest;
        }
    }

    public void UnloadAssetBundle(string name)
    {
        if(m_bundleList.ContainsKey(name))
        {
            Debug.Log("Unload bundle : " + name);
            m_bundleList[name].Unload(false);
            m_bundleList.Remove(name);
        }
    }

    public void LoadScene(string path, string scenename)
    {
        sceneUrl = path;
        sceneName = scenename;
        StartCoroutine("LoadSceneAB");
    }


    IEnumerator LoadSceneAB()
    {
        string url = sceneUrl;

        UnityWebRequest uwr;
        if (m_useCache)
        {
            uwr = UnityWebRequestAssetBundle.GetAssetBundle(url, Hash128.Compute(sceneName), 0);
        }
        else
        {
            uwr = UnityWebRequestAssetBundle.GetAssetBundle(url);
        }

        using (uwr)
        {
            m_state = "SendWebRequest : " + url;
            //Debug.Log(m_state);
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                m_state = uwr.error + ", ResponseCode: " + uwr.responseCode + ", NetworkError: " +
                    uwr.isNetworkError + ", HttpError: " + uwr.isHttpError;

                Debug.Log(m_state);
            }
            else
            {
                m_state = "DownloadHandlerAssetBundle.GetContent";
                //Debug.Log(m_state);
                // Get downloaded asset bundle
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(uwr);
                m_bundleList.Add(bundle.name, bundle);

                string[] deps = m_rootManifest.GetAllDependencies(bundle.name);
                Debug.Log(bundle.name + " dependencies count : " + deps.Length);
                foreach(var d in deps)
                {
                    if (!m_bundleList.ContainsKey(d))
                    {
                        yield return StartCoroutine(LoadAssetbundle(d, ""));
                    }
                }
                
                m_state = "SceneManager.LoadSceneAsync";
                //Debug.Log(m_state);
                AsyncOperation opt = SceneManager.LoadSceneAsync(sceneName);
            }
        }
    }

    public void RemoveAsset(string name)
    {
        if(m_assetList.ContainsKey(name))
        {
            //Debug.Log("destroying :" + name);
            //Destroy(m_assetList[name]);
            m_assetList.Remove(name);
            Resources.UnloadUnusedAssets();
        }
    }

    public GameObject GetAsset(string name)
    {
        if(m_assetList.ContainsKey(name))
        {
            return m_assetList[name];
        }
        else
        {
            StartCoroutine(LoadAssetbundle(name.ToLower(), name));

            if (m_assetList.ContainsKey(name))
            {
                return m_assetList[name];
            }
            else
            {
                return null;
            }
        }
    }

    IEnumerator LoadAssetbundle(string bundlename, string objname)
    {
        string url = serverPath + bundlename;
        if (m_loadingUrls.Contains(url))
            yield break;

        m_loadingUrls.Add(url);

        //CachedAssetBundle cab = new CachedAssetBundle();
        //cab.hash = Hash128.Compute(bundlename);
        //cab.name = bundlename;
        UnityWebRequest uwr;
        if(m_useCache)
        {
            uwr = UnityWebRequestAssetBundle.GetAssetBundle(url, Hash128.Compute(bundlename), 0);
        }
        else
        {
            uwr = UnityWebRequestAssetBundle.GetAssetBundle(url);
        }
        using (uwr) 
        {
            if (!Caching.ready) 
            {
                yield return 0;
            }
            m_state = "SendWebRequest : " + url;
            //float starttime = Time.time;
            yield return uwr.SendWebRequest();
            //Debug.Log(bundlename + " Cost time : " + (Time.time - starttime) * 1000 + " ms");
            
            if (uwr.isNetworkError || uwr.isHttpError)
            {
                m_state = uwr.error + ", ResponseCode: " + uwr.responseCode + ", NetworkError: " +
                    uwr.isNetworkError + ", HttpError: " + uwr.isHttpError;

                Debug.Log(m_state);

            }
            else
            {
                // Get downloaded asset bundle
                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(uwr);
                
                if (!m_bundleList.ContainsKey(bundle.name)) 
                {
                    m_bundleList.Add(bundle.name, bundle);
                }

                m_state = "AssetBundlen : " + bundle.name;
 
                if(bundle.Contains(objname))
                {
                    GameObject obj = bundle.LoadAsset<GameObject>(objname);
                    if (!m_assetList.ContainsKey(obj.name))
                    {
                        m_assetList.Add(obj.name, obj);
                    }
                }
                

                m_loadingUrls.Remove(url);
            }
        }
        
    }


    private void OnGUI()
    {
        GUILayout.Space(30);
        GUILayout.BeginVertical();
        GUILayout.Space(30);
        //GUILayout.Label(m_state);
        GUILayout.Space(30);
        m_useCache = GUILayout.Toggle(m_useCache, "Use AssetBunlde Cache");
        
        if (m_bundleList.Count > 0)
        {
            GUILayout.Space(30);
            if (GUILayout.Button("UnloadBundle"))
            {
                Dictionary<string, AssetBundle>.Enumerator e = m_bundleList.GetEnumerator();

                while (e.MoveNext())
                {
                    e.Current.Value.Unload(false);
                    Destroy(e.Current.Value);
                    Debug.LogWarning("Unloading : " + e.Current.Key);
                }

                m_bundleList.Clear();

                System.GC.Collect();
            }

            GUILayout.BeginVertical("box");
            GUILayout.Label("----Bundle List----");
            foreach (var b in m_bundleList)
            {
                GUILayout.Label(b.Key);
            }
            GUILayout.EndVertical();
        }

        //if (GUILayout.Button("File"))
        //{
        //    StartCoroutine(LoadBundleFromFile());
        //}
        //if (GUILayout.Button("Memory"))
        //{
        //    StartCoroutine(LoadBundleFromMemory());
        //}
    }


    //////////////////
    ///
    IEnumerator LoadBundleFromFile()
    {
        string path = Application.persistentDataPath + "/AssetBundle/creepybird";

        float oldtime = Time.time;
        Debug.Log(Time.time);
        var bundleLoadRequest = AssetBundle.LoadFromFileAsync(path);
        yield return bundleLoadRequest;

        var myLoadedAssetBundle = bundleLoadRequest.assetBundle;
        if (myLoadedAssetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            yield break;
        }

        Debug.Log(Time.time);
        Debug.Log(Time.time - oldtime);

        myLoadedAssetBundle.Unload(true);
        yield return 0;
    }

    IEnumerator LoadBundleFromMemory()
    {
        string path = Application.persistentDataPath + "/AssetBundle/creepybird";

        float oldtime = Time.time;
        Debug.Log(Time.time);
        var bundleLoadRequest = AssetBundle.LoadFromMemoryAsync(File.ReadAllBytes(path));
        yield return bundleLoadRequest;

        var myLoadedAssetBundle = bundleLoadRequest.assetBundle;
        if (myLoadedAssetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            yield break;
        }

        Debug.Log(Time.time);
        Debug.Log(Time.time - oldtime);

        myLoadedAssetBundle.Unload(true);
        yield return 0;
    }
}
