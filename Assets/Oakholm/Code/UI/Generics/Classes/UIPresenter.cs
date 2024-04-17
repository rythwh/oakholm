namespace Oakholm.UI {
	public abstract class UIPresenter<TView> : IUIPresenter where TView : UIView {

		protected TView View { get; }

		protected UIPresenter(TView view) {
			View = view;
		}

		public abstract void OnCreate();

		public abstract void OnClose();
	}
}
