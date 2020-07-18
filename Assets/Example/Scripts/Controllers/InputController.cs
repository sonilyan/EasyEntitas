using Entitas;
using UnityEngine;

public class InputController : UnityEngine.MonoBehaviour, Context.IListener<InputComponent>
{
	public void OnAdd(Entity entity, InputComponent component)
	{
	}

	private void Start()
	{
		Contexts.Get<Input>().AddListener<InputComponent>(this);
	}

	void Update()
	{
		var c = Contexts.Get<Input>().Get<BurstModeComponent>();

		if (UnityEngine.Input.GetKeyDown("b"))
		{
			Contexts.Get<Input>().Replace<BurstModeComponent>((newValue) => { newValue.value = !c.value; });
		}

		var input = c.value
			? UnityEngine.Input.GetMouseButton(0)
			: UnityEngine.Input.GetMouseButtonDown(0);

		if (input)
		{
			var hit = UnityEngine.Physics2D.Raycast(
				UnityEngine.Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition),
				UnityEngine.Vector2.zero, 100);
			if (hit.collider != null)
			{
				var pos = hit.collider.transform.position;

				Contexts.Get<Input>().CreateEntity().Add<InputComponent>(x =>
				{
					x.x = (int) pos.x;
					x.y = (int) pos.y;
				});
			}
		}
	}
}
