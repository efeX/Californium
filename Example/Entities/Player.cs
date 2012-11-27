﻿using System;
using Californium;
using SFML.Graphics;
using SFML.Window;

namespace Example.Entities
{
    class Player : Entity
    {
        private const int SpriteSize = 8;

        private Sprite sprite;

        private float hSave, vSave;
        private bool keyW, keyA, keyS, keyD;

        private float hSpeed, vSpeed;

        public Player(Vector2f position)
        {
            Solid = true;
            Position = position;
            Origin = new Vector2f(SpriteSize / 2, SpriteSize / 2);
            Size = new Vector2f(SpriteSize, SpriteSize);

            sprite = new Sprite(TextureManager.Load("Player.png")) { Origin = Origin };

            Input.Key[Keyboard.Key.W] = args => keyW = args.Pressed;
            Input.Key[Keyboard.Key.A] = args => keyA = args.Pressed;
            Input.Key[Keyboard.Key.S] = args => keyS = args.Pressed;
            Input.Key[Keyboard.Key.D] = args => keyD = args.Pressed;
        }

        public override void Update(float dt)
        {
            const float maxHSpeed = 100;
            const float maxVSpeed = 150;
            const float speed = 75;
            const float jumpSpeed = 7;
            const float gravity = 25;
            const float friction = 20;

            if (keyA) hSpeed -= speed * dt;
            if (keyD) hSpeed += speed * dt;

            hSpeed *= 1 - (friction * dt);
            hSpeed = Utility.Clamp(hSpeed, -maxHSpeed, maxHSpeed);

            var bounds = BoundingBox;
            bounds.Top++;

            if (keyW && !Parent.PlaceFree(bounds)) vSpeed -= jumpSpeed;

            vSpeed += gravity * dt;
            vSpeed = Utility.Clamp(vSpeed, -maxVSpeed, maxVSpeed);

            int hRep = (int)Math.Floor(Math.Abs(hSpeed));
            int vRep = (int)Math.Floor(Math.Abs(vSpeed));

            hSave += (float)(Math.Abs(hSpeed) - Math.Floor(Math.Abs(hSpeed)));
            vSave += (float)(Math.Abs(vSpeed) - Math.Floor(Math.Abs(vSpeed)));

            while (hSave >= 1)
            {
                --hSave;
                ++hRep;
            }

            while (vSave >= 1)
            {
                --vSave;
                ++vRep;
            }

            var testRect = BoundingBox;
            while (hRep-- > 0)
            {
                testRect.Left += Math.Sign(hSpeed);
                if (!Parent.PlaceFree(testRect))
                {
                    hSave = 0;
                    hSpeed = 0;
                    break;
                }

                Position.X += Math.Sign(hSpeed);
            }

            testRect = BoundingBox;
            while (vRep-- > 0)
            {
                testRect.Top += Math.Sign(vSpeed);
                if (!Parent.PlaceFree(testRect))
                {
                    vSave = 0;
                    vSpeed = 0;
                    break;
                }

                Position.Y += Math.Sign(vSpeed);
            }
        }

        public override void Draw(RenderTarget rt)
        {
            sprite.Position = Position;
            rt.Draw(sprite);
        }

        private static float Direction(bool neg, bool pos)
        {
            float res = 0;
            if (neg) res -= 1;
            if (pos) res += 1;
            return res;
        }
    }
}
