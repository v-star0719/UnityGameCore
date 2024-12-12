using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCore;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameCore.Core
{
    [ExecuteInEditMode]
    public class TestGridCellMerge : MonoBehaviour
    {
        public bool test;
        public int randomCount;
        public bool randomCells;
        public bool fixedCells;
        public List<Vector2Int> cells = new List<Vector2Int>();
        public List<RectInt> rects = new List<RectInt>();


        private void Start()
        {
        }

        public void Update()
        {
            if (test)
            {
                test = false;
                rects = GridUtils.MergeGrids(cells);
            }

            if (randomCells)
            {
                randomCells = false;
                cells.Clear();
                rects.Clear();
                HashSet<int> exist = new HashSet<int>();
                for (int i = 0; i < randomCount; i++)
                {
                    var x = Random.Range(0, 10);
                    var y = Random.Range(0, 10);
                    var key = x * 1000 + y;
                    if (exist.Contains(key))
                    {
                        i--;
                        continue;
                    }

                    exist.Add(key);
                    cells.Add(new Vector2Int(x, y));
                }
            }

            if (fixedCells)
            {
                fixedCells = false;
                cells.Clear();
                rects.Clear();
                cells.Add(new Vector2Int(0, 0));
                cells.Add(new Vector2Int(0, 1));
                cells.Add(new Vector2Int(1, 0));
                cells.Add(new Vector2Int(1, 1));

                cells.Add(new Vector2Int(2, 1));
                cells.Add(new Vector2Int(2, 2));

                cells.Add(new Vector2Int(3, 0));
            }
        }

        public void OnDrawGizmos()
        {
            foreach (var cell in cells)
            {
                Gizmos.DrawWireCube(new Vector3(cell.x + 0.5f, 0, cell.y + 0.5f),  new Vector3(0.8f, 0, 0.8f));
            }

            foreach (var rect in rects)
            {
                Gizmos.DrawWireCube(new Vector3(rect.x + 0.5f * rect.size.x, 0, rect.y + 0.5f * rect.size.y),
                    new Vector3(rect.size.x - 0.1f, 0, rect.size.y - 0.1f));
            }
        }

    }
}
