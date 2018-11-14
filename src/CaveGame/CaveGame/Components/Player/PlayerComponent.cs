﻿using CaveGame.Cavegen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Tiled;
using System;
using Glint;
using System.Collections.Generic;

namespace CaveGame.Components {
    public class PlayerComponent : RenderableComponent, IUpdatable {
        public override RectangleF bounds => new RectangleF(0, 0, Constants.BUFFER_ZONE + Constants.CAVE_WIDTH, Constants.CAVE_HEIGHT);

        public bool grounded = false;

        TiledMapMover mover;
        BoxCollider collider;
        Vector2 velocity;
        List<CollisionResult> collisions = new List<CollisionResult>();

        public override void onAddedToEntity() {
            base.onAddedToEntity();
            mover = this.getComponent<TiledMapMover>();
            collider = entity.getComponent<BoxCollider>();

            grounded = false;
        }

        void IUpdatable.update() {
            if (Input.isKeyDown(Keys.Left) && !Input.isKeyDown(Keys.Right)) {
                velocity.X = -(float) Constants.PLAYER_SPEED;
            }
            if (Input.isKeyDown(Keys.Right) && !Input.isKeyDown(Keys.Left)) {
                velocity.X = (float) Constants.PLAYER_SPEED;
            }
            if (!Input.isKeyDown(Keys.Left) && !Input.isKeyDown(Keys.Right)) {
                velocity.X = 0;
            }
            if (Input.isKeyDown(Keys.Up) && grounded) {
                Console.WriteLine("jump");
                velocity.Y = -(float) Math.Sqrt(2 * Constants.GRAVITY * Constants.PLAYER_JUMP_HEIGHT);
            }

            grounded = false;
            velocity.Y += (float) Constants.GRAVITY * Time.deltaTime;

            var motion = velocity * Time.deltaTime;

            collisions.Clear();
            if (collider.collidesWithAnyMultiple(motion, collisions)) {
                for (int i = 0; i < collisions.Count; i++) {
                    motion -= collisions[i].minimumTranslationVector;
                    Console.WriteLine(collisions[i].normal);

                    if (Math.Abs(collisions[i].normal.Y) == 1) {
                        velocity.Y = 0;
                        grounded = true;
                    }
                }
            }

            entity.position += motion;
        }

        public override void render(Graphics graphics, Camera camera) {
            var g = graphics.batcher;
            g.drawRect(entity.position.X,
                       entity.position.Y,
                       Constants.PLAYER_WIDTH,
                       Constants.PLAYER_HEIGHT, 
                       Constants.PLAYER_COLOR);
        }
    }
}