using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*****
声音管理器，负责声音的播放.
*****/

public enum AudioName
{
    duijiekou = 0,
    qizhacang = 1,
    fuwucang = 2,
    hengjia = 3,
    taiyangnengdianchiban = 4,
    huocang = 5,
    huojianfadongji = 6,
    jiashicang = 7,
    ranliaochuguan = 8,
    shenghuocang = 9,
    BGM_denglu = 10      //背景音乐.
}

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get { return _instance; }
    }

    private AudioSource musicAS;    //播放音乐的音源组件.
    private AudioSource[] soundArray;  //播放音效的音源数组.

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        //创建一个空对象，取名为soundGo，用于挂载上面的音乐音源组件和音效音源组件.
        GameObject sndGo = new GameObject("soundGo");
        //在空对象身上，添加一个音源组件，专用于播放音乐.
        musicAS = sndGo.AddComponent<AudioSource>();
        musicAS.playOnAwake = false;
        //对于音效播放，5个音源组件轮流使用，已经足够.
        soundArray = new AudioSource[5];
        for (int i = 0; i < soundArray.Length; i++)
        {
            soundArray[i] = sndGo.AddComponent<AudioSource>();
        }
    }

    //播放背景音乐.
    public void PlayMusic()
    {
        //加载音乐文件.
        AudioClip clip = Resources.Load<AudioClip>("Audio/BGM_denglu");
        //将加载的声音作为音乐音源组件的clip属性.
        musicAS.clip = clip;
        musicAS.volume = 0.5f;
        //播放音乐.
        musicAS.Play();
        Debug.LogWarning("播放音乐,musicAS.clip:" + musicAS.clip);
    }

    //播放音效.
    public void PlaySound(AudioName audioName)
    {
        string soundName = Enum.GetName(typeof(AudioName), audioName);
        //加载音乐文件.
        AudioClip clip = Resources.Load<AudioClip>("Audio/" + soundName);
        //获得一个空闲的音源组件.
        AudioSource soundAS = GetFreeSource();
        //停止所有音效.
        StopSound();
        //将加载的声音直接播放.
        //soundAS.clip = clip;
        soundAS.loop = false;
        soundAS.PlayOneShot(clip);
    }

    //获得一个未占用的音源组件.
    AudioSource GetFreeSource()
    {
        //循环遍历音效组件数组，看看哪个音效组件是空闲的.
        for (int i = 0; i < soundArray.Length; i++)
        {
            if (!soundArray[i].isPlaying)
            {
                return soundArray[i];
            }
        }
        return soundArray[0];
    }

    public void StopMusic()
    {
        // 停止音乐
        musicAS.Stop();
    }

    public void StopSound()
    {
        //循环遍历音效组件数组，停止所有音效.
        for (int i = 0; i < soundArray.Length; i++)
        {
            if (soundArray[i].isPlaying)
            {
                soundArray[i].Stop();
            }
        }
    }
}