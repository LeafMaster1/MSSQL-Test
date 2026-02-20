using Microsoft.Data.SqlClient;
using Dapper;
using System.Security.Cryptography.X509Certificates;



using var connection = new SqlConnection("Server=localhost,1433;Database=Exempelbase;User ID=sa;Password=Lösenord!;Encrypt=True;TrustServerCertificate=True;");

connection.Open();

var sql = @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Authors')
            CREATE TABLE Authors(
            Id INT IDENTITY (1,1) PRIMARY KEY,
            Name NVARCHAR(100) UNIQUE NOT NULL

            );";
connection.Execute(sql);
    sql = @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Book')
    CREATE TABLE Book(
    Id INT IDENTITY (1,1) PRIMARY KEY,
    Titel NVARCHAR(100) UNIQUE NOT NULL
    );";

connection.Execute(sql);

sql = @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'BookAuthors')
CREATE TABLE BookAuthors(
AuthorId INT NOT NULL FOREIGN KEY REFERENCES Authors(Id) ON DELETE CASCADE,
BookId INT NOT NULL FOREIGN KEY REFERENCES Book(Id) ON DELETE CASCADE,
PRIMARY KEY (AuthorId, BookId)
);"    
;
connection.Execute(sql);
if (args.Length > 0 && (args[0] == "list" && args[1] == "authors" || args[0] == "l" && args[1] == "a"))
{
    var authors = connection.Query<string>("SELECT Name FROM Authors");
    foreach (var auth in authors)
    {
        Console.WriteLine("All authors: " + auth);

    }
}

if (args.Length > 0 && (args[0] == "add" && args[1] == "author" || args[0] == "a" && args[1] == "a"))
{
    if (args.Length < 2)
    {
        Console.WriteLine("Skriv add author 'name' eller a a 'name' för att lägga till. ");
    }

    var author = args[2];
    sql = @"
        INSERT INTO Authors(Name)
        VALUES(@Name)";

    connection.Execute(sql, new { Name = author });
    Console.WriteLine($"Författaren {author} har lagts till.");
}
if (args.Length > 0 && (args[0] == "remove" && args[1] == "author" || args[0] == "r" && args[1] == "a"))
{
    var author = args[2];
    var rows = connection.Execute("DELETE FROM Authors WHERE Name = @Name", new { Name = author });
    if (rows == 0)
    {
        Console.WriteLine("Kunde inte hitta någon med det namnet");
    }
    else
    {
        Console.WriteLine($"Författaren: {author} har tagits bort. ");
    }


}
    if (args.Length > 0 && (args[0] == "add" && args[1] == "book" || args[0] == "a" && args[1] == "b"))
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Skriv add book 'book' eller a b 'book' för att lägga till bok sedan skriv titel. ");
        }


        var book = args[2];
        sql = @"
        INSERT INTO Book(Titel)
        VALUES(@Titel)";

        connection.Execute(sql, new { Titel = book });
        Console.WriteLine($"Titel på {book} har lagts till.");
    }
if (args.Length > 0 && (args[0] == "list" && args[1] == "book" || args[0] == "l"  &&  args[1] == "b"))
{
    var book = connection.Query<string>("SELECT Titel FROM Book");
    foreach (var books in book)
    {
        Console.WriteLine("All Books: " + books);
    }


}
if (args.Length > 0 && (args[0] == "remove" && args[1] == "book" || args[0] == "r" && args[1] == "b"))
{
    var book = args[2];
    var rows = connection.Execute("DELETE FROM Book WHERE Titel = @Titel", new { Titel = book });
    if (rows == 0)
    {
        Console.WriteLine("Kunde inte hitta någon med det titeln");
    }
    else
    {
        Console.WriteLine($"Titeln: {book} har tagits bort. ");
    }
}

if (args.Length >= 6 &&
((args[0] == "modify" || args[0] == "m") &&
    (args[1] == "author" || args[1] == "a")))
{ 
var authorName = args[2];
var action = args[3];
var target = args[4];
var bookName = args[5];

    if ((action == "add" || action == "a") &&
        (target == "book" || target == "b"))
    {
        var authorId = connection.QuerySingleOrDefault<int?>(
            "SELECT Id FROM Authors WHERE Name = @name",
            new { name = authorName });
        if(authorId == null)
        {
            Console.WriteLine($"Author {authorName} not found ");
            return;
        }
        var bookId = connection.QuerySingleOrDefault<int?>(
            "SELECT Id FROM Book WHERE Titel = @titel",
            new { titel = bookName });

        connection.Execute(@"
        INSERT INTO BookAuthors(AuthorId, BookId)
        VALUES(@author, @book)", new { author = authorId, book = bookId });
        
        Console.WriteLine($"{authorName} länkades med {bookName}");
    }
    
}