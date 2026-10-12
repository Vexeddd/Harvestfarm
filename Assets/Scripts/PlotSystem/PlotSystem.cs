using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotSystem : SingletonAutoMono<PlotSystem>
{
    public StoryData currentStory;  //当前剧情故事线
                                    //故事线（story）-> 章节（segment）-> 节点（node）（节点就是每一步：播放动画，接任务，交任务，Timeline什么的）

    public SegmentRunner currentRunner; //序列执行器

    public int currentSegment;


    public PlayFilmModule playFilmModule;
    public TimelineModule timelineModule;
    public DialogueModule dialogueModule;
    public QuestModule questModule;

    public GameObject stepItemPrefab; //任务预制体

    protected override void Awake()
    {
        base.Awake();

        TextAsset text = Resources.Load<TextAsset>("Data/Plot");
        currentStory = JsonConvert.DeserializeObject<StoryData>(text.text);



        playFilmModule = UnityEngine.Object.FindObjectOfType<PlayFilmModule>();
        if (playFilmModule == null)
        {
            GameObject obj1 = new GameObject("PlayFilmModule");
            playFilmModule = obj1.AddComponent<PlayFilmModule>();
            obj1.transform.parent = this.transform;
        }

        timelineModule = UnityEngine.Object.FindObjectOfType<TimelineModule>();
        if (timelineModule == null)
        {
            GameObject obj1 = new GameObject("TimelineModule");
            timelineModule = obj1.AddComponent<TimelineModule>();
            obj1.transform.parent = this.transform;
        }

        dialogueModule = UnityEngine.Object.FindObjectOfType<DialogueModule>();
        if (dialogueModule == null)
        {
            GameObject obj1 = new GameObject("DialogueModule");
            dialogueModule = obj1.AddComponent<DialogueModule>();
            obj1.transform.parent = this.transform;
        }

        questModule = UnityEngine.Object.FindObjectOfType<QuestModule>();
        if (questModule == null)
        {
            GameObject obj1 = new GameObject("QuestModule");
            questModule = obj1.AddComponent<QuestModule>();
            obj1.transform.parent = this.transform;
        }

        stepItemPrefab = Resources.Load<GameObject>("Prefab/UI/QuestStepItem");
    }

    private void Start()
    {
        currentSegment = -1;
        NextSegment();
    }

    public void NextSegment()
    {
        currentSegment++;
        Debug.Log($"进入下一剧情{currentSegment}");

        if (currentSegment >= currentStory.segments.Count)
        {
            Debug.Log("当前故事线完结");
            return;
        }

        SegmentData segment = currentStory.segments[currentSegment];
        currentRunner = new SegmentRunner(segment);

        currentRunner.Start();
    }

    public void ResumeSegment(int segmentIndex, int nodeIndex)
    {
        if (segmentIndex < 0 || segmentIndex >= currentStory.segments.Count)
        {
            return;
        }

        currentSegment = segmentIndex;
        SegmentData segment = currentStory.segments[currentSegment];
        currentRunner = new SegmentRunner(segment);
        currentRunner.Start(nodeIndex);
    }   
}
