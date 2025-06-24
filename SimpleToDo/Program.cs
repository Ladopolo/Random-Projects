using Microsoft.VisualBasic;

class Program
{
    static void Main()
    {
        List<string> toDo = new();
        string? task;
        string? taskToRemove;
        bool isRunning = true;

        while (isRunning)
        {
            Console.WriteLine("Choose an option");
            Console.WriteLine("1. Add a task");
            Console.WriteLine("2. List tasks");
            Console.WriteLine("3. Remove task");
            Console.WriteLine("4. Quit To-Do List");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.WriteLine("Add a task");
                    task = Console.ReadLine()!;
                    toDo.Add(task);
                    break;
                case "2":
                    foreach (string i in toDo)
                    {
                        Console.WriteLine(i);
                    }
                    break;
                case "3":
                    Console.WriteLine("Remove a task");
                    taskToRemove = Console.ReadLine()!;
                    toDo.Remove(taskToRemove);
                    break;
                case "4":
                    isRunning = false;
                    break;
            }
        }
    }
}