using System.Text;
using Borrow = (int id, int memberId, string title, string author, System.DateOnly borrowDate, System.DateOnly? returnDate);
using Member = (int id, string name, string surname, System.DateOnly dob);
var members = new List<Member>
{
    (512369754, "Ante", "Antić", new DateOnly(2006, 5, 30)),
    (574890313, "Mate", "Matić", new DateOnly(2005, 6, 12)),
    (342780942, "Hrvoje", "Horvat", new DateOnly(2010, 11, 12))
};
var borrows = new List<Borrow>
{
    (48912778, 512369754, "Idiot", "Fjodor Mihajlovič Dostojevski", new DateOnly(2026, 3, 25), new DateOnly(2026, 4, 18)),
    (48912779, 512369754, "Zločin i kazna", "Fjodor Mihajlovič Dostojevski", new DateOnly(2026, 9, 1), null),
    (48912780, 574890313, "Na Drini ćuprija", "Ivo Andrić", new DateOnly(2026, 8, 10), new DateOnly(2026, 9, 2)),
    (48912781, 574890313, "Mali princ", "Antoine de Saint-Exupéry", new DateOnly(2026, 9, 5), null),
    (48912782, 342780942, "Prokleta avlija", "Ivo Andrić", new DateOnly(2026, 7, 1), new DateOnly(2026, 7, 20)),
    (48912783, 342780942, "Harry Potter i kamen mudraca", "J. K. Rowling", new DateOnly(2026, 8, 15), new DateOnly(2026, 9, 10))
};

Console.OutputEncoding = Encoding.UTF8;

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
            StartBorrowsMenuLoop();
            break;
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

void StartBorrowsMenuLoop()
{
    while (LoopBorrowsMenu() != 0) ;
}

int LoopBorrowsMenu()
{
    const string menu = @"1 - Unos nove posudbe
2 - Vraćanje knjige
3 - Brisanje posudbe
4 - Uređivanje postojeće posudbe
5 - Pregled svih posudbi
6 - Izvještaji i analize
0 - Povratak na glavni izbornik
";
    char[] allowed = ['1', '2', '3', '4', '5', '6'];
    char option = ShowMenu(menu, allowed);

    switch (option)
    {
        case '1':
            AddNewBorrow();
            return 1;
        case '2':
            ReturnBook();
            return 1;
        case '3':
            DeleteBorrow();
            return 1;
        default:
            return 0;
    }
}

void DeleteBorrow()
{
    Console.Clear();

    if (!HasChosenBorrowFromList(out var borrow))
    {
        WaitForKey();
        return;
    }
    else if (borrow.returnDate == null)
    {
        Console.WriteLine("Nije moguće izbrisati nezaključenu posudbu!");
        WaitForKey();
        return;
    }

    PrintBorrow(borrow);
    if (IsPromptPositive("Želite li izbrisati ovu posudbu?"))
    {
        borrows.Remove(borrow);
        Console.WriteLine("Posudba uspješno izbrisana.");
    }

    WaitForKey();
    return;
}

void AddNewBorrow()
{
    Console.Clear();

    if (!HasChosenMemberFromList(out var member))
    {
        WaitForKey();
        return;
    }

    string author = "";
    string title = "";
    DateOnly borrowDate;

    while (title == "")
    {
        Console.Write("Unesite naslov knjige: ");
        title = Console.ReadLine() ?? "";
    }

    while (author == "")
    {
        Console.Write("Unesite autora knjige: ");
        author = Console.ReadLine() ?? "";
    }

    while (true)
    {
        Console.Write("Unesite datum posudbe (YYYY-MM-DD): ");
        if (!DateOnly.TryParse(Console.ReadLine(), out borrowDate))
        {
            Console.WriteLine("Neispravan unos datuma, molimo pokušajte ponovno.");
        }
        else if (borrowDate > DateOnly.FromDateTime(DateTime.Today))
        {
            Console.WriteLine("Datum posudbe ne može biti u budućnosti!");
        }
        else
            break;
    }

    borrows.Add((new Random().Next(), member.id, title, author, borrowDate, null));
    Console.WriteLine("Posudba uspješno dodana.");

    WaitForKey();
}

void ReturnBook()
{
    Console.Clear();

    if (!HasChosenBorrowFromList(out var borrow))
    {
        WaitForKey();
        return;
    }

    if (borrow.returnDate != default)
    {
        Console.WriteLine("Ova je posudba već zaključena!");
        WaitForKey();
        return;
    }

    DateOnly returnDate;
    while (true)
    {
        Console.Write("Unesite datum vraćanja: ");
        if (!DateOnly.TryParse(Console.ReadLine(), out returnDate))
            Console.WriteLine("Neispravan unos datuma, molimo pokušajte ponovno.");
        else if (returnDate < borrow.borrowDate)
            Console.WriteLine("Nije moguće vratiti knjigu prije posudbe!");
        else if (returnDate > DateOnly.FromDateTime(DateTime.Today))
            Console.WriteLine("Datum vraćanja ne može biti u budućnosti!");
        else
            break;
    }

    borrow.returnDate = returnDate;
    int borrowIndex = borrows.FindIndex(b => b.id == borrow.id);
    if (borrowIndex >= 0)
        borrows[borrowIndex] = borrow;

    var isLate = IsLate(borrow, returnDate, out int days);
    Console.WriteLine($"Knjiga uspješno vraćena. Vraćanje {(isLate ? $"je zakašnjelo {days} dana, a zakasnina iznosi {days * 0.5} €" : "nije zakašnjelo")}.");

    WaitForKey();
}

bool HasChosenMemberFromList(out Member member)
{
    member = default;

    if (members.Count == 0)
    {
        Console.WriteLine("Nema članova, molimo unesite člana.");
        return false;
    }

    for (int i = 0; i < members.Count; i++)
    {
        Console.WriteLine($"{i + 1}. [{members[i].id}] {members[i].surname}, {members[i].name} ({GetMemberAge(members[i])})");
    }

    int choice;
    while (true)
    {
        Console.Write("\nUnesite člana (prazno za odustajanje): ");
        if (int.TryParse(Console.ReadLine(), out choice) && choice > 0 && choice <= members.Count)
            break;
        else if (choice == 0)
            return false;
    }

    member = members[choice - 1];
    return true;
}

bool HasChosenBorrowFromList(out Borrow borrow)
{
    borrow = default;
    if (borrows.Count == 0)
    {
        Console.WriteLine("Nema posudbi, molimo unesite posudbu.");
        return false;
    }

    PrintBorrows();

    int choice;
    while (true)
    {
        Console.Write("\nUnesite posudbu (prazno za odustajanje): ");
        if (int.TryParse(Console.ReadLine(), out choice) && choice > 0 && choice <= borrows.Count)
            break;
        else if (choice == 0)
            return false;
    }

    borrow = borrows[choice - 1];
    return true;
}

void PrintBorrows()
{
    for (int i = 0; i < borrows.Count; i++)
    {
        var b = borrows[i];
        Console.Write($"{i + 1}. ");
        PrintBorrow(b);
    }
}

void PrintBorrow(in Borrow b)
{
    var returnDate = b.returnDate ?? DateOnly.FromDateTime(DateTime.Today);
    var isLate = IsLate(b, returnDate, out int days);

    Console.WriteLine($"[{b.id}] \"{b.title}\" - {b.author} | posuđeno: {b.borrowDate} | {(b.returnDate != null ? $"vraćeno: {b.returnDate}" : $"nije vraćeno | kašnjenje: {days} dana | zakasnina: {days * 0.5} €")} ");
}

bool IsLate(in Borrow borrow, DateOnly returnDate, out int days)
{
    var deadline = borrow.borrowDate.AddDays(21);
    days = 0;

    if (deadline < returnDate)
    {
        days = (returnDate.DayNumber - deadline.DayNumber);
        return true;
    }
    else
        return false;
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

    members.Add((new Random().Next(), name, surname, date));

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
    while (!idValid)
    {
        Console.Write("Unesite ID clana: ");
        idValid = int.TryParse(Console.ReadLine(), out id);
    }

    var firstMember = members.Find(m => m.id == id);
    if (firstMember != default)
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
    if (firstMember != default)
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
    if (IsPromptPositive($"Pronađen član [{member.id}] {member.surname}, {member.name} ({GetMemberAge(member)}). Želite li ga izbrisati? (Y/N) "))
    {
        foreach (var borrow in borrows)
        {
            if (borrow.memberId == member.id && borrow.returnDate == null)
            {
                Console.WriteLine("Nije moguće izbrisati člana s nevraćenim knjigama!");
                return;
            }
        }
        members.Remove(member);
        Console.WriteLine("Član uspješno izbrisan.");
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

    int memberIndex = members.FindIndex(m => m.id == id);
    if (memberIndex >= 0)
    {
        var member = members[memberIndex];
        EditMember(ref member);
        members[memberIndex] = member;
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

    if (IsPromptPositive($"Član će se urediti u {surname}, {name} ({_GetMemberAge(dobValid ? dob : member.dob)}). Jeste li sigurni?"))
    {
        member.name = name;
        member.surname = surname;
        member.dob = dobValid ? dob : member.dob;
        Console.WriteLine("Izmjena uspješna.");
    }

    WaitForKey();
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

bool IsPromptPositive(in string prompt)
{
    while (true)
    {
        Console.Write($"{prompt} (Y/N) ");
        var choice = Console.ReadKey().KeyChar;
        Console.WriteLine();
        if (choice == 'y' || choice == 'Y')
            return true;
        else if (choice == 'n' || choice == 'N')
            return false;
    }
}