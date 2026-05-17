using IL.MoreSlugcats;
using RWCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ParkourScugPlugin.RevenantAbilities
{
    public class HunterProgram : MechanicalProgram
    {
        private int originalThrowingSkill;


        protected override void ConnectToPlayer()
        {
            originalThrowingSkill = player.slugcatStats.throwingSkill;
            player.slugcatStats.throwingSkill = 3;
            player.spearOnBack = new Player.SpearOnBack(player);
        }
        protected override void DisconnectFromPlayer()
        {
            player.slugcatStats.throwingSkill = originalThrowingSkill;
            player.spearOnBack = null;
            originalThrowingSkill = -1;
        }
        protected override void PlayerTick()
        {
            if (UnityEngine.Random.value < 0.01f)
            {
                player.room.AddObject(new Explosion.ExplosionLight(player.firstChunk.pos, 120f, 1f, 30, new Color(135 / 256f, 107 / 256f, 31 / 256f)));
                player.room.PlaySound(SoundID.Death_Lightning_Spark_Object, player.firstChunk.pos, 0.2f, 1f);
            }
        }
        protected override void Throw(Creature.Grasp grasp)
        {
            if (PlayerInput.y == 1)
            {
                player.animation = Player.AnimationIndex.Flip;
            }
        }


        public HunterProgram() : base() { }
        public HunterProgram(Player player) : base(player) { }
        public HunterProgram(Creature creature) : base(creature) { }
    }


    public abstract class MechanicalProgram
    {
        public bool inPlayer;
        protected Player.InputPackage PlayerInput => player.input[0];
        protected bool abstracted = false;
        protected Player player;
        protected ParkourScugData playerData;
        protected Creature creature;
        public Creature Owner
        {
            get
            {
                return creature;
            }
            set
            {
                if (value is Player)
                {
                    ConnectPlayer(value as Player);
                }
                else
                {
                    creature = value;
                    inPlayer = false;
                }
            }
        }


        public MechanicalProgram()
        {
            inPlayer = false;
            abstracted = true;
        }
        public MechanicalProgram(Creature creature)
        {
            this.creature = creature;
            inPlayer = false;
        }
        public MechanicalProgram(Player player)
        {
            ConnectPlayer(player);
        }
        public void ConnectPlayer(Player player)
        {
            this.player = player;
            creature = player;
            inPlayer = true;
            playerData = ParkourScugPlugin.GetParkourScugData(player);
            ConnectToPlayer();
        }
        public void Disconnect()
        {
            if (inPlayer)
            {
                DisconnectFromPlayer();
            }
            player = null;
            creature = null;
            inPlayer = false;
            abstracted = true;
        }


        public void Update()
        {
            if (abstracted || creature == null) { return; }
            Tick();
            if (inPlayer)
            {
                PlayerTick();
                if (player.input[0].spec)
                {
                    UseAbility();
                }
            }
            else
            {
                CreatureTick();
            }
        }
        public void ThrowObject(Creature.Grasp grasp)
        {
            Throw(grasp);
        }
        protected virtual void ConnectToPlayer() { }
        protected virtual void DisconnectFromPlayer() { }
        protected virtual void Tick() { }
        protected virtual void PlayerTick() { }
        protected virtual void CreatureTick() { }
        protected virtual void UseAbility() { }
        protected virtual void Throw(Creature.Grasp grasp) { }
    }
}
