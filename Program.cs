PrintMainMenu();

while (true)
{
    PrintPrompt();
    var input = Console.ReadLine() ?? "";
    switch (input)
    {
        case "1":
            PrintMainMenu();
            break;
        case "2":
            PrintMainMenu();
            break;
        case "3":
            PrintMainMenu();
            break;
        case "0":
            return;
        default:
            Console.WriteLine("Nepoznata opcija. Pokušajte ponovno.");
            break;
    }
}

static void PrintMainMenu()
{
    const string menu = @"1. Članovi
2. Posudba
3. Statistika
0. Izlaz
";

    Console.Clear();
    Console.WriteLine(menu);
}

static void PrintPrompt()
{
    Console.Write("Odaberite opciju: ");
}
