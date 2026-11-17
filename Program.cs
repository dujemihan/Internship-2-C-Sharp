var members = new List<(
    string name,
    string surname,
    DateOnly dob,
    List<(string title, string author, DateOnly borrowDate)> borrows)>
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

void StartMembersMenu()
{
    while (MembersMenuLoop()) ;
}

bool MembersMenuLoop()
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
                AddNewMember();
                return true;
            case '2':
                return DeleteMember();
            case '3':
                return true;
            case '4':
                PrintAllMembers();
                return true;
            case '0':
                return false;
            default:
                Console.WriteLine("Nepoznata opcija. Pokušajte ponovno.");
                break;
        }
    }
}

void AddNewMember()
{
    var name = "";
    var surname = "";
    var dateString = "";
    DateOnly date;

    Console.Clear();
    while (name == "")
    {
        Console.Write("Unesite ime: ");
        name = Console.ReadLine() ?? "";
        if (name == "")
            Console.WriteLine("Molimo unesite ime!");
    }
    while (surname == "")
    {
        Console.Write("Unesite prezime: ");
        surname = Console.ReadLine() ?? "";
        if (surname == "")
            Console.WriteLine("Molimo unesite prezime!");
    }
    while (true)
    {
        Console.Write("Unesite datum rodenja (YYYY-MM-DD): ");
        dateString = Console.ReadLine() ?? "";

        if (!DateOnly.TryParse(dateString, out date))
        {
            Console.WriteLine("Neispravan unos datuma, molimo pokusajte ponovno.");
        }
        else
            break;
    }

    members.Add((name, surname, date,
        new List<(string title, string author, DateOnly borrowDate)>()));

    Console.WriteLine("Unos je uspjesan.");
    WaitKey();
}

bool DeleteMember()
{
    const string menu = @"1 - Po ID-ju
2 - Po imenu i prezimenu
0 - Povratak
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
                return DeleteMemberById();
            case '2':
                return true;
            default:
                Console.WriteLine("Nepoznata opcija. Pokušajte ponovno.");
                break;
        }
    }
}

bool DeleteMemberById()
{
    int id = -1;
    bool idValid = false;

    Console.Clear();
    while (!idValid) {
        Console.Write("Unesite ID clana: ");
        idValid = int.TryParse(Console.ReadLine(), out id);
    }

    try
    {
        var member = members[id];
        members.Remove(member);
        Console.WriteLine("Clan uspjesno izbrisan.");
    } catch (ArgumentOutOfRangeException)
    {
        Console.WriteLine("Navedeni clan ne postoji.");
        idValid = false;
    }

    WaitKey();
    return idValid;
}

void PrintAllMembers()
{
    Console.Clear();
    Console.WriteLine("Popis clanova:");

    foreach (var member in members)
    {
        Console.WriteLine($"[{members.IndexOf(member)}] {member.surname}, {member.name} ({DateTime.Now.AddYears(-member.dob.Year).Year})");
    }

    WaitKey();
}

void WaitKey()
{
    Console.WriteLine("Pritisnite bilo koju tipku za nastavak...");
    Console.ReadKey();
}