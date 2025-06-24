class Program
{
    static void Main()
    {
        string? guess;
        int randomNumber;
        int guessedNumero;
        bool gameRunning = true;

        while (gameRunning)
        {
            Random random = new();
            randomNumber = random.Next(1, 11);
            Console.WriteLine("I am thinking of a number between 1 and 10");
            guess = Console.ReadLine();
            guessedNumero = int.Parse(guess!);

            if (guessedNumero == randomNumber)
            {
                Console.WriteLine("Correct!");
            }
            else
            {
                Console.WriteLine("Wrong!");
            }
        }
    }
}