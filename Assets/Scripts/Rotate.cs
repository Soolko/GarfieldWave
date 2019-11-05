using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEditor;
using static Unity.Mathematics.math;
using quaternion = Unity.Mathematics.quaternion;

namespace GarfieldWave
{
	[AddComponentMenu("Garfield Wave/Rotate"), RequiresEntityConversion]
	public class Rotate : MonoBehaviour, IConvertGameObjectToEntity
	{
		[Header("Settings")]
		public float speed = 5.0f;
		public float3 direction = Vector3.right;
		
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			RotateSystem.Data data = new RotateSystem.Data
			{
				speed = radians(this.speed),
				direction = this.direction
			};
			dstManager.AddComponentData(entity, data);
		}
		
		#if UNITY_EDITOR
		private void OnValidate()
		{
			direction = clamp(direction, -1.0f, 1.0f);
		}
		#endif
		
		#if UNITY_EDITOR
		private void OnDrawGizmosSelected()
		{
			const float Radius = 5.0f;
			float3 position = transform.position;
			
			Handles.color = Color.red;
			float3 dir = normalize(direction);
			float3 right = rotate(Quaternion.AngleAxis(90.0f, Vector3.right), dir);
			Handles.DrawWireArc(position, normalize(direction), right, 360.0f, Radius);
			
			float3 target = position + (right * speed);
			Handles.DrawAAPolyLine(1.0f, position, target);
		}
		#endif
	}
	
	internal class RotateSystem : ComponentSystem
	{
		[System.Serializable]
		public struct Data : IComponentData
		{
			public float speed;
			public float3 direction;
		}
		
		protected override void OnUpdate()
		{
			Entities.ForEach((ref Data data, ref Rotation rotation) =>
			{
				float delta = Time.deltaTime;
				rotation.Value = mul
				(
					normalize(rotation.Value),
					quaternion.AxisAngle
					(
						data.direction,
						data.speed * delta
					)
				);
			});
		}
	}
}