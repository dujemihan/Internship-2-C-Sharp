using Borrow = (int id, string title, string author, System.DateOnly borrowDate, bool returned);
using Member = (int id, string name, string surname, System.DateOnly dob,
    System.Collections.Generic.List<(int id, string title, string author, System.DateOnly borrowDate, bool returned)> borrows);
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
    while (LoopMembersMenu() != 0) ;
}

int LoopMembersMenu()
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
            return 1;
        case '2':
            DeleteMember();
            return 1;
        case '3':
            return 1;
        case '4':
            PrintAllMembers();
            return 1;
        default:
            return 0;
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

    members.Add((new Random().Next(), name, surname, date,
        new List<Borrow>()));

    Console.WriteLine("Unos je uspjesan.");
    WaitForKey();
}

void DeleteMember()
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
            DeleteMemberById();
            break;
        case '2':
            DeleteMemberByName();
            break;
        default:
            return;
    }
}

void DeleteMemberById()
{
    int id = -1;
    bool idValid = false;

    Console.Clear();
    while (!idValid) {
        Console.Write("Unesite ID clana: ");
        idValid = int.TryParse(Console.ReadLine(), out id);
    }

    var firstMember = members.Find(m => m.id == id);
    if (firstMember != default(Member))
    {
        DeleteFoundMember(firstMember);
    }
    else
    {
        Console.WriteLine("Navedeni clan ne postoji.");
    }

    WaitForKey();
}

void DeleteMemberByName()
{
    Console.Clear();

    var name = "";
    var surname = "";
    while (name == "")
    {
        Console.Write("Unesite ime: ");
        name = Console.ReadLine();
    }
    while (surname == "")
    {
        Console.Write("Unesite prezime: ");
        surname = Console.ReadLine();
    }

    var firstMember = members.Find(m => m.name == name && m.surname == surname);
    if (firstMember != default(Member))
    {
        DeleteFoundMember(firstMember);
    }
    else
    {
        Console.WriteLine("Navedeni clan ne postoji.");
    }
    WaitForKey();
}

void DeleteFoundMember(Member member)
{
    while (true)
    {
        Console.Write($"Pronaden clan [{member.id}] {member.surname}, {member.name} ({GetMemberAge(member)}). Zelite li ga izbrisati? (Y/N) ");
        var key = Console.ReadKey().KeyChar;
        Console.WriteLine("");
        if (key == 'y' || key == 'Y')
        {
            members.Remove(member);
            Console.WriteLine("Clan uspjesno izbrisan.");
            return;
        }
        else if (key == 'n' || key == 'N')
            return;
    }
}

void PrintAllMembers()
{
    Console.Clear();
    Console.WriteLine("Popis clanova:");

    foreach (var member in members)
    {
        Console.WriteLine($"[{member.id}] {member.surname}, {member.name} ({GetMemberAge(member)})");
    }

    WaitForKey();
}

int GetMemberAge(in Member member)
{
    var dob = member.dob;
    var today = DateTime.Today;
    var age = today.Year - dob.Year;

    if (dob.DayOfYear > today.DayOfYear)
        age--;

    return age;
}

void WaitForKey()
{
    Console.WriteLine("Pritisnite bilo koju tipku za nastavak...");
    Console.ReadKey();
}

char ShowMenu(in string menu, in char[] allowedOptions)
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