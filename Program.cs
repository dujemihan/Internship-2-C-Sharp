var members = new Dictionary<int, (
    string name,
    string surname,
    DateOnly dob,
    Dictionary<int, (string title, string author, DateOnly date)> borrows)>
{
};

while (true)
{
    bool reset = false;

    PrintMainMenu();
    while (!reset)
    {
        PrintPrompt();
        var input = Console.ReadKey();
        switch (input.KeyChar)
        {
            case '1':
                StartMembersMenu();
                reset = true;
                break;
            case '2':
                reset = true;
                break;
            case '3':
                reset = true;
                break;
            case '0':
                return;
            default:
                Console.WriteLine("Nepoznata opcija. Pokušajte ponovno.");
                break;
        }
    }
}

static void PrintMainMenu()
{
    const string menu = @"1 - Članovi
2 - Posudba
3 - Statistika
0 - Izlaz
";

    Console.Clear();
    Console.WriteLine(menu);
}

static void PrintPrompt()
{
    Console.Write("Odaberite opciju: ");
}

static void StartMembersMenu()
{
    while (MembersMenuLoop()) ;
}

static bool MembersMenuLoop()
{
    const string menu = @"1 - Unos novog člana
2 - Brisanje člana
3 - Uređivanje člana
4 - Pregled svih članova
0 - Povratak na glavni izbornik
";
    const string prompt = "Odaberite opciju: ";

    Console.Clear();
    Console.WriteLine(menu);
    while (true)
    {
        Console.Write(prompt);
        var input = Console.ReadKey();

        switch (input.KeyChar)
        {
            case '1':
                return true;
            case '2':
                return true;
            case '3':
                return true;
            case '4':
                return true;
            case '0':
                return false;
            default:
                Console.WriteLine("Nepoznata opcija. Pokušajte ponovno.");
                break;
        }
    }
}