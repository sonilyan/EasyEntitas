using Entitas;

public sealed class GameSystems : Feature {

    public GameSystems() {

        // Input
        Add(new InputSystem());

        // Update
        Add(new GameBoardSystems());
        Add(new ScoreSystem());

		//Event
	    Add(new Feature(ListenerSystem.Instance));
        // Cleanup
        Add(new DestroyEntitySystem());
    }
}
