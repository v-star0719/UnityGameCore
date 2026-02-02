using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace GameCore.Unity
{
    public class TreeViewExampleWnd : EditorWindowBase
    {
        public class TreeNode : ITreeNode
        {
            public string Name { get; private set; }
            public Rect Rect { get; set; }
            public List<ITreeNode> ChildNodes { get; private set; } = new List<ITreeNode>();
            public ITreeNode ParentNode { get; private set; }
            public bool IsDragging { get; set; }
            public float TotalWidth { get; set; }

            public TreeNode(string name, TreeNode parent = null)
            {
                this.Name = name;
                this.ParentNode = parent;
            }
        }

        private TreeView treeView;

        [MenuItem("Window/TreeViewExampleWnd")]
        public static void ShowWindow()
        {
            GetWindow<TreeViewExampleWnd>("TreeViewExample");
        }

        protected override void OnEnable()
        {
            treeView = new TreeView(120, 60, 80, 100);
            treeView.onNodeClick = OnNodeClick;

            // 创建一个示例树结构

            // 创建根节点
            var rootNode = new TreeNode("Root");

            // 添加一级子节点
            ITreeNode child1 = new TreeNode("Child 1", rootNode);
            ITreeNode child2 = new TreeNode("Child 2", rootNode);

            rootNode.ChildNodes.Add(child1);
            rootNode.ChildNodes.Add(child2);

            // 为Child 1添加子节点
            child1.ChildNodes.Add(new TreeNode("Child 1-1", (TreeNode)child1));
            child1.ChildNodes.Add(new TreeNode("Child 1-2", (TreeNode)child1));

            // 为Child 2添加子节点
            child2.ChildNodes.Add(new TreeNode("Child 2-1", (TreeNode)child2));
            child2.ChildNodes.Add(new TreeNode("Child 2-2", (TreeNode)child2));

            // 为Child 2-1添加子节点
            child2.ChildNodes[0].ChildNodes.Add(new TreeNode("Child 2-1-1", child2.ChildNodes[0] as TreeNode));

            treeView.rootNode = rootNode;
        }

        protected void OnGUI()
        {
            treeView.OnGUI(position);

            //if (GUI.changed)
            //{
            //    Repaint();
            //}
        }

        private void OnNodeClick(ITreeNode node)
        {
            Debug.Log("OnNodeClick: " + node.Name);
        }

        // 窗口大小改变时重新布局
        //protected override void OnResize(Vector2 currentSize, Vector2 lastSize)
        //{
        //    base.OnResize(currentSize, lastSize);
        //    treeView();
        //}
    }
}