using System;
using System.Collections.Generic;
using UnityEngine;

namespace CompositeArchitecture
{
	[Serializable]
	public class CompositeSpawner<T> : CompositeFactory<T>
		where T : MonoBehaviour, ICompositeElement
	{
		public List<T> Elements;

		public override T Spawn(bool activate = true, Action<T> onBeforeInstall = null)
		{
			Elements.Add(base.Spawn(activate, onBeforeInstall));
			OnSpawned.Invoke(Elements[^1]);
			return Elements[^1];
		}

		public override void Despawn (T instance)
		{
			var index = Elements.IndexOf(instance);
			base.Despawn(instance);
			Elements.RemoveAt(index);
		}
	}
}