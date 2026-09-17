using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;

namespace RandomPatches.RandomPatchesCode.Singletons;

public class OstyHpPartyListSingleton() : CustomSingletonModel(HookType.Combat)
{
    public static readonly Vector2 OstyHpBarPosition = new (63, 58);
    public const string Torchhead = "Collector.CollectorCode.Core.TorchheadMonsterModel";
    
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
        if (!IsOstyLike(creature)) return Task.CompletedTask;
        if (creature.PetOwner == null) return Task.CompletedTask;

        var state = NRun.Instance!.GlobalUi.MultiplayerPlayerContainer._nodes
            .Find(s => 
                s.Player == creature.PetOwner);
        if (state == null) return Task.CompletedTask;

        if (GetWhichOstyLikeWillTank(creature.PetOwner) == creature)
        {
            CreateOstyBar(state, creature);
        }
        
        return Task.CompletedTask;
    }

    public override Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (!IsOstyLike(creature)) return Task.CompletedTask;
        
        var state = NRun.Instance!.GlobalUi.MultiplayerPlayerContainer._nodes
            .Find(s => 
                s.Player == creature.PetOwner);
        if (state == null) return Task.CompletedTask;
        
        if (GetWhichOstyLikeWillTank(creature.PetOwner) == creature &&
            OstyHpBar[state]._creature != creature)
        {
            CreateOstyBar(state, creature);
        }
        
        UpdateOstyValues(state);
        
        return Task.CompletedTask;
    }

    public override Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (!IsOstyLike(creature)) return Task.CompletedTask;
        
        var state = NRun.Instance!.GlobalUi.MultiplayerPlayerContainer._nodes
            .Find(s => 
                s.Player == creature.PetOwner);
        if (state == null) return Task.CompletedTask;
        
        if (OstyHpBar[state]._creature != creature) return Task.CompletedTask;

        Creature? nextOstyLike = GetWhichOstyLikeWillTank(creature.PetOwner);

        if (nextOstyLike != null) CreateOstyBar(state, nextOstyLike);
        
        return Task.CompletedTask;
    }

    public static void CreateOstyBar(NMultiplayerPlayerState state, Creature creature)
    {
        OstyHpBar[state]._creature = null!;
        OstyHpBar[state].SetCreature(creature);
        
        OstyHpBar[state].FadeOutHpLabel(0, 0);
        OstyHpBar[state]._hpLabel.ZIndex = 1;

        state.MoveChild(OstyHpBar[state], 0);

        UpdateOstyValues(state);
        OstyHpBar[state].Visible = true;
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

    public static bool IsOstyLike(Creature creature)
    {
        return creature.Monster is Osty ||
               creature.Monster?.GetType().FullName == Torchhead;
    }

    public static Creature? GetWhichOstyLikeWillTank(Player? player)
    {
        return player?.Creature.Pets.FirstOrDefault(c => IsOstyLike(c) && c.IsAlive);
    }
}