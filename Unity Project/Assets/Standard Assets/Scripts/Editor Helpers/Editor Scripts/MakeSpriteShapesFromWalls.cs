#if UNITY_EDITOR
using Extensions;
using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEngine.U2D;
using System.Collections.Generic;

namespace EternityEngine
{
	public class MakeSpriteShapesFromWalls : EditorScript
	{
		public SpriteShapeController spriteShapeControllerPrefab;
		public float insertPointDist;
		public float maxOff;
		
		public override void Do ()
		{
			Collider2D[] colliders = FindObjectsOfType<Collider2D>();
			List<Transform> wallsTransforms = new List<Transform>();
			foreach (Collider2D collider in colliders)
				if (collider.name.StartsWith("Wall"))
					wallsTransforms.Add(collider.transform);
			CompositeCollider2D compositeCollider = new GameObject().AddComponent<CompositeCollider2D>();
			Transform compositeColliderTrs = compositeCollider.transform;
			foreach (Transform wallTrs in wallsTransforms)
				wallTrs.SetParent(compositeColliderTrs);
			PhysicsShapeGroup2D physicsShapeGroup = new PhysicsShapeGroup2D();
			compositeCollider.GetShapes(physicsShapeGroup);
			for (int i = 0; i < physicsShapeGroup.shapeCount; i ++)
			{
				SpriteShapeController spriteShapeController = (SpriteShapeController) PrefabUtility.InstantiatePrefab(spriteShapeControllerPrefab);
				List<Vector2> corners = new List<Vector2>();
				physicsShapeGroup.GetShapeVertices(i, corners);
				Vector2 prevCorner = corners[corners.Count - 1];
				foreach (Vector2 corner in corners)
				{
					Vector2 toCorner = corner - prevCorner;
					for (float dist = 0; dist < toCorner.magnitude; dist += insertPointDist)
						spriteShapeController.spline.InsertPointAt(0, prevCorner + toCorner.normalized * dist + Random.insideUnitCircle * maxOff);
					prevCorner = corner;
				}
			}
			compositeColliderTrs.DetachChildren();
			GameManager.DestroyOnNextEditorUpdate (compositeCollider.gameObject);
			foreach (Transform wallTrs in wallsTransforms)
				wallTrs.gameObject.SetActive(false);
		}
	}
}
#else
namespace EternityEngine
{
	public class MakeSpriteShapesFromWalls : EditorScript
	{
	}
}
#endif