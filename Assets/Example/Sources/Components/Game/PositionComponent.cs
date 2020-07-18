using Entitas;

public sealed class PositionComponent : ILookupComponent<PositionComponent, IntVector2> {
	public IntVector2 value;

	public override IntVector2 GetLookupValue() {
		return value;
	}
}
