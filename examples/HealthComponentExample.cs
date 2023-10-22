using Godot;
using System;
using System.Collections.Generic;

public partial class HealthComponentExample : Node2D
{
	private HealthComponent _healthComponent;
	private ProgressBar _progressBar;
	private Label _healthPoints;
	private Button _reviveButton;
	private Button _damageButton;
	private Button _healthButton;
	private CheckBox _health_regen_checkbox;
	private CheckBox _invulnerable_checkbox;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_healthComponent = GetNode<HealthComponent>("GodotParadiseHealthComponent");
		_progressBar = GetNode<ProgressBar>("%ProgressBar");
		_healthPoints = GetNode<Label>("%HealthPoints");
		_reviveButton = GetNode<Button>("%ReviveButton");
		_damageButton = GetNode<Button>("%DamageButton");
		_healthButton = GetNode<Button>("%HealthButton");
		_health_regen_checkbox = GetNode<CheckBox>("%HealthRegenCheckBox");
		_invulnerable_checkbox = GetNode<CheckBox>("%InvulnerableCheckBox");

		Dictionary<string, float> healthInformation = _healthComponent.GetHealthPercent();
		_progressBar.Value = healthInformation["CurrentHealthPercentage"] * 100;

		float overflow = healthInformation["OverflowHealth"];
		_healthPoints.Text = $"{_healthComponent.CurrentHealth} + {overflow} overflow points";

		_health_regen_checkbox.ButtonPressed = _healthComponent.HealthRegen > 0;
		_reviveButton.Visible = _healthComponent.CurrentHealth == 0;

		_healthComponent.HealthChanged += OnHealthChanged;
		_healthComponent.InvulnerabilityChanged += OnInvulnerabilityChanged;

		_damageButton.Pressed += OnDamageButtonPressed;
		_healthButton.Pressed += OnHealthButtonPressed;
		_health_regen_checkbox.Toggled += OnHealthRegenCheckToggled;
		_invulnerable_checkbox.Toggled += OnInvulnerableCheckToggled;
		_reviveButton.Pressed += OnReviveButtonPressed;
	}

	private void OnDamageButtonPressed()
	{
		_healthComponent.Damage(15);
	}

	private void OnHealthButtonPressed()
	{
		_healthComponent.Health(10);
	}

	private void OnHealthChanged(int amount, int type)
	{
		Dictionary<string, float> healthInformation = _healthComponent.GetHealthPercent();

		_progressBar.Value = healthInformation["CurrentHealthPercentage"] * 100;
		float overflow = healthInformation["OverflowHealth"];

		_healthPoints.Text = $"{_healthComponent.CurrentHealth} + {overflow} overflow points";
		_reviveButton.Visible = _healthComponent.CurrentHealth == 0;
	}

	private void OnHealthRegenCheckToggled(bool buttonPressed)
	{
		_healthComponent.EnableHealthRegen(buttonPressed ? 5 : 0);
	}

	private void OnInvulnerableCheckToggled(bool buttonPressed)
	{
		_healthComponent.EnableInvulnerability(buttonPressed);
	}

	private void OnInvulnerabilityChanged(bool enabled)
	{
		_invulnerable_checkbox.ButtonPressed = enabled;
	}

	private void OnReviveButtonPressed()
	{
		_healthComponent.Health(_healthComponent.MaxHealth);
	}


}
