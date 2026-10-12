
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 节点工厂 （把数据构建成节点）
/// </summary>
public static class NodeFactory
{
    public static Dictionary<NodeType, NodeCreator> creators = new Dictionary<NodeType, NodeCreator>(); //创建器字典

    //swtich
    //   case NodeType.PlayFilm
    //  . ...return new PlayFilmNode()
    //   break;

    //  case . ...

    public delegate IStoryNode NodeCreator(INodeBody body);

    public static void Register(NodeType type, NodeCreator nodeCreator)
    {
        creators[type] = nodeCreator;
    }

    static NodeFactory()
    {
        Register(NodeType.PlayFilm, PlayFilmNode.CreateFromJson);
        Register(NodeType.Timeline, TimelineNode.CreateFromJson);
        Register(NodeType.Dialogue, DialogueNode.CreateFromJson);
        Register(NodeType.GiveQuest, GiveQuestNode.CreateFromJson);
    }


    public static IStoryNode Create(NodeData nodeData)
    {
        if (creators.TryGetValue(nodeData.type, out var creator))
        {
            return creator(nodeData.body);
        }

        Debug.Log("未知节点类型");
        return null;
    }
}
