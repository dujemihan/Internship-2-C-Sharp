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
    char[] allowed = ['1', '2', '3', '4'];
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
    char[] allowed = ['1', '2', '3', '4'];
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
            StartEditMemberMenu();
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
        Console.Write("Unesite datum rođenja (YYYY-MM-DD): ");
        var dateString = Console.ReadLine() ?? "";

        if (!DateOnly.TryParse(dateString, out date))
            Console.WriteLine("Neispravan unos datuma, molimo pokušajte ponovno.");
        else if (date > DateOnly.FromDateTime(DateTime.Today))
            Console.WriteLine("Datum rođenja ne može biti u budućnosti!");
        else
            break;
    }

    members.Add((new Random().Next(), name, surname, date,
        new List<Borrow>()));

    Console.WriteLine("Unos je uspješan.");
    WaitForKey();
}

void DeleteMember()
{
    const string menu = @"1 - Po ID-ju
2 - Po imenu i prezimenu
0 - Povratak
";
    char[] allowed = ['1', '2'];
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
        Console.WriteLine("Navedeni član ne postoji.");
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
        Console.WriteLine("Navedeni član ne postoji.");
    }
    WaitForKey();
}

void DeleteFoundMember(in Member member)
{
    while (true)
    {
        Console .Write($"Pronađen član [{member.id}] {member.surname}, {member.name} ({GetMemberAge(member)}). Želite li ga izbrisati? (Y/N) ");
        var key = Console.ReadKey().KeyChar;
        Console.WriteLine();
        if (key == 'y' || key == 'Y')
        {
            foreach (var borrow in member.borrows)
            {
                if (!borrow.returned)
                {
                    Console.WriteLine("Nije moguće izbrisati člana s nevraćenim knjigama!");
                    return;
                }
            }
            members.Remove(member);
            Console.WriteLine("Član uspješno izbrisan.");
            return;
        }
        else if (key == 'n' || key == 'N')
            return;
    }
}

void StartEditMemberMenu()
{
    int id = -1;
    bool idValid = false;

    Console.Clear();
    while (!idValid)
    {
        Console.Write("Unesite ID člana: ");
        idValid = int.TryParse(Console.ReadLine(), out id);
    }

    var firstMember = members.Find(m => m.id == id);
    if (firstMember != default(Member))
    {
        EditMember(ref firstMember);
    }
    else
    {
        Console.WriteLine("Navedeni član ne postoji.");
    }

    WaitForKey();
}

void EditMember(ref Member member)
{
    Console.WriteLine($"Pronađen član [{member.id}] {member.surname}, {member.name} ({GetMemberAge(member)})");

    Console.Write("Unesite novo ime (prazno = nema promjene): ");
    var name = Console.ReadLine() ?? member.name;
    Console.Write("Unesite novo prezime (prazno = nema promjene): ");
    var surname = Console.ReadLine() ?? member.surname;
    Console.Write("Unesite novi datum rođenja (prazno = nema promjene): ");
    var dobValid = DateOnly.TryParse(Console.ReadLine(), out var dob) && dob <= DateOnly.FromDateTime(DateTime.Today);

    while (true)
    {
        Console.Write($"Član će se urediti u {surname}, {name} ({_GetMemberAge(dobValid ? dob : member.dob)}). Jeste li sigurni? (Y/N) ");
        var key = Console.ReadKey().KeyChar;
        Console.WriteLine("");

        if (key == 'y' || key == 'Y')
        {
            member.name = name;
            member.surname = surname;
            member.dob = dobValid ? dob : member.dob;
            Console.WriteLine("Izmjena uspješna.");
            break;
        }
        else if (key == 'n' || key == 'N')
            break;
    }
}

void PrintAllMembers()
{
    Console.Clear();
    Console.WriteLine("Popis članova:");

    foreach (var member in members)
    {
        Console.WriteLine($"[{member.id}] {member.surname}, {member.name} ({GetMemberAge(member)})");
    }

    WaitForKey();
}

int GetMemberAge(in Member member)
{
    return _GetMemberAge(member.dob);
}

int _GetMemberAge(in DateOnly dob)
{
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
        Console.WriteLine();
        if (option == '0' || allowedOptions.Contains(option))
            return option;

        Console.WriteLine("Nepoznata opcija. Pokušajte ponovno.");
    }
}