// See https://aka.ms/new-console-template for more informati
using System;
using System.Data.SqlClient;
using System.IO;

class Program
{
    
    static void Main()
    {
        string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=DocumentDatabase;Trusted_Connection=True"; ;
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            
            SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                
                InsertDocument(connection, transaction, "Sample Document", "sample.docx", GetDocumentBytes());

                
                transaction.Commit();
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error: {ex.Message}");
                transaction.Rollback();
            }
            finally
            {
                
                connection.Close();
            }
            
        }
    }

    static void InsertDocument(SqlConnection connection, SqlTransaction transaction, string name, string filename, byte[] fileData)
    {
        string insertQuery = "INSERT INTO Documents (Name, Filename, FileData) VALUES (@Name, @Filename, @FileData)";

        using (SqlCommand command = new SqlCommand(insertQuery, connection, transaction))
        {
            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@Filename", filename);
            command.Parameters.AddWithValue("@FileData", fileData);

            command.ExecuteNonQuery();
        }
    }

    static byte[] GetDocumentBytes()
    {
        return new byte[] {  };
    }
    static void SaveDocumentToFile(int documentId, string outputPath)
    {
        string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=DocumentDatabase;Trusted_Connection=True"; 

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string selectQuery = "SELECT FileData FROM Documents WHERE Id = @Id";

            using (SqlCommand command = new SqlCommand(selectQuery, connection))
            {
                command.Parameters.AddWithValue("@Id", documentId);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        byte[] fileData = (byte[])reader["FileData"];
                        File.WriteAllBytes(outputPath, fileData);
                        Console.WriteLine($"Document saved to {outputPath}");
                    }
                    else
                    {
                        Console.WriteLine($"Document with Id {documentId} not found.");
                    }
                }
            }

            connection.Close();
        }
    }
}