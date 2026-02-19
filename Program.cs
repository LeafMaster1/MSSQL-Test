

using Microsoft.Data.SqlClient;
using System.Data.SqlClient;

internal class Program
    {
        private static void Main(string[] args)
        {
            using var connection = new SqlConnection("Server=localhost,1433;Database=exempelbas;User ID=sa;Password=Lösenord!;Encrypt=True;TrustServerCertificate=True;");

            using var command = connection.CreateCommand();
            string createTableQuery = @"
            
            
            CREATE TABLE dbo.duck (
                id INT IDENTITY(1,1) PRIMARY KEY,
                namn NVARCHAR(100) NOT NULL,
                address_id INT NOT NULL,
                CONSTRAINT FK_duck_adresses FOREIGN KEY (adress_id) REFERENCES adresses(id)
            );
        ";
        }
    }