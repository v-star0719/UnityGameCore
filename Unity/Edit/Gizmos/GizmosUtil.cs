using System;
using UnityEngine;

namespace GameCore.Edit
{
	public static class GizmosUtils
	{
		public static void DrawPlane(Vector3 position, Quaternion rotation, Vector2 size, Color color, Color colorInner)
		{
			using(GizmosMatrix(Matrix4x4.TRS(position, rotation, UnityEngine.Vector3.one)))
			{
				// 画四条线
                using(GizmosColor(color))
				{
                    Gizmos.DrawLine(new UnityEngine.Vector3(size.x / 2, 0, size.y / 2), new UnityEngine.Vector3(size.x / 2, 0, -size.y / 2));
					Gizmos.DrawLine(new UnityEngine.Vector3(size.x / 2, 0, -size.y / 2), new UnityEngine.Vector3(-size.x / 2, 0, -size.y / 2));
					Gizmos.DrawLine(new UnityEngine.Vector3(-size.x / 2, 0, -size.y / 2), new UnityEngine.Vector3(-size.x / 2, 0, size.y / 2));
					Gizmos.DrawLine(new UnityEngine.Vector3(-size.x / 2, 0, size.y / 2), new UnityEngine.Vector3(size.x / 2, 0, size.y / 2));
				}
				// 画里面的
                using(GizmosColor(colorInner))
				{
					Gizmos.DrawCube(UnityEngine.Vector3.zero, new UnityEngine.Vector3(size.x, 0, size.y));
				}
			}
		}

        //draw round on y=0 panel
        public static void DrawRound(Vector3 center, float r)
        {
            Vector3 beginPoint = Vector3.zero;
            Vector3 firstPoint = Vector3.zero;
            for (float theta = 0; theta < 2 * Mathf.PI; theta += Mathf.Deg2Rad)
            {
                float x = r * Mathf.Cos(theta);
                float z = r * Mathf.Sin(theta);
                Vector3 endPoint = new Vector3(x, 0, z);
                if (theta == 0)
                {
                    firstPoint = endPoint;
                }
                else
                {
                    Gizmos.DrawLine(beginPoint + center, endPoint + center);
                }
                beginPoint = endPoint;
            }

            // 绘制最后一条线段
            Gizmos.DrawLine(firstPoint, beginPoint);
        }

		public static void DrawCapsule(Vector3 position, Quaternion rotation, float height, float radius, Color color, Color colorInner)
		{
			using(GizmosMatrix(Matrix4x4.TRS(position, UnityEngine.Quaternion.identity, UnityEngine.Vector3.one)))
			{
				height = Mathf.Max(0, height - radius * 2);
				using(GizmosColor(color))
				{
					var r = radius * 0.52532f;
					// 画四条竖线
					{
						Gizmos.DrawLine(new UnityEngine.Vector3(radius, height / 2, 0), new UnityEngine.Vector3(radius, -height / 2, 0));
						Gizmos.DrawLine(new UnityEngine.Vector3(-radius, height / 2, 0), new UnityEngine.Vector3(-radius, -height / 2, 0));
						Gizmos.DrawLine(new UnityEngine.Vector3(0, height / 2, radius), new UnityEngine.Vector3(0, -height / 2, radius));
						Gizmos.DrawLine(new UnityEngine.Vector3(0, height / 2, -radius), new UnityEngine.Vector3(0, -height / 2, -radius));
					}
					// 再画四条竖线
					{
						Gizmos.DrawLine(new UnityEngine.Vector3(r, height / 2, r), new UnityEngine.Vector3(r, -height / 2, r));
						Gizmos.DrawLine(new UnityEngine.Vector3(-r, height / 2, -r), new UnityEngine.Vector3(-r, -height / 2, -r));
						Gizmos.DrawLine(new UnityEngine.Vector3(r, height / 2, -r), new UnityEngine.Vector3(r, -height / 2, -r));
						Gizmos.DrawLine(new UnityEngine.Vector3(-r, height / 2, r), new UnityEngine.Vector3(-r, -height / 2, r));
					}
					// 上下各画一个圆两个半圆
					{
						//UnityEngine.Vector3 h = new UnityEngine.Vector3(0, height / 2, 0);
						//Handles.color = color;
						//Handles.matrix = Gizmos.matrix;
						//Handles.DrawWireArc(h, UnityEngine.Vector3.up, UnityEngine.Vector3.right, 360, radius);
						//Handles.DrawWireArc(-h, UnityEngine.Vector3.up, UnityEngine.Vector3.right, 360, radius);
						//Handles.DrawWireArc(h, UnityEngine.Vector3.left, UnityEngine.Vector3.forward, 180, radius);
						//Handles.DrawWireArc(h, UnityEngine.Vector3.forward, UnityEngine.Vector3.right, 180, radius);
						//Handles.DrawWireArc(-h, UnityEngine.Vector3.left, UnityEngine.Vector3.back, 180, radius);
						//Handles.DrawWireArc(-h, UnityEngine.Vector3.back, UnityEngine.Vector3.right, 180, radius);
					}
				}
			}
		}

		public static void DrawCylinder(Vector3 position, Quaternion rotation, float height, float radius, Color color, Color colorInner)
		{
			using(GizmosMatrix(Matrix4x4.TRS(position, UnityEngine.Quaternion.identity, UnityEngine.Vector3.one)))
			{
				using(GizmosColor(color))
				{
					// 以下是画圆柱体的代码
					var r = radius * 0.52532f;
					// 画四条竖线
					{
						Gizmos.DrawLine(new UnityEngine.Vector3(radius, height / 2, 0), new UnityEngine.Vector3(radius, -height / 2, 0));
						Gizmos.DrawLine(new UnityEngine.Vector3(-radius, height / 2, 0), new UnityEngine.Vector3(-radius, -height / 2, 0));
						Gizmos.DrawLine(new UnityEngine.Vector3(0, height / 2, radius), new UnityEngine.Vector3(0, -height / 2, radius));
						Gizmos.DrawLine(new UnityEngine.Vector3(0, height / 2, -radius), new UnityEngine.Vector3(0, -height / 2, -radius));
					}
					// 再画四条竖线
					{
						Gizmos.DrawLine(new UnityEngine.Vector3(r, height / 2, r), new UnityEngine.Vector3(r, -height / 2, r));
						Gizmos.DrawLine(new UnityEngine.Vector3(-r, height / 2, -r), new UnityEngine.Vector3(-r, -height / 2, -r));
						Gizmos.DrawLine(new UnityEngine.Vector3(r, height / 2, -r), new UnityEngine.Vector3(r, -height / 2, -r));
						Gizmos.DrawLine(new UnityEngine.Vector3(-r, height / 2, r), new UnityEngine.Vector3(-r, -height / 2, r));
					}
					// 画上下两个圆
					{
						//UnityEngine.Vector3 h = new UnityEngine.Vector3(0, height / 2, 0);
						//Handles.color = color;
						//Handles.matrix = Gizmos.matrix;
						//Handles.DrawWireArc(h, UnityEngine.Vector3.up, UnityEngine.Vector3.right, 360, radius);
						//Handles.DrawWireArc(-h, UnityEngine.Vector3.up, UnityEngine.Vector3.right, 360, radius);
					}
				}
			}
		}

		public static void DrawCube(Vector3 position, Quaternion rotation, Vector3 size, Color color, Color colorInner)
		{
			using(GizmosMatrix(Matrix4x4.TRS(position, rotation, UnityEngine.Vector3.one)))
			{
				using(GizmosColor(color))
				{
					Gizmos.DrawWireCube(UnityEngine.Vector3.zero, size);
					Gizmos.DrawCube(UnityEngine.Vector3.zero, size);
				}
			}
		}

		public static void DrawBall(Vector3 position, Quaternion rotation, float radius, Color color, Color colorInner)
		{
			using(GizmosMatrix(Matrix4x4.TRS(position, rotation, UnityEngine.Vector3.one)))
			{
				using(GizmosColor(color))
				{
					Gizmos.DrawWireSphere(UnityEngine.Vector3.zero, radius);
					Gizmos.DrawSphere(UnityEngine.Vector3.zero, radius);
				}
			}
		}

		public static IDisposable GizmosMatrix(Matrix4x4 mtr)
		{
			return new GizmosDrawMatrix(mtr);
		}

		public static IDisposable GizmosColor(Color clr)
		{
			return new GizmosDrawColor(clr);
		}

		private class GizmosDrawMatrix : IDisposable
		{
			private readonly Matrix4x4 matrixGizmos;

			public GizmosDrawMatrix(Matrix4x4 mtr)
			{
				matrixGizmos = Gizmos.matrix;
                Gizmos.matrix = mtr;
            }

			public void Dispose()
			{
				Gizmos.matrix = matrixGizmos;
			}
		}

		private class GizmosDrawColor : IDisposable
		{
			private readonly Color gizmosClr;
			public GizmosDrawColor(Color clr)
			{
				gizmosClr = Gizmos.color;
				Gizmos.color = clr;
			}
			public void Dispose()
			{
				Gizmos.color = gizmosClr;
			}
		}
	}
}