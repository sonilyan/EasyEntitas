using Entitas;

public sealed class AssetComponent : IComponent {
    public string value;

	public void SetValue(string newValue) {
		value = newValue;
	}
}
