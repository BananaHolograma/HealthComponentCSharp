#if TOOLS
using Godot;
using System;

[Tool]
public partial class GodotParadiseHealthComponentPlugin : EditorPlugin
{
	public override void _EnterTree()
	{
		AddCustomType("GodotParadiseHealthComponent",
			"Node",
			GD.Load<Script>("res://addons/health_component/HealthComponent.cs"), //.CS files needs to be on lowercase to prevent Godot errors
			GD.Load<Texture2D>("res://addons/health_component/suitheart.svg")
		);
	}

	public override void _ExitTree() => RemoveCustomType("GodotParadiseHealthComponent");
}
#endif