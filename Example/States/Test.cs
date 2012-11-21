﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Californium;
using SFML.Graphics;
using SFML.Window;
using Example.Entities;

namespace Example.States
{
    class Test : State
    {
        private Camera camera;
        private Player player;

        public Test()
        {
            camera = new Camera(Game.DefaultView);
            player = new Player(new Vector2f(100, 100));

            Entities.Add(player);

            var random = new Random();
            Map = new TileMap(1000, 1000, "Tiles.png");

            for (int y = 0; y < 1000; y++)
            {
                for (int x = 0; x < 1000; x++)
                {
                    if (y == 0 || x == 0 || x == 999 || y == 999)
                    {
                        Map[x, y] = new Tile(0, false);
                    }
                    else if (x > 20 || y > 20)
                    {
                        if (random.NextDouble() > 0.8)
                        {
                            Map[x, y] = new Tile(1 + random.Next(3), true);
                        }
                    }
                }
            }
        }

        public override void Update(float dt)
        {
            base.Update(dt);

            camera.Position = player.Position;
            camera.Update(dt);
        }

        public override void Draw(RenderTarget rt)
        {
            camera.Draw(rt);

            Map.Draw(rt);
            base.Draw(rt);
        }
    }
}
