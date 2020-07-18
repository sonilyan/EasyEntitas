using Entitas;
using UnityEngine;

public class EntityService {

    public RandomService randomService = RandomService.game;

    public static EntityService singleton = new EntityService();

    static readonly string[] _pieces = {
        Res.Piece0,
        Res.Piece1,
        Res.Piece2,
        Res.Piece3,
        Res.Piece4,
        Res.Piece5
    };

	public void Initialize() {

	}

	public Entity CreateGameBoard() {
	    var entity = Contexts.Get<Game>().Add<GameBoardComponent>(x => {
		    x.columns = 8;
		    x.rows = 9;
	    });
		return entity;
    }

    public Entity CreateRandomPiece(int x, int y) {
        var entity = Contexts.Get<Game>().CreateEntity();
        entity.Add<GameBoardElementComponent>();
        entity.Add<MovableComponent>();
        entity.Add<InteractiveComponent>();
	    entity.Add<PositionComponent>(nv => {
			nv.value = new IntVector2(x, y);
	    });
	    entity.Add<AssetComponent>((newValue) => {
		    newValue.value = randomService.Element(_pieces);
	    });
        return entity;
    }

	public Entity ClientCreatePiece(int eindex,int x, int y, string piece) {
		var entity = Contexts.Get<Game>().CreateEntity(eindex);
		entity.Add<GameBoardElementComponent>();
		entity.Add<PositionComponent>(nv => {
			nv.value = new IntVector2(x, y);
		});
		entity.Add<AssetComponent>((newValue) => {
			newValue.value = piece;
		});

		if (piece != Res.Blocker) {
			entity.Add<MovableComponent>();
			entity.Add<InteractiveComponent>();
		}

		return entity;
	}

	public Entity CreateBlocker(int x, int y) {
        var entity = Contexts.Get<Game>().CreateEntity();
	    entity.Add<GameBoardElementComponent>();
        entity.Add<PositionComponent>(nv=> {
	        nv.value = new IntVector2(x, y);
        });
        entity.Add<AssetComponent>(nv => {
	        nv.value = Res.Blocker;
        });
        return entity;
    }
}
