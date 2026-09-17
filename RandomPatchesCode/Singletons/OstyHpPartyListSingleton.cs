using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;

namespace RandomPatches.RandomPatchesCode.Singletons;

public class OstyHpPartyListSingleton() : CustomSingletonModel(HookType.Combat)
{
    public static readonly Vector2 OstyHpBarPosition = new (63, 58);
    
    public static AddedNode<NMultiplayerPlayerState, NHealthBar> OstyHpBar = new(state =>
    {
        var hpBar = (NHealthBar) state._healthBar.Duplicate();
        
        hpBar.Position = OstyHpBarPosition;
        
        hpBar._blockTrackingCreature = state.Player.Creature;
        hpBar.Visible = false;

        return hpBar;
    });

    public override Task AfterCreatureAddedToCombat(Creature creature)
    {
        if (creature.Monster is not Osty) return Task.CompletedTask;

        var state = NRun.Instance!.GlobalUi.MultiplayerPlayerContainer._nodes
            .Find(s => 
                s.Player == creature.PetOwner);
        if (state == null) return Task.CompletedTask;

        OstyHpBar[state]._creature = null!;
        OstyHpBar[state].SetCreature(creature);
        
        OstyHpBar[state].FadeOutHpLabel(0, 0);
        OstyHpBar[state]._hpLabel.ZIndex = 1;

        state.MoveChild(OstyHpBar[state], 0);

        UpdateOstyValues(state);
        OstyHpBar[state].Visible = true;
        return Task.CompletedTask;
    }

    public override Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (creature.Monster is not Osty) return Task.CompletedTask;
        
        var state = NRun.Instance!.GlobalUi.MultiplayerPlayerContainer._nodes
            .Find(s => 
                s.Player == creature.PetOwner);

        if (state == null) return Task.CompletedTask;
        
        UpdateOstyValues(state);
        
        return Task.CompletedTask;
    }

    public static void UpdateOstyValues(NMultiplayerPlayerState state)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (OstyHpBar[state]._creature == null) return;
        
        OstyHpBar[state].SetHpBarContainerSizeWithOffsetsImmediately(OstyHpBar[state].HpBarContainer.Size with
        {
            X = Math.Max(OstyHpBar[state]._creature.MaxHp / 80f * 175f,
                         15 / 80f * 175f)
        });
        
        OstyHpBar[state].RefreshValues();
    }
}