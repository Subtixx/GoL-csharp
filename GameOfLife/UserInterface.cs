using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace GameOfLife
{
    /// <summary>
    /// UI Class
    /// </summary>
    public class UserInterface
    {
        private int _windowWidth;
        private int _windowHeight;
        private const int Border = 5;

        public const float Height = 150.0f;
        private float _width;
        private Vector2 _position;

        private SpriteFont _font;

        public void Initialize(GraphicsDeviceManager graphics)
        {
            _windowHeight = graphics.PreferredBackBufferHeight;
            _windowWidth = graphics.PreferredBackBufferWidth;
            _width = _windowWidth - 2; // 2 Pixel "border"

            _position = new Vector2(1, _windowHeight - Height);
        }

        public void LoadContent(ContentManager content)
        {
            _font = content.Load<SpriteFont>("PixelLove");
        }

        public void Update()
        {
            // TODO: Drawing on board
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.FillRectangle(_position, new Size2(_width, Height),
                new Color(0, 0, 0, 125));

            var simulateString = (Game1.Instance.IsSimulating ? "Simulating" : "Paused");
            spriteBatch.DrawString(_font, simulateString,
                new Vector2(_width / 2.0f, GetRelativeY(10)),
                Color.White);
            
            spriteBatch.DrawString(_font, "Generation " + Board.Instance.Generation,
                GetRelative(10, 10),
                Color.White);
        }

        private Vector2 GetRelative(int x, int y)
        {
            return new Vector2(_position.X + Border + x, _position.Y + Border + y);
        }

        private float GetRelativeY(int y)
        {
            return _position.Y + Border + y;
        }

        private Vector2 GetRelative(Vector2 pos)
        {
            return _position + pos + new Vector2(Border, Border);
        }
    }
}