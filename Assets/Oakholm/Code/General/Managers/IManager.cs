namespace Oakholm {
	public interface IManager {
		void OnCreate();
		void OnGameSetupComplete();
		void OnUpdate();
		void OnClose();
	}
}
