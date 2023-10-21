#if TOOLS
using Godot;

[Tool]
public partial class GodotParadiseHealthComponent : EditorPlugin
{
	public override void _EnterTree()
	{
		AddCustomType("GodotParadiseHealthComponent",
			"Node",
			GD.Load<Script>("res://addons/GodotParadiseHealthComponent/plugin.cs"),
			GD.Load<Texture2D>("res://addons/GodotParadiseHealthComponent/SuitHearts.svg")
		);
	}

	public override void _ExitTree() => RemoveCustomType("GodotParadiseHealthComponent");
}
#endif