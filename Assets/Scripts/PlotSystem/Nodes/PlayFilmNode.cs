
using System;

public class PlayFilmNode : IStoryNode
{
    public PlayFilmNodeData data;

    public PlayFilmNode() { }

    public PlayFilmNode(PlayFilmNodeData data)
    {
        this.data = data;
    }

    public void StartNode(Action<object> callBack)
    {
        PlotSystem.Instance.playFilmModule.StartVideo(data, () =>
        {
            callBack?.Invoke(null);
        });
    }


    public static IStoryNode CreateFromJson(INodeBody body)
    {
        PlayFilmNodeData data = body as PlayFilmNodeData;

        return new PlayFilmNode(data);
    }
}
