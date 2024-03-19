using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region SIMPLE_SINGLETON
    private static AudioManager m_instance = null;
    public static AudioManager Instance { get { return m_instance; } }
    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion // SIMPLE_SINGLETON

    [SerializeField] AudioSource bgm_AS;
    [SerializeField] AudioSource sfx_AS;

    public int BGM_Volume;
    public int SFX_Volume;

    //TODO make button clicks, bgm start, bgm stop, bgm fade for load, bgm fade after load, y'know, audio stuff
}
