using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEnterGame : MonoBehaviour {

    public GameObject m_loadingAni;

    public GameObject m_mask;

    //string m_state="Waiting";


    SpriteRenderer sr;
    ParticleSystem ps;
    ParticleSystemRenderer psr;

	public void OnEnterGame()
    {
        m_mask.SetActive(true);
        m_loadingAni.SetActive(true);

        MyBundleManager.Instance.LoadScene("http://10.86.34.119:8080/app/AssetBundle/demo", "Demo");
    }
}
