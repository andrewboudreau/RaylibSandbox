using Raylib_cs;

using System;
using System.Collections.Generic;
using System.Numerics;

namespace WebParticleSystem
{
    public partial class Program
    {
        private static Texture2D logo;
        private static readonly List<Particle> particles = [];

        /// <summary>
        /// Application entry point
        /// </summary>
        public static void Main(string[] args)
        {
            Console.WriteLine("OMG!");
            // Main entry point - this matches the RaylibWasm structure
            const int screenWidth = 800;
            const int screenHeight = 600;

            // Initialize Raylib window - this works in WebAssembly as well
            Console.WriteLine("Window about to Init");
            Raylib.InitWindow(screenWidth, screenHeight, "Particle System");
            Console.WriteLine("Window Init'ed");
            Console.WriteLine("About to set target fps");
            Raylib.SetTargetFPS(60);
            Console.WriteLine("Target fps set");

            bool shouldClose = false;
            try
            {
                while (!shouldClose)
                {
                    Console.WriteLine("Frame Start");
                    Update();
                    Draw();
                    Console.WriteLine("Frame End");
                    Console.WriteLine("About check WindowShouldClose");
                    //shouldClose = Raylib.WindowShouldClose();
                    Console.WriteLine("WindowShouldClose: " + shouldClose); // This is always false, so the loop never breaks
                }

                Console.WriteLine("Window about to close");
                Raylib.CloseWindow();
                Console.WriteLine("Window closed");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void Update()
        {
            // Spawn new particles on mouse click
            if (Raylib.IsMouseButtonDown(MouseButton.Left))
            {
                var mousePos = Raylib.GetMousePosition();
                for (int i = 0; i < 5; i++) // Spawn 5 particles per frame while clicked
                {
                    particles.Add(new Particle(mousePos));
                }
            }

            // Update all particles
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                particles[i].Update();
                if (particles[i].IsDead())
                {
                    particles.RemoveAt(i);
                }
            }
        }

        public static void Draw()
        {
            Raylib.BeginDrawing();

            Raylib.ClearBackground(Color.White);

            Raylib.DrawFPS(4, 4);
            Raylib.DrawText("All systems operational!", 4, 32, 20, Color.Maroon);

            Raylib.DrawTexture(logo, 4, 64, Color.White);

            Raylib.EndDrawing();
        }

        class Particle
        {
            public Vector2 Position;
            public Vector2 Velocity;
            public float Life;
            public float InitialSize;
            public Color Color;

            public Particle(Vector2 pos)
            {
                Position = pos;

                // Random velocity in a circular pattern
                float angle = Random.Shared.NextSingle() * MathF.PI * 2;
                float speed = Random.Shared.NextSingle() * 2 + 1;
                Velocity = new Vector2(
                    MathF.Cos(angle) * speed,
                    MathF.Sin(angle) * speed
                );

                Life = 1.0f;
                InitialSize = Random.Shared.NextSingle() * 4 + 2; // Random size between 2 and 6

                // Random color between yellow and red
                Color = new Color(
                    (byte)255,                                     // R
                    (byte)(Random.Shared.NextSingle() * 255),   // G
                    (byte)0,                                       // B
                    (byte)255                                      // A
                );
            }

            public void Update()
            {
                // Apply physics
                Velocity.Y += 0.05f; // gravity
                Position += Velocity;

                // Add some horizontal wind effect
                Velocity.X += (Random.Shared.NextSingle() - 0.5f) * 0.1f;

                // Slow down over time (air resistance)
                Velocity *= 0.99f;

                // Decrease life
                Life -= 0.005f;

                // Update color alpha based on life
                Color.A = (byte)(Life * 255);

                if (Position.Y > 580 && Velocity.Y > 0)
                {
                    Velocity.Y *= -0.6f; // Bounce with energy loss
                    Position.Y = 580;
                }
            }

            public void Draw()
            {
                float size = InitialSize * Life; // Particles shrink as they die
                Raylib.DrawCircleV(Position, size, Color);

                // Optional: Add a glowing effect
                Color glow = Color;
                glow.A = (byte)(Life * 127);
                Raylib.DrawCircleV(Position, size * 2, glow);
            }

            public bool IsDead()
            {
                return Life <= 0 || Position.Y > 600; // Die if life runs out or falls below screen
            }
        }
    }
}