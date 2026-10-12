using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentRunner
{
    public SegmentData Data { get; private set; }  //包含节点数据

    public List<IStoryNode> nodes = new List<IStoryNode>();   // 构建后的节点
    public int currentIndex = -1;

    public SegmentRunner()
    {

    }

    public SegmentRunner(SegmentData data)
    {
        Data = data;
    }

    public void Start()
    {
        Start(0);
    }

    public void Start(int startIndex)
    {
        if (Data.nodes != null)
        {
            foreach (var nd in Data.nodes)
            {
                var node = NodeFactory.Create(nd);
                nodes.Add(node);
            }
        }

        MonoMgr.Instance.StartCoroutine(RunNodes(startIndex));
    }


    //开始运行一系列节点
    IEnumerator RunNodes(int startIndex)
    {
        for (int i = startIndex; i < nodes.Count; i++)
        {
            //当前节点完成标志
            bool isFinish = false;
            //用来接收IStoryNode中StartNode的回调函数的返回值
            object result = null;

            currentIndex = i;
            IStoryNode storyNode = nodes[i];
            storyNode.StartNode((obj) =>
            {
                isFinish = true;
                result = obj;
            });

            //等待当前节点完成，进入下一节点
            while (!isFinish)
            {
                yield return null;
            }

            //yield return new WaitForSeconds(3f);
        }

        //当前nodes内所有节点完成，代表当前片段（章节：Segment）全部完成，进入下一章
        FinishSegment();

    }

    private void FinishSegment()
    {
        //完成后的一些处理：记录信息，播放动画，UI提示等等等等
        //.......

        //Debug.Log("当前segment全部完成");
        //开启协程是为了防止下一章的资源没加载完，就进入了下一章，导致出现问题
        MonoMgr.Instance.StartCoroutine(NextSegment());
    }

    IEnumerator NextSegment()
    {
        yield return new WaitForSeconds(2f);

        PlotSystem.Instance.NextSegment();  
    }
}
