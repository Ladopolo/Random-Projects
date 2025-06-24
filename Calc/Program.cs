class Program
{
    static int firstNumber;
    static int secondNumber;
    static int output;
    static string? firstInput;
    static string? secondInput;
    static string? operand;
    static bool success = false;
    static bool isInProgress = true;
    static void Main()
    {
        while (isInProgress)
        {
            Console.WriteLine("Enter first number");
            firstInput = Console.ReadLine()!;
            firstNumber = int.Parse(firstInput);
            Console.WriteLine("Enter second number");
            secondInput = Console.ReadLine()!;
            secondNumber = int.Parse(secondInput);
            Console.WriteLine("Enter operand");
            operand = Console.ReadLine()!;

            if (operand == "+")
            {
                Addition();
            }
            else if (operand == "-")
            {
                Subtraction();
            }
            else if (operand == "*")
            {
                Multiplication();
            }
            else if (operand == "/")
            {
                Division();
            }
            if (success)
            {
                Console.WriteLine("Answer: " + output);
            }
            else if (!success)
            {
                Console.WriteLine("Error!");
            }

            /* This is for asking if they wish to continue; redundant */
            // Console.WriteLine("Do you wish to continue?");
            // string? response = Console.ReadLine();

            // if (response != "y") break;
        }
    }

    static void Addition()
    {
        success = true;
        output = firstNumber + secondNumber;
    }

    static void Subtraction()
    {
        success = true;
        output = firstNumber - secondNumber;
    }

    static void Multiplication()
    {
        success = true;
        output = firstNumber * secondNumber;
    }

    static void Division()
    {
        if (secondNumber == 0)
        {
            success = false;
            Console.WriteLine("Can't divide by zero loser");
        }
        else
        {
            success = true;
            output = firstNumber / secondNumber;
        }
    }
}