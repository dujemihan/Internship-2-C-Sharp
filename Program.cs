using Member = (string name, string surname, System.DateOnly dob,
    System.Collections.Generic.List<(string title, string author, System.DateOnly borrowDate)> borrows);
var members = new List<Member>
{
};

while (true)
{
    const string menu = @"1 - Članovi
2 - Posudba
3 - Statistika
0 - Izlaz
";
    char[] allowed = { '1', '2', '3', '4' };
    char option = ShowMenu(menu, allowed);

    switch (option)
    {
        case '1':
            StartMembersMenuLoop();
            break;
        case '2':
        case '3':
            break;
        case '0':
            return;
    }
}

void StartMembersMenuLoop()
{
    while (LoopMembersMenu()) ;
}

bool LoopMembersMenu()
{
    const string menu = @"1 - Unos novog člana
2 - Brisanje člana
3 - Uređivanje člana
4 - Pregled svih članova
0 - Povratak na glavni izbornik
";
    char[] allowed = { '1', '2', '3', '4' };
    char option = ShowMenu(menu, allowed);

    switch (option)
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
        default:
            return false;
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
    WaitForKey();
}

bool DeleteMember()
{
    const string menu = @"1 - Po ID-ju
2 - Po imenu i prezimenu
0 - Povratak
";
    char[] allowed = { '1', '2' };
    char option = ShowMenu(menu, allowed);
    
    switch (option)
    {
        case '1':
            return DeleteMemberById();
        case '2':
        default:
            return true;
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

    WaitForKey();
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

    WaitForKey();
}

void WaitForKey()
{
    Console.WriteLine("Pritisnite bilo koju tipku za nastavak...");
    Console.ReadKey();
}

char ShowMenu(in string menu, char[] allowedOptions)
{
    Console.Clear();
    Console.WriteLine(menu);

    while (true)
    {
        Console.Write("Odaberite opciju: ");
        char option = Console.ReadKey().KeyChar;
        if (option == '0' || allowedOptions.Contains(option))
            return option;

        Console.WriteLine("Nepoznata opcija. Pokušajte ponovno.");
        Console.ReadKey();
    }
}