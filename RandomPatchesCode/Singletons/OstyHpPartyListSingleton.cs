using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Multiplayer;

namespace RandomPatches.RandomPatchesCode.Singletons;

public class OstyHpPartyListSingleton() : CustomSingletonModel(HookType.Combat)
{
    public static readonly Vector2 OstyHpBarPosition = new (63, 58);
    
    public const string Torchhead = "Collector.CollectorCode.Core.TorchheadMonsterModel";
    
    public const string Doloris = "AveMujica.AveMujicaCode.Cards.Dolls.DolorisDoll";
    public const string Mortis = "AveMujica.AveMujicaCode.Cards.Dolls.MortisDoll";
    public const string Timoris = "AveMujica.AveMujicaCode.Cards.Dolls.TimorisDoll";
    public const string Amoris = "AveMujica.AveMujicaCode.Cards.Dolls.AmorisDoll";
    public static readonly ModelId MortisPower = new ("POWER", "AVEMUJICA-DO_NOT_FEAR_DEATH_POWER");

    public static readonly ModelId GotYourBackPower = new("POWER", "THEHEROEXPANSION-GOT_YOUR_BACK_POWER");
    
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

        PossiblyChangeWhoIsTanking(creature);
        
        return Task.CompletedTask;
    }

    public override Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (!IsOstyLike(creature)) return Task.CompletedTask;
        
        PossiblyChangeWhoIsTanking(creature);
        
        var state = NRun.Instance!.GlobalUi.MultiplayerPlayerContainer._nodes
            .Find(s => 
                s.Player == creature.PetOwner);
        if (state == null) return Task.CompletedTask;
        
        UpdateOstyValues(state);
        
        return Task.CompletedTask;
    }

    public override Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (!IsOstyLike(creature)) return Task.CompletedTask;

        PossiblyChangeWhoIsTanking(creature);
        
        return Task.CompletedTask;
    }

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power.Id != MortisPower && power.Id != GotYourBackPower) return Task.CompletedTask;
        
        if (power.Id == GotYourBackPower && power.Owner.Player != null)
        {
            PossiblyChangeWhoIsTanking(power.Owner, power.Owner.Player);
        }
        else
        {
            PossiblyChangeWhoIsTanking(power.Owner);
        }
        
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
            X = Math.Max(
                OstyHpBar[state]._creature.MaxHp / 80f * 175f,
                (OstyHpBar[state]._creature.MaxHp + 20) / 120f * 175f
            )
        });
        
        OstyHpBar[state].RefreshValues();
    }

    public static void PossiblyChangeWhoIsTanking(Creature somePet, Player? player = null)
    {
        player ??= somePet.PetOwner;
        
        var state = NRun.Instance!.GlobalUi.MultiplayerPlayerContainer._nodes
            .Find(s => 
                s.Player == player);
        if (state == null) return;

        Creature? ostyLike = GetWhichOstyLikeWillTank(player);
        
        if (ostyLike != null && OstyHpBar[state]._creature != ostyLike)
        {
            CreateOstyBar(state, ostyLike);
        }

        if (ostyLike == null)
        {
            OstyHpBar[state].Visible = false;
            OstyHpBar[state]._creature = null!;
        }
    }

    public static bool IsOstyLike(Creature creature)
    {
        return
            (
                creature.Monster is Osty &&
                ((creature.PetOwner?.Creature.GetPower(GotYourBackPower)?.Amount ?? 0) == 0)
            ) ||
            creature.Monster?.GetType().FullName == Torchhead ||
            (
                creature.Monster?.GetType().FullName == Mortis &&
                ((creature.GetPower(MortisPower)?.Amount ?? 0) > 0)
            );
    }

    public static Creature? GetWhichOstyLikeWillTank(Player? player)
    {
        return player?.Creature.Pets.FirstOrDefault(c => IsOstyLike(c) && c.IsAlive);
    }
}