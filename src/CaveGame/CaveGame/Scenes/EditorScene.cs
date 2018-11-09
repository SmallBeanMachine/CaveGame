﻿using CaveGame.Cavegen;
using CaveGame.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Tiled;
using Glint;

namespace CaveGame.Scenes {
    public class EditorScene : Scene {
        public Nez.VirtualButton leftClick;
        public Nez.VirtualButton rightClick;
        public Nez.VirtualButton play;
        public Nez.VirtualButton edit;

        public CaveEditor caveEditor;

        public Entity mapEntity;
        public Entity playerEntity;

        enum Mode : int {
            editting,
            playing
        }

        public int mode = (int) Mode.editting;

        public override void initialize() {
            base.initialize();

            // setup
            addRenderer(new DefaultRenderer());
            clearColor = Color.White;

            // add cave view component
            var caveViewEntity = createEntity("cave_view");
            caveEditor = caveViewEntity.addComponent(new CaveEditor());
            caveEditor.generate();

            leftClick = new Nez.VirtualButton();
            leftClick.nodes.Add(new Nez.VirtualButton.MouseLeftButton());

            rightClick = new Nez.VirtualButton();
            rightClick.nodes.Add(new Nez.VirtualButton.MouseRightButton());

            play = new Nez.VirtualButton();
            play.nodes.Add(new Nez.VirtualButton.KeyboardKey(Microsoft.Xna.Framework.Input.Keys.P));

            edit = new Nez.VirtualButton();
            edit.nodes.Add(new Nez.VirtualButton.KeyboardKey(Microsoft.Xna.Framework.Input.Keys.E));

            mapEntity = this.createEntity("map_tiles");
            playerEntity = this.createEntity("player");
        }

        public override void update() {
            base.update();

            Vector2 mouseLocation = Input.scaledMousePosition;
            
            if (leftClick.isDown) {
                caveEditor.setBlock(mouseLocation.X, mouseLocation.Y);
            }

            if (rightClick.isDown) {
                caveEditor.selectBlock(mouseLocation.X, mouseLocation.Y);
            }

            if (play.isDown && mode == (int) Mode.editting) {
                TiledMap map = new TiledMap(0, Constants.LEVEL_ROWS, Constants.LEVEL_COLUMNS, Constants.TILE_WIDTH, Constants.TILE_HEIGHT);
                Texture2D tilesetTexture = GlintCore.contentSource.Load<Texture2D>("spritesheet");
                TiledTileset tileset = map.createTileset(tilesetTexture, 0, Constants.TILE_WIDTH, Constants.TILE_HEIGHT, true, 0, 0, 4, 4);
                map.createTileLayer("walls", map.width, map.height, caveEditor.level.bake(tileset));
                map.createTileLayer("lighting", map.width, map.height, caveEditor.level.bake(tileset));
                TiledTile[] tiles = caveEditor.level.bake(tileset);
                mapEntity.setPosition(Constants.BUFFER_ZONE, Constants.BUFFER_ZONE);
                mapEntity.addComponent(new LevelComponent(map, "walls"));

                playerEntity.addComponent(new PlayerComponent());
                mode = (int) Mode.playing;
            }

            if (edit.isDown && mode == (int) Mode.playing) {
                mapEntity.removeComponent<LevelComponent>();
                mode = (int) Mode.editting;
            }
        }
    }
}