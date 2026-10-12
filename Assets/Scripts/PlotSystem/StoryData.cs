using System;
using System.Collections.Generic;
using Newtonsoft.Json;

//剧情数据
[Serializable]
public class StoryData
{
    public string story_id; //id
    public string title;
    public string desc;
    public List<SegmentData> segments;
}

//片段数据
public class SegmentData
{
    public string segment_id;
    public string title;
    public string next_segment;

    public List<NodeData> nodes; //节点组
}

//节点数据
[JsonConverter(typeof(NodeDataConverter))]
public class NodeData
{
    public NodeType type;
    public INodeBody body { get; set; }
}

//节点数据接口
public interface INodeBody { }


//对话节点数据
public class DialogueNodeData : INodeBody
{
    public string dialogue_id;
    public string npc;
    public string[] lines; //对话内容
}

//电影节点数据
public class PlayFilmNodeData : INodeBody
{
    public string film_id;
}


//时间线(动画编排) 节点数据
public class TimelineNodeData : INodeBody
{
    public string timeline_id;
}


//交给任务 节点数据
public class GiveQuestNodeData : INodeBody
{
    public string quest_id;

    public List<QuestStep> steps;
    public string title;
    public List<RewardItem> grants;

}


//节点类型
public enum NodeType
{
    Timeline, //时间线
    Dialogue, //对话
    GiveQuest, //布置任务
    PlayFilm, //播放影片
}

