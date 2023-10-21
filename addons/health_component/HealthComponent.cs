/*
## Created by https://github.com/GodotParadise organization with LICENSE MIT
# There are no restrictions on modifying, sharing, or using this component commercially
# We greatly appreciate your support in the form of stars, as they motivate us to continue our journey of enhancing the Godot community
# ***************************************************************************************
# This component provides complete control over damage handling and health management for the parent node. 
# It is commonly used with a "CharacterBody2D" but can also be applied to a "StaticRigidBody2D," 
# allowing you to add health management to objects like trees or in-game elements, 
##

*/

using Godot;
using System.Collections.Generic;

public partial class HealthComponent : Node
{
    public enum TYPES
    {
        DAMAGE,
        HEALTH,
        REGEN
    }
    [Signal]
    public delegate void HealthChangedEventHandler(int amount, TYPES type);
    [Signal]
    public delegate void InvulnerabilityChangedEventHandler(bool active);
    [Signal]
    public delegate void DiedEventHandler();

    [ExportGroup("Health parameters")]
    [Export]
    public int MaxHealth = 100;
    [Export]
    public float HealthOverflowPercentage = 0.0f;
    [Export]
    public int CurrentHealth
    {
        get { return _currentHealth; }
        set { _currentHealth = Mathf.Clamp(value, 0, MaxHealthOverflow); }
    }

    [ExportGroup("Additional behaviours")]
    [Export]
    public int HealthRegen = 0;
    [Export]
    public float HealthRegenTickTime = 1.0f;
    [Export]
    public bool IsInvulnerable = false;
    [Export]
    public float InvulnerabilityTime = 1.0f;


    public Timer InvulnerabilityTimer;
    public Timer HealthRegenTimer;

    private int _currentHealth = 100;
    public int MaxHealthOverflow { get { return MaxHealth + (int)(MaxHealth * HealthOverflowPercentage / 100); } }

    public override void _Ready()
    {
        EnableHealthRegen(HealthRegen, HealthRegenTickTime);
        EnableInvulnerability(IsInvulnerable, InvulnerabilityTime);

        HealthChanged += OnHealthChanged;
        Died += OnDied;
    }

    public void Health(int amount, TYPES type = TYPES.HEALTH)
    {
        amount = Mathf.Abs(amount);
        CurrentHealth += amount;

        EmitSignal(SignalName.HealthChanged, amount, (int)type);
    }

    public void Damage(int amount, TYPES type = TYPES.DAMAGE)
    {
        amount = Mathf.Abs(amount);
        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);

        EmitSignal(SignalName.HealthChanged, amount);
    }

    public bool CheckIsDead()
    {
        bool IsDead = CurrentHealth == 0;

        if (IsDead)
        {
            EmitSignal(SignalName.Died);
        }

        return IsDead;
    }

    public Dictionary<string, float> GetHealthPercent()
    {
        float CurrentHealthPercentage = Mathf.Snapped(CurrentHealth / MaxHealth, 0.01f);

        return new Dictionary<string, float>{
            {"CurrentHealthPercentage", Mathf.Min(CurrentHealthPercentage, 1.0f)},
            {"OverflowHealthPercentage", Mathf.Max(0.0f, CurrentHealthPercentage - 1.0f)},
            {"OverflowHealth", Mathf.Max(0, CurrentHealth - MaxHealth)}
        };
    }

    public void EnableInvulnerability(bool enable, float time = 1.0f)
    {
        CreateInvulnerabilityTimer(time);
        IsInvulnerable = enable;
        InvulnerabilityTime = time;

        if (IsInvulnerable)
        {
            if (InvulnerabilityTime > 0)
            {
                InvulnerabilityTimer.Start();
            }
        }
        else
        {
            InvulnerabilityTimer.Stop();
        }
    }

    public void EnableHealthRegen(int amount = 0, float time = 1.0f)
    {
        HealthRegen = amount;
        HealthRegenTickTime = time;

        CreateHealthRegenTimer(HealthRegenTickTime);

        if (CurrentHealth >= MaxHealth && HealthRegenTimer.TimeLeft > 0 || HealthRegen <= 0)
        {
            HealthRegenTimer.Stop();
            return;
        }

        if (HealthRegenTimer.IsStopped() && HealthRegen > 0)
        {
            HealthRegenTimer.Start();
        }
    }

    private void CreateHealthRegenTimer(float time = 1.0f)
    {

        if (HealthRegenTimer is not null)
        {
            if (HealthRegenTimer.WaitTime != time)
            {
                HealthRegenTimer.Stop();
                HealthRegenTimer.WaitTime = time;
            }
        }
        else
        {
            HealthRegenTimer = new()
            {
                Name = "HealthRegenTimer",
                WaitTime = time,
                OneShot = false,
                Autostart = false
            };

            AddChild(HealthRegenTimer);
            HealthRegenTimer.Timeout += OnHealthRegenTimerTimeout;
        }
    }

    private void CreateInvulnerabilityTimer(float time = 1.0f)
    {
        if (InvulnerabilityTimer is not null)
        {
            if (InvulnerabilityTimer.WaitTime != time)
            {
                InvulnerabilityTimer.Stop();
                InvulnerabilityTimer.WaitTime = time;
            }
        }
        else
        {
            InvulnerabilityTimer = new()
            {
                Name = "InvulnerabilityTimer",
                WaitTime = time,
                OneShot = true,
                Autostart = false
            };

            AddChild(InvulnerabilityTimer);
            InvulnerabilityTimer.Timeout += OnInvulnerabilityTimerTimeout;
        }
    }

    private void OnHealthChanged(int amount, TYPES type)
    {
        if (type == TYPES.DAMAGE)
        {
            EnableHealthRegen();
            Callable.From(CheckIsDead).CallDeferred();
        }
    }

    private void OnDied()
    {
        HealthRegenTimer.Stop();
        InvulnerabilityTimer.Stop();
    }

    private void OnHealthRegenTimerTimeout()
    {
        Health(HealthRegen, TYPES.REGEN);
    }

    private void OnInvulnerabilityTimerTimeout()
    {
        EnableInvulnerability(false);
    }
}