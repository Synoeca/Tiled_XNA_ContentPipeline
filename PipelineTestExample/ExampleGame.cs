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
            _basicTiledLayer = Content.Load<BasicTiledLayer>("TestMapRev4");
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
