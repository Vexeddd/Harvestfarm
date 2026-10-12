using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class PlayFilmModule : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RawImage rawImage;

    private void Awake()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GameObject.Find("Movie").GetComponent<VideoPlayer>();
        }

        if (rawImage == null)
        {
            rawImage = GameObject.Find("Movie").GetComponent<RawImage>();
        }

        rawImage.enabled = false;
    }

    public void StartVideo(PlayFilmNodeData data, Action callBack)
    {
        rawImage.enabled = true;

        StartCoroutine(PlayMovie(data, callBack));
    }

    IEnumerator PlayMovie(PlayFilmNodeData data, Action callBack)
    {
        Debug.Log("加载Movie");
        var v = Resources.LoadAsync<VideoClip>("Movie/" + data.film_id);
        yield return v;

        Debug.Log("加载完成开始播放");
        videoPlayer.clip = v.asset as VideoClip;
        videoPlayer.Play();

        yield return new WaitForSeconds(3f);    //  videoPlayer.clip.length

        videoPlayer.Stop();
        rawImage.enabled = false;

        callBack.Invoke();
    }   
}
