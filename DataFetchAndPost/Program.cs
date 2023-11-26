using Dapper;
using DataFetchAndPost.Model;
using System.Configuration;
using System.Data.SqlClient;

public class Program
{
    static void Main()
    {
        string sourceConnectionString = ConfigurationManager.ConnectionStrings["FetchConnection"].ConnectionString;
        string destinationConnectionString = ConfigurationManager.ConnectionStrings["PostConnection"].ConnectionString;

        try
        {
            using (var sourceConnection = new SqlConnection(sourceConnectionString))
            {
                sourceConnection.Open();

                var data = sourceConnection.Query<UserDetails>("select distinct a.UserName,a.email as Email,b.email, b.attendeesName as Name, b.Phone, b.Address,b.DepartmentId,c.DepartmentName\r\nfrom aspnetusers a\r\nleft join Attendees_Setup b on a.email = b.email\r\nleft join Department_Setup c on c.DepartmentId = b.DepartmentId\r\nOrder by DepartmentId").ToList();

                using (var destinationConnection = new SqlConnection(destinationConnectionString))
                {
                    destinationConnection.Open();
                    foreach( var item in data )
                    {
                        string insertUserQuery = "INSERT INTO Auth.tblUser (UserName, Password,Name, Address,ContactNo,Email,Status, CreatedDate,DepartmentId,RoleId)\r\nValues(@UserName,'Dishhome@123',@Name,@Address,@Phone,@Email,1,GetDate(),@DepartmentId, 4)";
                        destinationConnection.Execute(insertUserQuery, item);
                    }
                }
            }
        }
        catch (SqlException ex) 
        {
            Console.WriteLine($"Error accessing the database: {ex.Message}");
        }
    }

}