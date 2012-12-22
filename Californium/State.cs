﻿using System;
using SFML.Graphics;

namespace Californium
{
    public abstract class State
    {
        [Flags]
        public enum UpdateMode
        {
            None = 0,
            Input = 1,
            Update = 2,
            Draw = 4,
            Background = Update | Draw,
            All = Input | Update | Draw
        }

        public Camera Camera;
        public EntityManager Entities;
        public TileMap Map;

        public UpdateMode InactiveMode { get; protected set; }
        public Color ClearColor { get; protected set; }

        private Input input;
        public Input Input
        {
            get { return input ?? (input = new Input()); }
        }

        protected State()
        {
            InitializeCamera();
            Entities = new EntityManager(this);
            InactiveMode = UpdateMode.All;
        }

        public bool PlaceFree(FloatRect r)
        {
            return Map.PlaceFree(r) && Entities.PlaceFree(r);
        }

        internal void UpdateInternal()
        {
            Entities.Update();
            Update();
            Camera.Update();
        }

        public virtual void Update()
        {
            
        }

        public virtual void Draw(RenderTarget rt)
        {
            Camera.Apply(rt);

            if (Map != null)
                Map.Draw(rt);

            Entities.Draw(rt);
        }

        public virtual bool ProcessEvent(InputArgs args)
        {
            if (input != null && input.ProcessInput(args))
                return true;
            return Entities.ProcessInput(args);
        }

        public virtual void InitializeCamera()
        {
            Camera = new Camera(Game.DefaultView);
        }
    }
}
