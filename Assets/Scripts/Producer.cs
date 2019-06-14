using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Producer : MonoBehaviour {
    
    GameObject m_coinpref;
    GameObject m_chestpref;
    
    public int m_type;

    int totalcount = 7;

    List<GameObject> coinList = new List<GameObject>();
    
    private void Start()
    {
        BoxCollider2D bc = GetComponent<BoxCollider2D>();
    }

    void LoadAsset()
    {
        if (m_coinpref == null && m_type == 0)
        {
            m_coinpref = MyBundleManager.Instance.GetAsset("coin");
        }
        if(m_chestpref == null && m_type == 1)
        {
            m_chestpref = MyBundleManager.Instance.GetAsset("Chest");
        }
    }
    
    private void OnMouseDown()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if(hit.transform.Equals(transform))
        {
            //Debug.Log("i am the one ..... " + name);

            if (m_type == 0)
            {
                LoadAsset();
                CreateGoods(m_coinpref);
            }

            if (m_type == 1)
            {
                LoadAsset();
                CreateGoods(m_chestpref);
            }
        }

    }

    // Update is called once per frame
    //void Update () {
    //    if (Input.GetMouseButtonDown(0) && m_type == 0) 
    //    {
    //        LoadAsset();
    //        CreateGoods(m_coinpref);
    //    }

    //    if (Input.GetMouseButtonDown(1) && m_type == 1)
    //    {
    //        LoadAsset();
    //        CreateGoods(m_chestpref);
    //    }
    //}

    void CreateGoods(GameObject pref)
    {
        if (pref == null)
            return;

        if (coinList.Count >= totalcount)
        {
            coinList[Random.Range(0,totalcount-1)].transform.localPosition = new Vector3(Random.Range(-20, 0), 0, 0);
            return;
        }
        GameObject obj = Instantiate(pref, transform);
        obj.transform.localPosition = new Vector3(Random.Range(-20, 0), 0, 0);
        coinList.Add(obj);
    }
}
