using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace GameCore.Unity
{
    // 树节点数据结构
    public interface ITreeNode
    {
        public string Name { get; }
        public Rect Rect { get; set; }
        public List<ITreeNode> ChildNodes { get; }
        public ITreeNode ParentNode { get; }
        public bool IsDragging { get; set; }
        public float TotalWidth { get; set; }
    }

    public class TreeView
    {
        public ITreeNode rootNode;
        public Action<ITreeNode> onNodeClick;
        protected ITreeNode draggingNode;
        protected Vector2 dragOffset;
        protected Vector2 scrollPosition;

        // 样式定义
        private GUIStyle nodeStyle;
        private GUIStyle connectionStyle;

        private float nodeWidth = 120f;
        private float nodeHeight = 60f;
        private float horizontalSpacing = 80f;  // 水平方向间距
        private float verticalSpacing = 100f;  // 垂直方向间距

        private Rect rect;

        public TreeView(int nodeWidth, int nodeHeight, int horzSpace, int vertSpace)
        {
            this.nodeWidth = nodeWidth;
            this.nodeHeight = nodeHeight;
            horizontalSpacing = horzSpace;
            verticalSpacing = vertSpace;

            // 初始化样式
            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0.png") as Texture2D;
            nodeStyle.normal.textColor = Color.white;
            nodeStyle.border = new RectOffset(12, 12, 12, 12);
            nodeStyle.padding = new RectOffset(10, 10, 10, 10);
            nodeStyle.alignment = TextAnchor.MiddleCenter;
            nodeStyle.wordWrap = true;

            connectionStyle = new GUIStyle();
        }

        public virtual void OnGUI(Rect r)
        {
            //scrollPosition = GUI.BeginScrollView(scrollPosition);
            var lastSize = rect.size;
            rect = r;

            // 确保节点位置已计算
            if(rootNode != null && (rootNode.Rect.position == Vector2.zero || lastSize != r.size))
            {
                CalculateNodePositions();
            }

            // 绘制背景网格
            DrawGrid();

            // 绘制连接线
            DrawConnections(rootNode);

            // 绘制所有节点
            DrawNodes(rootNode);

            // 处理事件
            ProcessEvents(Event.current);

            //GUI.EndScrollView();

            //if (GUI.changed)
            //{
            //    Repaint();
            //}
        }

        private void DrawGrid()
        {
            int gridSize = 20;
            Color gridColor = new Color(0.2f, 0.2f, 0.2f, 0.3f);

            Handles.BeginGUI();
            Handles.color = gridColor;

            Rect visibleRect = new Rect(scrollPosition.x, scrollPosition.y, rect.width, rect.height);

            int widthDivs = Mathf.CeilToInt(visibleRect.width / gridSize);
            int heightDivs = Mathf.CeilToInt(visibleRect.height / gridSize);

            for (int i = 0; i < widthDivs; i++)
            {
                float x = visibleRect.x + i * gridSize;
                Handles.DrawLine(new Vector3(x, visibleRect.y, 0), new Vector3(x, visibleRect.y + visibleRect.height, 0));
            }

            for (int j = 0; j < heightDivs; j++)
            {
                float y = visibleRect.y + j * gridSize;
                Handles.DrawLine(new Vector3(visibleRect.x, y, 0), new Vector3(visibleRect.x + visibleRect.width, y, 0));
            }

            Handles.EndGUI();
        }

        private void DrawNodes(ITreeNode node)
        {
            if (node == null) return;

            // 绘制当前节点
            GUI.Box(node.Rect, node.Name, nodeStyle);

            // 递归绘制子节点
            foreach (var child in node.ChildNodes)
            {
                DrawNodes(child);
            }
        }

        private void DrawConnections(ITreeNode node)
        {
            if (node == null || node.ChildNodes.Count == 0) return;

            Handles.BeginGUI();
            Handles.color = Color.gray;

            foreach (var child in node.ChildNodes)
            {
                // 计算连线的起点和终点
                Vector3 startPos = new Vector3(node.Rect.center.x, node.Rect.yMax, 0);
                Vector3 endPos = new Vector3(child.Rect.center.x, child.Rect.yMin, 0);

                // 绘制连线
                Handles.DrawLine(startPos, endPos);

                // 递归绘制子节点的连线
                DrawConnections(child);
            }

            Handles.EndGUI();
        }

        private void ProcessEvents(Event e)
        {
            // 处理节点拖动
            if (draggingNode != null && e.type == EventType.MouseDrag && e.button == 0)
            {
                var rect = draggingNode.Rect;
                rect.position = e.mousePosition - dragOffset + scrollPosition;
                draggingNode.Rect = rect;
                draggingNode.IsDragging = true;
                GUI.changed = true;
            }
            else if (e.type == EventType.MouseUp && e.button == 0)
            {
                if (draggingNode != null)
                {
                    draggingNode.IsDragging = false;
                    draggingNode = null;
                }
            }
            // 处理节点点击
            else if (e.type == EventType.MouseDown && e.button == 0)
            {
                CheckNodeClick(e);
            }
        }

        protected void CheckNodeClick(Event e)
        {
            if (rootNode == null)
            {
                return;
            }

            // 转换鼠标位置到滚动视图坐标
            Vector2 mousePos = e.mousePosition + scrollPosition;

            // 从叶子节点开始检查，确保上层节点可以被点击
            CheckNodeClickRecursive(rootNode, mousePos);
        }

        protected bool CheckNodeClickRecursive(ITreeNode node, Vector2 mousePos)
        {
            // 先检查子节点
            foreach (var child in node.ChildNodes)
            {
                if (CheckNodeClickRecursive(child, mousePos))
                {
                    return true;
                }
            }

            // 再检查当前节点
            if (node.Rect.Contains(mousePos))
            {
                draggingNode = node;
                dragOffset = mousePos - node.Rect.position;
                OnNodeClick(node);
                return true;
            }

            return false;
        }
        
        // 计算所有节点的位置
        protected void CalculateNodePositions()
        {
            if(rootNode == null) return;

            CalculateTreeWidth(rootNode);

            // 根节点位置：窗口上方居中
            float rootX = rect.width * 0.5f;
            float rootY = nodeHeight * 0.5f;  // 距离顶部的距离
            rootNode.Rect = new Rect(rootX, rootY, nodeWidth, nodeHeight);

            // 递归计算所有子节点的位置
            PositionChildNodes(rootNode);
        }

        // 计算树的总宽度
        protected void CalculateTreeWidth(ITreeNode node)
        {
            if(node.ChildNodes.Count <= 1)
            {
                node.TotalWidth = nodeWidth;
                return;
            }

            // 子节点总宽度
            float totalWidth = 0;
            foreach(var child in node.ChildNodes)
            {
                CalculateTreeWidth(child);
                totalWidth += child.TotalWidth;
            }

            // 加上子节点之间的间距
            totalWidth += (node.ChildNodes.Count - 1) * horizontalSpacing;
            node.TotalWidth = totalWidth;
        }

        // 定位子节点
        private void PositionChildNodes(ITreeNode parentNode)
        {
            if(parentNode.ChildNodes.Count == 0) return;

            // 计算所有子节点占据的总宽度
            float totalWidth = parentNode.TotalWidth;

            // 计算起始X位置（使子节点整体居中于父节点下方）
            float startX = parentNode.Rect.center.x - (totalWidth / 2);
            float currentX = startX;
            float currentY = parentNode.Rect.yMax + verticalSpacing;

            // 定位每个子节点
            foreach(var child in parentNode.ChildNodes)
            {
                float childWidth = child.TotalWidth;
                // 子节点水平居中
                float childX = currentX + (childWidth / 2) - (nodeWidth / 2);

                child.Rect = new Rect(childX, currentY, nodeWidth, nodeHeight);

                // 移动到下一个子节点的位置
                currentX += childWidth + horizontalSpacing;

                // 递归定位子节点的子节点
                PositionChildNodes(child);
            }
        }

        protected virtual void OnNodeClick(ITreeNode node)
        {
            onNodeClick?.Invoke(node);
        }
    }
}