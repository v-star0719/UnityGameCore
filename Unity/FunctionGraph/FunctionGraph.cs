using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//函数图像
//定义一些运算，运算填充起来就行了，省事不用搞表达式解析
//运算类型：常量、变量（x,y,z）、加、减、乘、除
//未实现，待施工
public class FunctionGraph : MonoBehaviour
{
    public GameObject itemPrefab;
    public bool showBtn;
    public FunctionGraphAxis xAxis;
    public FunctionGraphAxis yAxis;
    public FunctionGraphAxis zAxis;
    private LinkedList<GameObject> usedBlocks = new LinkedList<GameObject>();
    private LinkedList<GameObject> unusedBlocks = new LinkedList<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (showBtn)
        {
            showBtn = false;
            Show();
        }
    }

    public void Show()
    {
        RecycleAllBlocks();
        var xStep = 0.1f;
        var yStep = 0.1f;
        for (float x = 0; x < 4; x+=0.1f)
        {
            for(float y = 0; y < 0.01f; y+=0.1f)
            {
                var go = GetBlock();
                var z = (1 + x) * (1 + 4 - x);
                go.transform.localPosition = new Vector3(x/xStep, z, y/yStep);
            }
        }
        xAxis.SetLength(4 * 1.1f);
        yAxis.SetLength(4 * 1.1f);
        zAxis.SetLength(4 * 1.1f);
    }

    private GameObject GetBlock()
    {
        if (unusedBlocks.Count > 0)
        {
            var rt = unusedBlocks.First;
            unusedBlocks.RemoveFirst();
            usedBlocks.AddLast(rt);
            return rt.Value;
        }

        var go = GameObject.Instantiate(itemPrefab, itemPrefab.transform.parent);
        usedBlocks.AddLast(go);
        return go;
    }

    private void RecycleAllBlocks()
    {
        foreach (var block in usedBlocks)
        {
            unusedBlocks.AddLast(block);
        }
        usedBlocks.Clear();
    }
}
