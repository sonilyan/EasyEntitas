using Entitas;
public sealed class GameBoardSystems : Feature {

    public GameBoardSystems() {
        Add(new GameBoardSystem());
        Add(new FallSystem());
        Add(new FillSystem());
    }
}
