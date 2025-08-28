using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*****
���������������������Ĳ���.
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
    BGM_denglu = 10      //��������.
}

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get { return _instance; }
    }

    private AudioSource musicAS;    //�������ֵ���Դ���.
    private AudioSource[] soundArray;  //������Ч����Դ����.

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        //����һ���ն���ȡ��ΪsoundGo�����ڹ��������������Դ�������Ч��Դ���.
        GameObject sndGo = new GameObject("soundGo");
        //�ڿն������ϣ����һ����Դ�����ר���ڲ�������.
        musicAS = sndGo.AddComponent<AudioSource>();
        musicAS.playOnAwake = false;
        //������Ч���ţ�5����Դ�������ʹ�ã��Ѿ��㹻.
        soundArray = new AudioSource[5];
        for (int i = 0; i < soundArray.Length; i++)
        {
            soundArray[i] = sndGo.AddComponent<AudioSource>();
        }
    }

    //���ű�������.
    public void PlayMusic()
    {
        //���������ļ�.
        AudioClip clip = Resources.Load<AudioClip>("Audio/BGM_denglu");
        //�����ص�������Ϊ������Դ�����clip����.
        musicAS.clip = clip;
        musicAS.volume = 0.5f;
        //��������.
        musicAS.Play();
        Debug.LogWarning("��������,musicAS.clip:" + musicAS.clip);
    }

    //������Ч.
    public void PlaySound(AudioName audioName)
    {
        string soundName = Enum.GetName(typeof(AudioName), audioName);
        //���������ļ�.
        AudioClip clip = Resources.Load<AudioClip>("Audio/" + soundName);
        //���һ�����е���Դ���.
        AudioSource soundAS = GetFreeSource();
        //ֹͣ������Ч.
        StopSound();
        //�����ص�����ֱ�Ӳ���.
        //soundAS.clip = clip;
        soundAS.loop = false;
        soundAS.PlayOneShot(clip);
    }

    //���һ��δռ�õ���Դ���.
    AudioSource GetFreeSource()
    {
        //ѭ��������Ч������飬�����ĸ���Ч����ǿ��е�.
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
        // ֹͣ����
        musicAS.Stop();
    }

    public void StopSound()
    {
        //ѭ��������Ч������飬ֹͣ������Ч.
        for (int i = 0; i < soundArray.Length; i++)
        {
            if (soundArray[i].isPlaying)
            {
                soundArray[i].Stop();
            }
        }
    }
}