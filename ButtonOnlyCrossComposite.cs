using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;
using Ryocatusn.TileTransforms;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
[DisplayName("Button Only Cross Composite")]
[DisplayStringFormat("{up}/{left}/{down}/{right}")]
public class ButtonOnlyCrossComposite : InputBindingComposite<Vector2>
{
    #if UNITY_EDITOR
    static ButtonOnlyCrossComposite()
    {
        Initialize();
    }
    #endif
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        InputSystem.RegisterBindingComposite<ButtonOnlyCrossComposite>();
    }

    [InputControl(layout = "Button")]
    public int up;
    [InputControl(layout = "Button")]
    public int down;
    [InputControl(layout = "Button")]
    public int left;
    [InputControl(layout = "Button")]
    public int right;

    private List<TileDirection.Direction> directions = new List<TileDirection.Direction>();

    public override Vector2 ReadValue(ref InputBindingCompositeContext context)
    {
        if (context.ReadValueAsButton(up)) AddDirection(TileDirection.Direction.Up);
        else RemoveDirection(TileDirection.Direction.Up);
        
        if (context.ReadValueAsButton(down)) AddDirection(TileDirection.Direction.Down);
        else RemoveDirection(TileDirection.Direction.Down);
        
        if (context.ReadValueAsButton(left)) AddDirection(TileDirection.Direction.Left);
        else RemoveDirection(TileDirection.Direction.Left);
        
        if (context.ReadValueAsButton(right)) AddDirection(TileDirection.Direction.Right);
        else RemoveDirection(TileDirection.Direction.Right);

        if (GetDirection() == null) return Vector2.zero;

        TileDirection.Direction direction = GetDirection() ?? TileDirection.Direction.Up;
        TileDirection humanDirection = new TileDirection(direction);
        return humanDirection.GetVector2();
    }
    public override float EvaluateMagnitude(ref InputBindingCompositeContext context)
    {
        return ReadValue(ref context).magnitude;
    }

    private void AddDirection(TileDirection.Direction direction)
    {
        if (directions.Contains(direction)) return;

        directions.Add(direction);
    }
    private void RemoveDirection(TileDirection.Direction direction)
    {
        if (!directions.Contains(direction)) return;

        directions.Remove(direction);
    }
    private TileDirection.Direction? GetDirection()
    {
        if (directions.Count == 0) return null;

        return directions[directions.Count - 1];
    }
}
