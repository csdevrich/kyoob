using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Kyoob.Voxel;
using Kyoob.Effects;

namespace Kyoob
{
    /// <summary>
    /// The main Kyoob engine.
    /// </summary>
    public class KyoobEngine : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private GraphicsDevice _device;
        private SpriteBatch _spriteBatch;

        private Camera _camera;
        private TextureEffect _effect;
        private Texture2D _texStone;
        private Texture2D _texDirt;
        private Chunk _chunk;

        /// <summary>
        /// Creates a new Kyoob engine.
        /// </summary>
        public KyoobEngine()
        {
            _graphics = new GraphicsDeviceManager( this );
            Content.RootDirectory = "content";
        }

        /// <summary>
        /// Initializes the Kyoob engine.
        /// </summary>
        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 768;
            _graphics.PreferMultiSampling = true;
            _graphics.ApplyChanges();

            // IsMouseVisible = true;

            base.Initialize();
        }

        /// <summary>
        /// Loads all XNA-compiled content for the Kyoob engine.
        /// </summary>
        protected override void LoadContent()
        {
            _device = GraphicsDevice;
            _spriteBatch = new SpriteBatch( _device );

            _camera = new Camera( _device, Vector3.Zero, 0.0f, 0.0f );

            _effect = new TextureEffect( Content.Load<Effect>( "fx/texture" ) );

            _texStone = Content.Load<Texture2D>( "tex/stone" );
            _texDirt = Content.Load<Texture2D>( "tex/dirt" );

            _chunk = new Chunk( _device, Vector3.Zero );
        }

        /// <summary>
        /// Unloads all non-XNA managed Kyoob content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Updates the Kyoob engine.
        /// </summary>
        /// <param name="gameTime">Frame time information.</param>
        protected override void Update( GameTime gameTime )
        {
            if ( Keyboard.GetState().IsKeyDown( Keys.Escape ) )
                this.Exit();

            _camera.Update( gameTime );

            base.Update( gameTime );
        }

        /// <summary>
        /// Renders the Kyoob engine.
        /// </summary>
        /// <param name="gameTime">Frame time information.</param>
        protected override void Draw( GameTime gameTime )
        {
            Stopwatch watch = new Stopwatch();
            _device.Clear( Color.CornflowerBlue );

            _effect.Projection = _camera.Projection;
            _effect.View = _camera.View;
            _effect.Texture = _texDirt;

            
            watch.Start();
            _chunk.Draw( _device, _effect, _camera );
            watch.Stop();
            Console.WriteLine( "Chunk draw time: {0:0.00}ms", watch.Elapsed.TotalMilliseconds );


            base.Draw( gameTime );
        }
    }
}