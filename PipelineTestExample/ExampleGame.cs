using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PipelineTestExample
{
    public class ExampleGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Tilemap _tilemap;
        private BasicTilemap _basicTilemap;
        private BasicTiledMap _basicTiledMap;
        private BasicTiledTileset _basicTiledTileset;
        private BasicTiledMapTileset _basicTiledMapTileset;
        private BasicTiledLayer _basicTiledLayer;
        private BasicTiledMTL _basicTiledMtl;
        private BasicTiledObjectGroupObject _basicTiledObjectGroupObject;
        private BasicTiledGroup _basicTiledGroup;
        private BasicTiledMTLG _basicTiledMtlg;
        private BasicTileAndTileset _basicTileAndTileset;
        private TSXTileset _tileset64;
        private TSXTileset _tileset4;
        private TSXTilesetDictionary _tsxTilesetDictionary;

        public ExampleGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _tilemap = new Tilemap("map.txt");
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            //_tilemap.LoadContent(Content);
            _basicTilemap = Content.Load<BasicTilemap>("example");
            //_basicTiledMap = Content.Load<BasicTiledMap>("TestMapRev4");
            //_basicTiledTileset = Content.Load<BasicTiledTileset>("TestMapRev4");
            //_basicTiledMapTileset = Content.Load<BasicTiledMapTileset>("TestMapRev4");
            //_basicTiledLayer = Content.Load<BasicTiledLayer>("TestMapRev4");
            //_basicTiledMtl = Content.Load<BasicTiledMTL>("TestMapRev4");
            //_basicTiledObjectGroupObject = Content.Load<BasicTiledObjectGroupObject>("TestMapRev4");
            //_basicTiledGroup = Content.Load<BasicTiledGroup>("TestMapRev4");
            //_basicTiledMtlg = Content.Load<BasicTiledMTLG>("TestMapRev4");
            _basicTileAndTileset = Content.Load<BasicTileAndTileset>("TestMapRev6");
            _tileset64 = Content.Load<TSXTileset>("tileset64");
            _tileset4 = Content.Load<TSXTileset>("tileset4");
            _tsxTilesetDictionary = new TSXTilesetDictionary();
            _tsxTilesetDictionary.Tilesets.Add(_tileset64.Name, _tileset64);
            _tsxTilesetDictionary.Tilesets.Add(_tileset4.Name, _tileset4);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(transformMatrix: Matrix.CreateTranslation(23, 23, 0));
            //_tilemap.Draw(gameTime, _spriteBatch);
            _basicTilemap.Draw(gameTime, _spriteBatch);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
