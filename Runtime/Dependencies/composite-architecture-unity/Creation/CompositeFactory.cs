using System;
using UnityEngine;

namespace CompositeArchitecture
{
	[Serializable]
	public abstract class CompositeFactory<T> : CompositeElement
		where T : MonoBehaviour, ICompositeElement
	{
		public Observable<T> OnSpawned = new();
		public Observable<T> OnDespawned = new();

		[SerializeField] public Transform ParentTransform;
		[SerializeField] public T Prefab;

		private InstantiationHandler _instantiationHandler;

		protected override void OnInstall(DependencyInjectionContainer diContainer)
		{
			Debug.Assert(Prefab != null, $"Failed installing factory for <b><color=#ff0000>{typeof(T)}</color></b>! Prefab is null!");
		}

		protected override void OnInject ()
		{
			Resolve(out _instantiationHandler);
		}

		public virtual T Spawn(bool activate = true, Action<T> onBeforeInstall = null)
		{
			var spawnedElement = _instantiationHandler.Instantiate(Prefab, ParentTransform);
			Initialize(spawnedElement, activate, onBeforeInstall);
			return spawnedElement;
		}

		private T Initialize(T spawnedElement, bool activate, Action<T> onBeforeInstall)
		{
			spawnedElement.gameObject.SetActive(true);
			onBeforeInstall?.Invoke(spawnedElement);
			var lifecycleFlags = activate ? LifecycleFlags.All : LifecycleFlags.WithoutActivate;
			InstallChild(spawnedElement, lifecycleFlags, BindingMode.Injectable, true);

			return spawnedElement;
		}

		public virtual void Despawn(T instance)
		{
			OnDespawned.Invoke(instance);
			UninstallChild(instance);
		}
	}
}