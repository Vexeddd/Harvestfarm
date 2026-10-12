using System;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : SingletonAutoMono<MusicManager>
{
    protected override string RootNodeName => "[Manager]";

    public Dictionary<MusicType, AudioClip> audio_dict = new Dictionary<MusicType, AudioClip>();

    private const int POOL_SIZE = 8;                      // 同时最多 8 个音效
    private readonly List<AudioSource> _pool = new List<AudioSource>();
    private int _index;

    protected override void Awake()
    {
        base.Awake();

        // 1. 加载所有音效
        AudioClip[] audioClips = Resources.LoadAll<AudioClip>("Music");
        foreach (var clip in audioClips)
        {
            if (Enum.TryParse<MusicType>(clip.name, out MusicType type))
            {
                audio_dict[type] = clip;
            }
            else
            {
                //Debug.LogWarning($"未识别的音效文件：{clip.name}");
            }

        }

        // 2. 创建 AudioSource 池
        for (int i = 0; i < POOL_SIZE; i++)
        {
            GameObject go = new GameObject($"AudioSource_{i}");
            go.transform.SetParent(transform);   // 挂到 [Manager] 下
            var src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            _pool.Add(src);
        }
    }

    public void PlayOneShot(MusicType type, float volume = 0.4f)
    {
        if (!audio_dict.TryGetValue(type, out var clip))
        {
            Debug.LogWarning($"未找到音效：{type}");
            return;
        }

        var src = GetFreeSource();
        src.clip = clip;
        src.volume = volume;
        src.Play();
    }

    private AudioSource GetFreeSource()
    {
        // 先找没在播放的
        for (int i = 0; i < _pool.Count; i++)
        {
            int idx = (_index + i) % _pool.Count;
            if (!_pool[idx].isPlaying)
            {
                _index = (idx + 1) % _pool.Count;
                return _pool[idx];
            }
        }

        // 全在播放，就抢最早的那个
        var src = _pool[_index];
        _index = (_index + 1) % _pool.Count;
        return src;
    }
}


//音乐枚举
public enum MusicType
{
    Jump, //跳跃
    OpenBag, //打开背包
    Button, //按下按钮


    PlantSeed, //种植
    Water, //浇水
    Harvest, //收货植物 √
    FillWaterCan, //水壶灌水

    Shoot, //射击声音
    Reload, //换弹
    EmptyBullet, //无弹攻击警告声
    MonsterHurt, //怪物受伤

    //汽车轰鸣声
    //喇叭声
    //刹车声
}
