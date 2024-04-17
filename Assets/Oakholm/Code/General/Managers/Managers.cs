using System;
using System.Collections.Generic;
using System.Linq;
using Oakholm.UI;
using UnityEngine;

namespace Oakholm {
	public static class Managers {

		private static readonly HashSet<IManager> ManagersHashSet = new HashSet<IManager>();

		public static void CreateManagers(SceneReferenceProvider sceneReferenceProvider) {
			CreateManager<StateManager>();
			CreateManager<LoadingManager>(GetManager<StateManager>());
			CreateManager<UIManager>(sceneReferenceProvider, GetManager<StateManager>(), GetManager<LoadingManager>());
			CreateManager<CameraManager>(sceneReferenceProvider, GetManager<StateManager>());
			CreateManager<MapManager>(sceneReferenceProvider, GetManager<StateManager>(), GetManager<CameraManager>(), GetManager<LoadingManager>());

			foreach (IManager manager in ManagersHashSet) {
				manager.OnGameSetupComplete();
			}
		}

		public static void UpdateManagers() {
			foreach (IManager manager in ManagersHashSet) {
				manager.OnUpdate();
			}
		}

		public static void RemoveManagers() {
			RemoveManager<UIManager>();
		}

		private static TManager CreateManager<TManager>(params object[] args) where TManager : Manager {

			TManager manager;
			try {
				manager = (TManager)Activator.CreateInstance(typeof(TManager), args);
			} catch (MissingMethodException) {
				IReadOnlyList<string> argNames = args.Select(a => a.GetType().Name).ToList();
				IReadOnlyList<string> typeConstructorParameterNames = typeof(TManager).GetConstructors()[0].GetParameters().Select(p => p.ParameterType.Name).ToList();

				IReadOnlyList<string> unusedArgNames = argNames.Where(argName => !typeConstructorParameterNames.Contains(argName)).ToList();
				IReadOnlyList<string> missingTypeParameterNames = typeConstructorParameterNames.Where(paramName => !argNames.Contains(paramName)).ToList();

				if (missingTypeParameterNames.Count > 0) {
					Debug.LogError($"Mismatch of constructor parameters for {typeof(TManager).Name}: {string.Join(", ", missingTypeParameterNames)}");
				}
				if (unusedArgNames.Count > 0) {
					Debug.LogWarning($"Unused constructor arguments for {typeof(TManager).Name}: {string.Join(", ", unusedArgNames)}");
				}
				return null;
			}

			ManagersHashSet.Add(manager);
			manager.OnCreate();

			return manager;
		}

		public static TManager GetManager<TManager>() where TManager : IManager {
			foreach (IManager manager in ManagersHashSet) {
				if (manager is TManager managerInstance) {
					return managerInstance;
				}
			}

			throw new Exception($"{nameof(TManager)} does not exist when it is trying to be accessed.");
		}

		private static void RemoveManager<TManager>() where TManager : IManager {
			IManager foundManager = null;
			foreach (IManager manager in ManagersHashSet) {
				if (manager is TManager) {
					foundManager = manager;
				}
			}

			foundManager?.OnClose();
		}
	}
}
