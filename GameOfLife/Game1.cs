using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameOfLife
{
    public class Game1 : Game
    {
        public static Game1 Instance;
        
        private readonly Board _board = new Board();
        private readonly UserInterface _interface = new UserInterface();
        
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private MouseState _oldMouseState = new MouseState();
        private KeyboardState _oldKeyboardState = new KeyboardState();

        public bool IsSimulating;

        public Game1()
        {
            Instance = this;

            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1600,
                PreferredBackBufferHeight = 900
            };
            
            _graphics.ApplyChanges();
            
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;

            base.Initialize();
            
            _interface.Initialize(_graphics);

            // Create a board filling the whole gamewindow
            var width = (double)_graphics.PreferredBackBufferWidth;
            var height = (double)_graphics.PreferredBackBufferHeight - UserInterface.Height;
            var cols = (int)Math.Floor(width / Board.GridSize);
            var rows = (int)Math.Floor(height / Board.GridSize);
            
            _board.Initialize(cols, rows);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _interface.LoadContent(Content);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();
            var keyboardState = Keyboard.GetState();
            
            base.Update(gameTime);

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back ==
                ButtonState.Pressed || Keyboard.GetState().IsKeyDown(
                    Keys.Escape))
                Exit();
            
            if (!_board.EnableHistory || (_board.EnableHistory && !_board.InPast))
            {
                // Space enables the simulation
                if (keyboardState.IsKeyUp(Keys.Space) && _oldKeyboardState.IsKeyDown(Keys.Space))
                    IsSimulating = !IsSimulating;

                // Enter runs a single generation
                if (keyboardState.IsKeyUp(Keys.Enter) && _oldKeyboardState.IsKeyDown(Keys.Enter))
                    _board.UpdateStep();
            }

            // End resets the board
            if (keyboardState.IsKeyUp(Keys.End) && _oldKeyboardState.IsKeyDown(Keys.End))
                _board.Reset();

            if (_board.EnableHistory)
            {
                // Left Arrow goes back in time
                if (keyboardState.IsKeyUp(Keys.Left) && _oldKeyboardState.IsKeyDown(Keys.Left))
                    _board.Undo();
                
                // Right Arrow steps forward in time
                if (keyboardState.IsKeyUp(Keys.Right) && _oldKeyboardState.IsKeyDown(Keys.Right))
                    _board.Redo();
            }
            
            _board.Update();
            _interface.Update();

            _oldMouseState = mouseState;
            _oldKeyboardState = keyboardState;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            _board.Draw(_spriteBatch);
            _interface.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}