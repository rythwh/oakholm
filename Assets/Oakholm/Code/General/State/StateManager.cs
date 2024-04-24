using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Oakholm {

	public enum EState {
		Boot,
		MainMenu,
		Loading,
		Simulation
	}

	[UsedImplicitly]
	public class StateManager : Manager {

		public EState State { get; private set; }
		public event Action<(EState oldState, EState newState)> OnStateChanged;
		public event Action<EState> OnPostStateChanged;

		private readonly Dictionary<EState, List<EState>> validStateTransitions = new Dictionary<EState, List<EState>>() {
			{ EState.Boot, new() { EState.MainMenu } },
			{ EState.MainMenu, new() { EState.Loading } },
			{ EState.Loading, new() { EState.MainMenu, EState.Simulation } },
			{ EState.Simulation, new() { EState.Loading } },
		};

		public override void OnCreate() {
			SetState(EState.Boot);
		}

		public override void OnGameSetupComplete() {
			SetState(EState.MainMenu);
		}

		public void SetState(EState newState) {
			if (State == newState) {
				return;
			}

			if (!validStateTransitions[State].Contains(newState)) {
				throw new Exception($"Attempting to move to {newState} which is not a valid transition from current state {State}");
			}

			EState oldState = State;
			State = newState;
			OnStateChanged?.Invoke((oldState, newState));
			OnPostStateChanged?.Invoke(State);
		}
	}
}
