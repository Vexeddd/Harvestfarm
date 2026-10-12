

using System;

public interface IStoryNode 
{
    void StartNode(Action<object> callBack);  //开始执行节点

    //void SkipNode();
}
