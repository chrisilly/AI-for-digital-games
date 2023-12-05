using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AI_for_digital_games
{
    public interface IBehaviourSystem
    {
        void Update(Agent body);
        void Decide(Agent body);
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public static Random random { get; private set; } = new Random();
        public static int ScreenWidth { get; private set; }
        public static int ScreenHeight { get; private set; }
        public static Vector2 ScreenCenter { get { return new Vector2(ScreenWidth / 2, ScreenHeight / 2); } }

        Agent subject;
        AgentHandler agentHandler;
        IBehaviourSystem brain;
        public static Texture2D texture;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            ScreenWidth = GraphicsDevice.Viewport.Width;
            ScreenHeight = GraphicsDevice.Viewport.Height;

            agentHandler = new AgentHandler();
            brain = new BrainDead();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            texture = Content.Load<Texture2D>("tile64");

            subject = new Agent(brain, Color.White, 0.5f);
            agentHandler.AddAgent(subject);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            agentHandler.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            agentHandler.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public static Vector2 GetRandomPosition()
        {
            int x = random.Next(0, ScreenWidth-64);
            int y = random.Next(0, ScreenHeight-64);
            return new Vector2(x, y);
        }

        public static Vector2 GetRandomDirection()
        {
            int x = random.Next(-1, 2);
            int y = random.Next(-1, 2);
            return new Vector2(x, y);
        }
    }
}