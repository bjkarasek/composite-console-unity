using UnityEngine;

namespace CompositeArchitecture
{
	public class InstantiationHandler
	{
		public Observable<GameObject> OnObjectInstantiated = new();
		public Observable<GameObject> OnObjectDestroyed = new();

		public virtual T Instantiate<T> (T original, Transform parent = default, Vector3 position = default, Quaternion rotation = default)
			where T : Behaviour
		{
			var obj = Object.Instantiate(original, parent);
			var transform = obj.transform;
			transform.localPosition = position;
			transform.localRotation = rotation;
			OnObjectInstantiated.Invoke(obj.gameObject);
			return obj;
		}

		public virtual void Destroy<T> (T instance)
			where T : Behaviour
		{
			OnObjectDestroyed.Invoke(instance.gameObject);
			Object.Destroy(instance);
		}
	}
}