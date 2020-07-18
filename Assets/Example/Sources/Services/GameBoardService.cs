using Entitas;

public class GameBoardService {

    public static GameBoardService singleton = new GameBoardService();

    public void Initialize() {
    }

    public int GetNextEmptyRow(IntVector2 position) {
        position.y -= 1;
        while (position.y >= 0) {
	        var count = PositionComponent.GetEntitiesWithValue(Contexts.Get<Game>(), position).Count;

			if (count != 0)
				break;
            position.y -= 1;
        }
        return position.y + 1;
    }
}
