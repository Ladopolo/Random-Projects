using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using Raylib_cs;

class GameSettings
{
    public const int screenWidth = 600;
    public const int screenHeight = 600;
    public const string name = "Flappy";
}

class Program
{

    static void Main()
    {
        Flappy flappy = new();
        Scoring scoring = new();
        scoring.highScore = Scoring.LoadHighScore();

        List<Pipes> pipes = new List<Pipes>();
        pipes.Add(new Pipes());

        GameState currenState = GameState.MainMenu;

        Raylib.InitWindow(GameSettings.screenWidth, GameSettings.screenHeight, GameSettings.name);
        Raylib.SetTargetFPS(60);
        Raylib.HideCursor();

        float timer = 0f;
        int spawnInterval = 2;

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.SkyBlue);

            switch (currenState)
            {
                case GameState.MainMenu:
                    MainMenu();
                    if (Raylib.IsKeyPressed(KeyboardKey.Space)) currenState = GameState.Flapping;
                    break;

                case GameState.Flapping:
                    float deltaTime = Raylib.GetFrameTime();

                    timer += deltaTime;

                    if (timer >= spawnInterval)
                    {
                        timer = 0;
                        pipes.Add(new Pipes());
                    }

                    foreach (Pipes p in pipes)
                    {
                        p.Update();
                        p.Draw();

                        if (p.posX + p.width < flappy.posX && p.scored == false)
                        {
                            scoring.score++;
                            p.scored = true;
                        }

                        if (flappy.Collision(p)) { currenState = GameState.GameOver; }
                    }

                    for (int i = pipes.Count - 1; i >= 0; i--)
                    {
                        if (pipes[i].posX + pipes[i].width <= 0)
                        {
                            pipes.RemoveAt(i);
                        }
                    }

                    flappy.Update();
                    flappy.Draw();
                    break;

                case GameState.GameOver:
                    if (scoring.score > scoring.highScore)
                    {
                        scoring.highScore = scoring.score;
                        Scoring.SaveHighScore(scoring.highScore);
                    }
                    scoring.Draw();
                    GameOverScreen();
                    if (Raylib.IsKeyPressed(KeyboardKey.R))
                    {
                        ResetFlappy(flappy, pipes, ref timer, scoring);
                        currenState = GameState.MainMenu;
                    }
                    break;
            }

            Raylib.EndDrawing();
        }
    }

    static void MainMenu()
    {
        Raylib.DrawCircle(GameSettings.screenWidth / 2, GameSettings.screenHeight / 2 - 50, 30, Color.Yellow); // Flappy
        Raylib.DrawRectangle(500, 0, 100, 250, Color.Green); // Top Pipe
        Raylib.DrawRectangle(500, 425, 100, 175, Color.Green);
        Raylib.DrawText("Flappy!", 250, 150, 30, Color.RayWhite);
        Raylib.DrawText("Press Space to Start!", 150, 300, 30, Color.RayWhite);
    }

    static void GameOverScreen()
    {
        Raylib.DrawText("Game Over!", 250, 150, 30, Color.RayWhite);
        Raylib.DrawText("Press [R] to Restart", 150, 300, 30, Color.RayWhite);
    }

    static void ResetFlappy(Flappy flappy, List<Pipes> pipes, ref float timer, Scoring scoring)
    {
        flappy.posY = GameSettings.screenHeight / 2;
        flappy.velocityY = 0;
        flappy.isDead = false;
        pipes.Clear();
        timer = 0f;
        pipes.Add(new Pipes());
        scoring.score = 0;
    }
}

enum GameState
{
    MainMenu,
    Flapping,
    GameOver
}

class Flappy
{
    public float velocityY = 0;
    public float gravity = 0.5f;
    public float jumpForce = -8f;

    public int posX = GameSettings.screenWidth / 4;
    public float posY = GameSettings.screenHeight / 2;
    public int radius = 30;

    public bool isDead = false;

    public void Draw()
    {
        Raylib.DrawCircle(posX, (int)posY, radius, Color.Yellow);
    }

    public void Controls()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Space))
        {
            velocityY = jumpForce;
        }
    }

    public void Update()
    {
        Controls();

        velocityY += gravity;
        posY += velocityY;
    }

    public bool Collision(Pipes pipe)
    {

        if (posX + radius > pipe.posX && posX - radius < pipe.posX + pipe.width)
        {
            if (posY - radius < pipe.gapY || posY + radius > pipe.gapY + pipe.gapSize) isDead = true;
        }

        if (posY - radius <= 0 || posY + radius >= GameSettings.screenHeight) isDead = true;

        return isDead;
    }
}

class Pipes
{
    Random rand = new();

    public int width = 100;
    public int posX = 500;
    public int posY = 0;
    public int gapY;
    public int gapSize = 175;
    public int movement = 3;

    public bool scored = false;

    public Pipes()
    {
        gapY = rand.Next(100, 400);
    }

    public void Draw()
    {
        Raylib.DrawRectangle(posX, posY, width, gapY, Color.Green);

        // Bottom Pipe
        int bottomPipeY = gapY + gapSize;
        int bottomPipeHeight = GameSettings.screenHeight - bottomPipeY;
        Raylib.DrawRectangle(posX, bottomPipeY, width, bottomPipeHeight, Color.Green);
    }

    public void Update()
    {
        posX -= movement;
    }
}

class Scoring
{
    public int score = 0;
    public int highScore = 0;

    static string GetHighScoreFilePath()
    {
        string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        return Path.Combine(homeDir, "highscore.txt");
    }

    public static void SaveHighScore(int highScore)
    {
        File.WriteAllText(GetHighScoreFilePath(), highScore.ToString());
    }

    public static int LoadHighScore()
    {
        string path = GetHighScoreFilePath();
        if (File.Exists(path))
        {
            string text = File.ReadAllText(path);
            if (int.TryParse(text, out int savedHighScore))
                return savedHighScore;
        }
        return 0;
    }

    public void Draw()
    {
        Raylib.DrawText($"Score: {score}", 150, 350, 30, Color.RayWhite);

        if (score >= highScore) highScore = score;

        Raylib.DrawText($"High Score: {highScore}", 150, 400, 30, Color.RayWhite);
    }
}