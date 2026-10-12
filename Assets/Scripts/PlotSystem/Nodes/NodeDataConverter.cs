using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// 自动节点json转换器
/// </summary>
public class NodeDataConverter : JsonConverter<NodeData>
{
    public override NodeData ReadJson(JsonReader reader, Type objectType,
        NodeData existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        JObject obj = JObject.Load(reader);

        NodeData node = new NodeData();

        // node.type = (NodeType)Enum.Parse(typeof(NodeType),obj["type"]?.ToString());
        node.type = obj["type"].ToObject<NodeType>();


        JObject dataObj = obj["body"] as JObject;

        // 根据 type 解析为具体的 Body 类型
        if (dataObj != null)
        {
            node.body = CreateNodeBody(node.type, dataObj);
        }

        return node;
    }

    private INodeBody CreateNodeBody(NodeType type, JObject body)
    {
        switch (type)
        {
            case NodeType.Dialogue:
                return body.ToObject<DialogueNodeData>();
            case NodeType.Timeline:
                return body.ToObject<TimelineNodeData>();
            case NodeType.GiveQuest:
                return body.ToObject<GiveQuestNodeData>();
            case NodeType.PlayFilm:
                return body.ToObject<PlayFilmNodeData>();
            default:
                UnityEngine.Debug.LogWarning($"Unknown node type: {type}");
                return null;
        }
    }

    public override void WriteJson(JsonWriter writer, NodeData value, JsonSerializer serializer)
    {
        JObject obj = new JObject();
        obj["type"] = JToken.FromObject(value.type);
        obj["body"] = JToken.FromObject(value.body, serializer);
        obj.WriteTo(writer);
    }
}
