using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class MusicMgr : BaseManager<MusicMgr>
{
    private AudioSource bkMusic;
    private float bkMusicVolume = 1f;

    private GameObject sound;
    private List<AudioSource> soundList = new List<AudioSource>();
    private float soundVolume = 1f;

    private MusicMgr()
    {
        MonoMgr.Instance.AddUpdateListener(MusicUpdate);
    }

    private void MusicUpdate()
    {
        for (int i = soundList.Count - 1; i >= 0; --i)
        {
            if (!soundList[i].isPlaying)
            {
                GameObject.Destroy(soundList[i]);
                soundList.RemoveAt(i);
            }
        }
    }

    public void PlayMusic(string name)
    {
        if (bkMusic == null)
        {
            GameObject musicObj = new GameObject("BKMusic");
            GameObject.DontDestroyOnLoad(musicObj);
            bkMusic = musicObj.AddComponent<AudioSource>();
        }

        ResMgr.Instance.LoadAsync<AudioClip>("Music/BK/" + name, (clip) =>
        {
            if (clip == null)
            {
                Debug.LogWarning($"音乐资源加载失败：Music/BK/{name}");
                return;
            }

            bkMusic.clip = clip;
            bkMusic.volume = bkMusicVolume;
            bkMusic.Play();
        });
    }

    public void PauseMusic()
    {
        if(bkMusic == null)
            return;
        bkMusic.Pause(); 
    }

    public void StopMusic()
    {
        if(bkMusic == null)
            { return; }
        bkMusic.Stop();
    }

    public void ChangeMusicVolume(float volume)
    {
        bkMusicVolume = volume;
        if (bkMusic != null)
        {
            bkMusic.volume = bkMusicVolume;
        }
    }


    public void PlaySound(string name, UnityAction<AudioSource> callBack)
    {
        if (sound == null)
        {
            sound = new GameObject("Sound");
            GameObject.DontDestroyOnLoad (sound);
        }

        ResMgr.Instance.LoadAsync<AudioClip>("Music/Sound/" + name, (clip) =>
        {
            if (clip == null)
            {
                Debug.LogWarning($"音效资源加载失败：Music/Sound{name}");
                return;
            }

            AudioSource soundAudio = sound.AddComponent<AudioSource>();
            soundAudio.clip = clip;
            soundAudio.volume = soundVolume;
            soundAudio.Play();

            soundList.Add(soundAudio);

            if (callBack != null)
            {
                callBack(soundAudio);
            }
        });
    }

    public void StopSound(AudioSource audio)
    {
        if (soundList.Contains(audio))
        {
            soundList.Remove(audio);  
            audio.Stop();
            GameObject.Destroy(audio);
        }
    }

    public void ChangeSoundVolume(float volume)
    {
        soundVolume = volume;
        foreach (AudioSource audioSource in soundList)
        {
            audioSource.volume = soundVolume;
        }
    }

}
