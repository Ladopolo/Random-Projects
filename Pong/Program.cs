using Raylib_cs;

class Program
{
    static void Main()
    {
        Paddles paddles = new();
        Ball ball = new();
        Scoreboard scoreboard = new();

        GameState currentState = GameState.MainMenu;

        Raylib.InitWindow(600, 600, "Pong!");
        Raylib.SetTargetFPS(60);
        Raylib.HideCursor();

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            switch (currentState)
            {
                case GameState.MainMenu:
                    Raylib.DrawText("Press Space to Start", 150, 250, 30, Color.White);
                    if (Raylib.IsKeyPressed(KeyboardKey.Space))
                        currentState = GameState.Playing;
                    break;

                case GameState.Playing:
                    paddles.Controls();

                    string? scorer = ball.Update(
                        paddles.rectX, paddles.RectY, paddles.width, paddles.height,
                        paddles.enemyX, paddles.enemyY, paddles.enemyWidth, paddles.enemyHeight
                    );

                    if (scorer != null)
                    {
                        scoreboard.AddPoint(scorer);
                        ball.ResetBall();
                    }

                    paddles.EnemyAI(ball.ballX, ball.ballY, ball.velX, ball.velY);
                    paddles.DrawRect();
                    ball.DrawBall();
                    scoreboard.Draw();

                    if (scoreboard.playerScore == 5 || scoreboard.EnemyScore == 5)
                    {
                        currentState = GameState.GameOver;
                    }
                    break;

                case GameState.GameOver:
                    string winner = scoreboard.playerScore > scoreboard.EnemyScore ? "Player Wins!" : "Enemy Wins!";
                    Raylib.DrawText(winner, 200, 200, 30, Color.White);
                    Raylib.DrawText("Press [R] to restart", 160, 250, 30, Color.White);

                    if (Raylib.IsKeyPressed(KeyboardKey.R))
                    {
                        scoreboard.playerScore = 0;
                        scoreboard.EnemyScore = 0;
                        ball.ResetBall();
                        currentState = GameState.MainMenu;
                    }
                    break;
            }

            Raylib.EndDrawing();
        }
    }
}

enum GameState
{
    MainMenu,
    Playing,
    GameOver
}

class Paddles
{
    public int rectX = 200;
    public int RectY = 525;
    public int width = 100;
    public int height = 25;
    // Enemy var
    public int enemyX = 200;
    public int enemyY = 50;
    public int enemyWidth = 100;
    public int enemyHeight = 25;

    int speed = 7;
    int enemySpeed = 7;
    float enemyAcceleration = 0.5f;

    float enemyCurrentSpeed = 0f;

    public void DrawRect()
    {
        Raylib.DrawRectangle(rectX, RectY, width, height, Color.Blue); // Player Paddle
        Raylib.DrawRectangle(enemyX, enemyY, enemyWidth, enemyHeight, Color.Red);            // Enemy Paddle
    }

    public void Controls()
    {
        if (Raylib.IsKeyDown(KeyboardKey.Right)) rectX += speed;
        if (Raylib.IsKeyDown(KeyboardKey.Left)) rectX -= speed;

        rectX = Math.Clamp(rectX, 0, 600 - width);
    }

    public void EnemyAI(int ballX, int ballY, int ballVelX, int ballVelY)
    {
        if (ballVelY < 0)
        {
            float framesToReach = (enemyY + enemyHeight - ballY) / (float)ballVelY;
            int predictedX = ballX + (int)(ballVelX * framesToReach);

            int targetX = predictedX - enemyWidth / 2;

            targetX = Math.Clamp(targetX, 0, 600 - enemyWidth);

            if (enemyX < targetX)
            {
                enemyCurrentSpeed = Math.Min(enemyCurrentSpeed + enemyAcceleration, enemySpeed);
            }
            else if (enemyX > targetX)
            {
                enemyCurrentSpeed = Math.Max(enemyCurrentSpeed - enemyAcceleration, -enemySpeed);
            }

            enemyX += (int)enemyCurrentSpeed;
            enemyX = Math.Clamp(enemyX, 0, 600 - enemyWidth);
        }
        else
        {
            enemyCurrentSpeed = 0;
        }
    }
}

class Ball
{
    int speed = 4;
    int hitCount = 0;
    int maxSpeed = 8;
    public int velX = 5;
    public int velY = -5;
    public int ballX = 300;
    public int ballY = 300;

    public void DrawBall()
    {
        Raylib.DrawCircle(ballX, ballY, 30, Color.White);
    }

    void IncreaseSpeed()
    {
        if (hitCount % 5 == 0)
        {
            if (velX > 0) velX = Math.Min(velX + 1, maxSpeed);
            else velX = Math.Max(velX - 1, -maxSpeed);
        }

        if (velY > 0) velY = Math.Min(velY + 1, maxSpeed);
        else velY = Math.Max(velY - 1, -maxSpeed);
    }

    public string? Update(int playerX, int playerY, int width, int height, int enemyX, int enemyY, int enemyWidth, int enemyHeight)
    {

        ballX += velX;
        ballY += velY;

        if (ballX <= 30 || ballX >= 570)
        {
            velX = -velX;
        }

        if (ballY >= 600)
        {
            return "enemy";
        }

        if (ballY <= 0)
        {
            return "player";
        }

        // Player Paddle
        if (
            ballX >= playerX &&
            ballX <= playerX + width &&
            ballY + 30 >= playerY &&
            ballY - 30 <= playerY + height
        )
        {
            velY = -velY;
            ballY = playerY - 30;

            hitCount++;
            IncreaseSpeed();
        }

        // Enemy Paddle
        if (
            ballX + 30 >= enemyX &&
            ballX - 30 <= enemyX + enemyWidth &&
            ballY - 30 <= enemyY + enemyHeight &&
            ballY + 30 >= enemyY
        )
        {
            velY = -velY;
            ballY = enemyY + enemyHeight + 30;

            hitCount++;
            IncreaseSpeed();
        }

        return null;
    }

    public void ResetBall()
    {
        ballX = 300;
        ballY = 300;
        velX = 5;
        velY = -5;
        hitCount = 0;
    }
}

class Scoreboard
{
    public int playerScore = 0;
    public int EnemyScore = 0;

    public bool isOver = false;

    public void Draw()
    {
        Raylib.DrawText($"Player: {playerScore}", 10, 10, 20, Color.White);
        Raylib.DrawText($"Enemy: {EnemyScore}", 10, 40, 20, Color.White);
    }

    public void AddPoint(string scorer)
    {
        if (scorer == "player") playerScore++;
        else if (scorer == "enemy") EnemyScore++;
    }
}