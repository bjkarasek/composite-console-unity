using System;
using UnityEngine;

namespace CompositeArchitecture
{
	[Serializable]
	public class CompositeInstantiator<T> : CompositeFactory<T>
		where T : MonoBehaviour, ICompositeElement
	{
		public T Element;

		public override T Spawn(bool activate = true, Action<T> onBeforeInstall = null)
		{
			if (Element == null)
			{
				Element = base.Spawn(activate, onBeforeInstall);
				OnSpawned.Invoke(Element);
			}
			else
			{
				Debug.LogError($"Trying to spawn {typeof(T).Name}, but it's already spawned!");
			}
			
			return Element;
		}

		public override void Despawn (T instance)
		{
			base.Despawn(instance);
			Element = default;
		}
	}
}