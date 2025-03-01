using Raylib_cs;

using System.Numerics;

class Program
{
    static List<Particle> particles = new();
    static Random random = new Random();

    static void Main()
    {
        const int screenWidth = 800;
        const int screenHeight = 600;

        Raylib.InitWindow(screenWidth, screenHeight, "Particle System");
        Raylib.SetTargetFPS(60);

        while (!Raylib.WindowShouldClose())
        {
            Update();
            Draw();
        }

        Raylib.CloseWindow();
    }

    static void Update()
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

    static void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.Black);

        // Draw all particles
        foreach (var particle in particles)
        {
            particle.Draw();
        }

        // Draw UI
        Raylib.DrawText($"Particles: {particles.Count}", 10, 10, 20, Color.White);
        Raylib.DrawText("Click and hold to spawn particles", 10, 30, 20, Color.White);

        Raylib.EndDrawing();
    }
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
            (byte)255,                                    // R
            (byte)(Random.Shared.NextSingle() * 255),     // G
            (byte)0,                                      // B
            (byte)255                                     // A
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