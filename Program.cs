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
    (48912778, 512369754, "Idiot", "Fjodor Mihajlovič Dostojevski", new DateOnly(2025, 7, 25), new DateOnly(2026, 8, 18)),
    (48912779, 512369754, "Zločin i kazna", "Fjodor Mihajlovič Dostojevski", new DateOnly(2026, 9, 1), null),
    (48912780, 574890313, "Na Drini ćuprija", "Ivo Andrić", new DateOnly(2026, 8, 10), new DateOnly(2026, 9, 2)),
    (48912781, 574890313, "Mali princ", "Antoine de Saint-Exupéry", new DateOnly(2026, 9, 5), null),
    (48912782, 342780942, "Prokleta avlija", "Ivo Andrić", new DateOnly(2026, 7, 1), new DateOnly(2026, 7, 20)),
    (48912783, 342780942, "Harry Potter i kamen mudraca", "J. K. Rowling", new DateOnly(2026, 8, 15), new DateOnly(2026, 9, 10))
};

const int BORROW_LENGTH         = 21;
const double LATE_FEE_PER_DAY   = 0.5;

Console.OutputEncoding = Encoding.UTF8;

while (true)
{
    const string menu = @"PROGRAM ZA VOĐENJE KNJIŽNICE

1 - Članovi
2 - Posudba
0 - Izlaz
";
    char[] allowed = ['1', '2'];
    char option = ShowMenu(menu, allowed);

    switch (option)
    {
        case '1':
            StartMembersMenuLoop();
            break;
        case '2':
            StartBorrowsMenuLoop();
            break;
        //case '3':
        //    break;
        default:
            return;
    }
}

void StartMembersMenuLoop()
{
    const string menu = @"ČLANOVI

1 - Unos novog člana
2 - Brisanje člana
3 - Uređivanje člana
4 - Pregled svih članova
0 - Povratak na glavni izbornik
";
    char[] allowed = ['1', '2', '3', '4'];

    while (true)
    {
        switch (ShowMenu(menu, allowed))
        {
            case '1':
                AddNewMember();
                break;
            case '2':
                DeleteMember();
                break;
            case '3':
                StartEditMemberMenu();
                break;
            case '4':
                PrintAllMembers();
                Console.WriteLine("Svi članovi mlađi od 18:");
                foreach (var member in members)
                {
                    if (GetMemberAge(member) < 18)
                        PrintMember(member);
                }
                Console.WriteLine("Svi članovi s barem jednom nevraćenom knjigom:");
                foreach (var member in members)
                {
                    var memberId = member.id;

                    foreach (var borrow in borrows)
                    {
                        if (memberId == borrow.memberId && !borrow.returnDate.HasValue)
                        {
                            PrintMember(member);
                            break;
                        }
                    }
                }
                WaitForKey();
                break;
            default:
                return;
        }
    }
}

void AddNewMember()
{
    DateOnly date;

    Console.Clear();
    var name = ReadRequiredString("Unesite ime", "Molimo unesite ime!");
    var surname = ReadRequiredString("Unesite prezime", "Molimo unesite prezime!");
    while (true)
    {
        var dateString = ReadRequiredString("Unesite datum rođenja (YYYY-MM-DD)", "Molimo unesite datum!");

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
    int id;

    Console.Clear();
    while (!int.TryParse(ReadRequiredString("Unesite ID člana", "Molimo unesite ID!"), out id)) ;

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

    var name = ReadRequiredString("Unesite ime", "Molimo unesite ime!");
    var surname = ReadRequiredString("Unesite prezime", "Molimo unesite ime!");

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
            if (borrow.memberId == member.id && !borrow.returnDate.HasValue)
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

    var name = ReadOptionalString("Unesite novo ime (prazno = nema promjene)", member.name);
    var surname = ReadOptionalString("Unesite novo prezime (prazno = nema promjene)", member.surname);
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
        PrintMember(member);
    }
}

void PrintMember(in Member member)
{
    Console.WriteLine($"[{member.id}] {member.surname}, {member.name} ({GetMemberAge(member)})");
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

void StartBorrowsMenuLoop()
{
    const string menu = @"POSUDBE

1 - Unos nove posudbe
2 - Vraćanje knjige
3 - Brisanje posudbe
4 - Uređivanje postojeće posudbe
5 - Pregled svih posudbi
6 - Izvještaji i analize
0 - Povratak na glavni izbornik
";
    char[] allowed = ['1', '2', '3', '4', '5', '6'];

    while (true)
    {
        switch (ShowMenu(menu, allowed))
        {
            case '1':
                AddNewBorrow();
                break;
            case '2':
                ReturnBook();
                break;
            case '3':
                StartDeleteBorrowMenu();
                break;
            case '4':
                EditBorrow();
                break;
            case '5':
                StartPrintBorrowsMenuLoop();
                WaitForKey();
                break;
            case '6':
                StartAnalyticsMenuLoop();
                WaitForKey();
                break;
            default:
                return;
        }
    }
}

void StartPrintBorrowsMenuLoop()
{
    const string menu = @"PREGLED SVIH POSUDBI

1 - Sve posudbe po redu
2 - Sortirane po datumu posudbe
3 - Sortirano po zakasnini
4 - Sve nevraćene posudbe
5 - Pretraga po naslovu/autoru
6 - Grupirane po mjesecu
";
    char[] allowed = ['1', '2', '3', '4', '5', '6'];

    while (true)
    {
        switch (ShowMenu(menu, allowed))
        {
            case '1':
                PrintBorrows(borrows);
                break;
            case '2':
                {
                    IOrderedEnumerable<Borrow> sortedBorrows;

                    if (IsPromptPositive("Želite li sortirati uzlazno?"))
                        sortedBorrows = borrows.OrderBy(b => b.borrowDate);
                    else
                        sortedBorrows = borrows.OrderByDescending(b => b.borrowDate);

                    PrintBorrows(sortedBorrows);
                    break;
                }
            case '3':
                {
                    Dictionary<int, int> lateDays = [];
                    
                    foreach (var borrow in borrows)
                    {
                        var compDate = borrow.returnDate ?? DateOnly.FromDateTime(DateTime.Today);
                        IsLate(borrow, compDate, out int days);
                        lateDays.Add(borrow.id, days);
                    }

                    if (IsPromptPositive("Želite li sortirati uzlazno?"))
                    {
                        var sortedLateDays = lateDays.OrderBy(kv => kv.Value);
                        foreach (var (k, v) in sortedLateDays)
                            PrintBorrow(borrows.Where(b => b.id == k).FirstOrDefault(), null);
                    }
                    else
                    {
                        var sortedLateDays = lateDays.OrderByDescending(kv => kv.Value);
                        foreach (var (k, v) in sortedLateDays)
                            PrintBorrow(borrows.Where(b => b.id == k).FirstOrDefault(), null);
                    }

                    break;
                }
            case '4':
                {
                    var unreturnedBorrows = borrows.Where(b => !b.returnDate.HasValue);
                    Console.WriteLine("Nevraćene posudbe:");
                    PrintBorrows(unreturnedBorrows);
                    break;
                }
            case '5':
                {
                    var term = ReadRequiredString("Unesite pojam za pretragu", "Molimo unesite pojam!");
                    var results = borrows.Where(b => b.author.Contains(term,
                        StringComparison.CurrentCultureIgnoreCase) || b.title.Contains(term, StringComparison.CurrentCultureIgnoreCase));
                    Console.WriteLine("Rezultati pretrage:");
                    PrintBorrows(results);
                    break;
                }
            case '6':
                {
                    var groups = borrows.GroupBy(b => new { b.borrowDate.Month, b.borrowDate.Year }).OrderByDescending(b => b.Key.Year).ThenByDescending(b => b.Key.Month);
                    foreach (var group in groups)
                    {
                        Console.WriteLine($"{group.Key.Year}-{group.Key.Month}");
                        PrintBorrows(group);
                    }
                    break;
                }
            default:
                return;
        }
        WaitForKey();
    }
}

void StartAnalyticsMenuLoop()
{
    const string menu = @"IZVJEŠTAJI I ANALIZE

1 - Ukupan broj posudbi i broj nevraćenih knjiga
2 - Ukupna zakasnina
3 - Prosječno trajanje posudbe
4 - Posudba s najvećim kašnjenjem
5 - Posudbe u vremenskom rasponu
0 - Povratak
";
    char[] allowed = ['1', '2', '3', '4', '5'];

    int id;
    while (!int.TryParse(ReadRequiredString("Unesite ID člana (0 za odustati)", "Molimo unesite ID!"), out id)) ;
    if (id == 0)
        return;

    var member = members.Find(m => m.id == id);
    if (member == default)
    {
        Console.WriteLine("Član ne postoji.");
        return;
    }

    while (true)
    {
        switch (ShowMenu(menu, allowed))
        {
            case '1':
                {
                    var memberBorrows = borrows.Where(b => b.memberId == id);
                    Console.WriteLine($"Ukupan broj posudbi jest {memberBorrows.Count()}, a nevraćenih knjiga ima {memberBorrows.Where(b => b.returnDate is null).Count()}.");
                    break;
                }
            case '2':
                {
                    var totalDays = 0;
                    foreach (var borrow in borrows.Where(b => b.memberId == id))
                    {
                        var compDate = borrow.returnDate ?? DateOnly.FromDateTime(DateTime.Today);
                        totalDays += int.Clamp(compDate.DayNumber - borrow.borrowDate.DayNumber - BORROW_LENGTH, 0, int.MaxValue);
                    }
                    Console.WriteLine($"Ukupna zakasnina za člana iznosi {totalDays * LATE_FEE_PER_DAY} €.");
                    break;
                }
            case '3':
                {
                    var memberBorrows = borrows.Where(b => b.memberId == id);
                    var totalDays = 0;
                    foreach (var borrow in memberBorrows)
                    {
                        var compDate = borrow.returnDate ?? DateOnly.FromDateTime(DateTime.Today);
                        totalDays += compDate.DayNumber - borrow.borrowDate.DayNumber;
                    }
                    Console.WriteLine($"Prosječna posudba člana traje {(double)totalDays / memberBorrows.Count()} dana.");
                    break;
                }
            case '4':
                {
                    var memberBorrows = borrows.Where(b => b.memberId == id);
                    Dictionary<int, int> lateDays = [];

                    foreach (var borrow in memberBorrows)
                    {
                        var compDate = borrow.returnDate ?? DateOnly.FromDateTime(DateTime.Today);
                        lateDays[borrow.id] = int.Clamp(compDate.DayNumber - borrow.borrowDate.DayNumber - BORROW_LENGTH, 0, int.MaxValue);
                    }
                    var max = lateDays.OrderByDescending(kvp => kvp.Value).FirstOrDefault();
                    Console.WriteLine($"Najkasnija članova posudba kasni/je kasnila {max.Value} dana.");
                    break;
                }
            case '5':
                {
                    DateOnly start, end;
                    while (!DateOnly.TryParse(ReadRequiredString("Unesite početni datum (YYYY-MM-DD)", "Molimo unesite datum!"), out start)) ;
                    while (!DateOnly.TryParse(ReadRequiredString("Unesite krajnji datum (YYYY-MM-DD)", "Molimo unesite datum!"), out end)) ;

                    var memberBorrows = borrows.Where(b => b.memberId == id && b.borrowDate >= start && b.borrowDate <= end);
                    PrintBorrows(memberBorrows);
                }
                break;
            default:
                return;
        }
        WaitForKey();
    }
}

void AddNewBorrow()
{
    Console.Clear();

    if (!HasChosenMemberFromList(out var member))
    {
        WaitForKey();
        return;
    }

    DateOnly borrowDate;
    var title = ReadRequiredString("Unesite naslov knjige", "Molimo unesite naslov!");
    var author = ReadRequiredString("Unesite autora knjige", "Molimo unesite autora!");

    while (true)
    {
        if (!DateOnly.TryParse(ReadRequiredString("Unesite datum posudbe (YYYY-MM-DD)", "Molimo unesite datum!"), out borrowDate))
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

    int id, index;
    while (!int.TryParse(ReadRequiredString("Unesite ID posudbe", "Molimo unesite ID!"), out id)) ;

    index = borrows.FindIndex(b => b.id == id);
    if (index < 0)
    {
        Console.WriteLine("Ova posudba ne postoji.");
        WaitForKey();
        return;
    }

    var borrow = borrows[index];
    if (borrow.returnDate != default)
    {
        Console.WriteLine("Ova je posudba već zaključena!");
        WaitForKey();
        return;
    }

    DateOnly returnDate;
    while (true)
    {
        if (!DateOnly.TryParse(ReadRequiredString("Unesite datum vraćanja", "Molimo unesite datum!"), out returnDate))
            Console.WriteLine("Neispravan unos datuma, molimo pokušajte ponovno.");
        else if (returnDate < borrow.borrowDate)
            Console.WriteLine("Nije moguće vratiti knjigu prije posudbe!");
        else if (returnDate > DateOnly.FromDateTime(DateTime.Today))
            Console.WriteLine("Datum vraćanja ne može biti u budućnosti!");
        else
            break;
    }

    PrintBorrow(borrow, returnDate);
    if (IsPromptPositive("Želite li vratiti ovu knjigu?"))
    {
        var isLate = IsLate(borrow, returnDate, out int days);

        borrow.returnDate = returnDate;
        borrows[index] = borrow;
        Console.WriteLine($"Knjiga uspješno vraćena. Vraćanje {(isLate ? $"je zakašnjelo {days} dana, a zakasnina iznosi {days * LATE_FEE_PER_DAY} €" : "nije zakašnjelo")}.");
    }

    WaitForKey();
}

void EditBorrow()
{
    int id;

    while (!int.TryParse(ReadRequiredString("Unesite ID posudbe", "Molimo unesite ID!"), out id)) ;

    var borrow = borrows.Find(b => b.id == id);
    if (borrow == default)
    {
        Console.WriteLine("Posudba ne postoji.");
        WaitForKey();
        return;
    }
    var borrowIndex = borrows.IndexOf(borrow);

    PrintBorrow(borrow, null);

    borrow.title = ReadOptionalString("Unesite novi naslov (prazno = nema promjene)", borrow.title);
    borrow.author = ReadOptionalString("Unesite novog autora (prazno = nema promjene)", borrow.author);
    Console.Write("Unesite novi datum posudbe (prazno ili nevaljano = nema promjene): ");
    if (!DateOnly.TryParse(Console.ReadLine()?.Trim(), out var borrowDate))
        borrowDate = borrow.borrowDate;
    borrow.borrowDate = borrowDate;

    PrintBorrow(borrow, null);

    if (IsPromptPositive("Želite li promijeniti posudbu?"))
    {
        borrows[borrowIndex] = borrow;
        Console.WriteLine("Promjena uspješna.");
    }

    WaitForKey();
}

void StartDeleteBorrowMenu()
{
    const string menu = @"1 - Po ID-ju
2 - Sve vraćene određenog člana
3 - Sve s zakasninom većom od iznosa
0 - Povratak
";
    char[] allowed = ['1', '2', '3'];

    while (true)
    {
        switch (ShowMenu(menu, allowed))
        {
            case '1':
                DeleteBorrowById();
                break;
            case '2':
                DeleteBorrowsByMember();
                break;
            case '3':
                DeleteBorrowsByFee();
                break;
            default:
                return;
        }
    }
}

void DeleteBorrowById()
{
    Console.Clear();

    int id, index;
    while (!int.TryParse(ReadRequiredString("Unesite ID posudbe", "Molimo unesite ID!"), out id)) ;

    index = borrows.FindIndex(b => b.id == id);
    if (index < 0)
    {
        Console.WriteLine("Posudba ne postoji.");
        WaitForKey();
        return;
    }

    var borrow = borrows[index];
    if (borrow.returnDate.HasValue)
    {
        Console.WriteLine("Nije moguće izbrisati nezaključenu posudbu!");
        WaitForKey();
        return;
    }

    PrintBorrow(borrow, null);
    if (IsPromptPositive("Želite li izbrisati ovu posudbu?"))
    {
        borrows.RemoveAt(index);
        Console.WriteLine("Posudba uspješno izbrisana.");
    }

    WaitForKey();
    return;
}

void DeleteBorrowsByMember()
{
    Console.Clear();

    int id;
    while (!int.TryParse(ReadRequiredString("Unesite ID člana", "Molimo unesite ID!"), out id)) ;

    var memberIndex = members.FindIndex(m => m.id == id);
    if (memberIndex < 0)
    {
        Console.WriteLine("Član nije pronađen.");
        WaitForKey();
        return;
    }

    List<Borrow> targetBorrows = [];
    foreach (var borrow in borrows)
    {
        if (borrow.memberId == id && !borrow.returnDate.HasValue)
        {
            targetBorrows.Add(borrow);
            PrintBorrow(borrow, borrow.returnDate);
        }
    }

    if (IsPromptPositive("Želite li izbrisati ove posudbe?"))
    {
        foreach (var borrow in targetBorrows)
            borrows.Remove(borrow);
        Console.WriteLine("Posudbe uspješno izbrisane.");
    }

    WaitForKey();
}

void DeleteBorrowsByFee()
{
    List<Borrow> targetBorrows = [];

    double threshold;
    while (!double.TryParse(ReadRequiredString("Unesite decimalan prag u €", "Molimo unesite prag!"), out threshold)) ;
    if (threshold <= 0)
    {
        Console.WriteLine("Prag je 0 ili negativan, odustajanje");
        WaitForKey();
        return;
    }

    foreach (var borrow in borrows)
    {
        var compDate = borrow.returnDate ?? DateOnly.FromDateTime(DateTime.Today);
        var isLate = IsLate(borrow, compDate, out int days);

        if (isLate && days >= threshold * 2)
        {
            targetBorrows.Add(borrow);
            PrintBorrow(borrow, borrow.returnDate);
        }
    }

    if (targetBorrows.Count < 1)
    {
        Console.WriteLine("Nema posudbi iznad navedenog praga.");
        WaitForKey();
        return;
    }

    if (IsPromptPositive("Želite li izbrisati ove posudbe?"))
    {
        foreach (var borrow in targetBorrows)
            borrows.Remove(borrow);
        Console.WriteLine("Posudbe uspješno izbrisane.");
    }

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

void PrintBorrows(IEnumerable<Borrow> borrowArray)
{
    for (int i = 0; i < borrowArray.Count(); i++)
    {
        var b = borrowArray.ElementAt(i);
        Console.Write($"{i + 1}. ");
        PrintBorrow(b, b.returnDate);
    }
}

void PrintBorrow(in Borrow b, DateOnly? returnDate)
{
    var compDate = b.returnDate ?? returnDate ?? DateOnly.FromDateTime(DateTime.Today);
    var isLate = IsLate(b, compDate, out int days);

    Console.WriteLine($"[{b.id}] \"{b.title}\" - {b.author} | posuđeno: {b.borrowDate} | {(b.returnDate.HasValue ? $"vraćeno: {b.returnDate}" : $"nije vraćeno | kašnjenje: {days} dana | zakasnina: {days * LATE_FEE_PER_DAY} €")} ");
}

bool IsLate(in Borrow borrow, DateOnly returnDate, out int days)
{
    var deadline = borrow.borrowDate.AddDays(BORROW_LENGTH);
    days = 0;

    if (deadline < returnDate)
    {
        days = returnDate.DayNumber - deadline.DayNumber;
        return true;
    }
    else
        return false;
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

string ReadRequiredString(string prompt, string? invalidMsg)
{
    while (true)
    {
        var str = ReadOptionalString(prompt, "");

        if (str == "")
        {
            if (invalidMsg is not null)
                Console.WriteLine(invalidMsg);
        }
        else
            return str;
    }
}

string ReadOptionalString(string prompt, string fallback)
{
    Console.Write($"{prompt}: ");
    var str = Console.ReadLine()?.Trim();

    if (string.IsNullOrWhiteSpace(str))
        return fallback;
    else
        return str;
}
