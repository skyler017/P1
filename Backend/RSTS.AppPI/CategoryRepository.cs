using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.Metadata.Ecma335;

// CRUD: Create, Retrieve, Update, Delete Operations.
namespace RSTS.AppPI
{
    public class CategoryRepository
    {
        public List<Category> GetAll(string connString)
        {
            var categories = new List<Category>();
            using (SqlConnection connection = new SqlConnection(connString))
            {
                StringBuilder qry = new StringBuilder();
                qry.Append(" SELECT CategoryID, CategoryName, Description FROM Categories");

                SqlCommand cmd = new SqlCommand(qry.ToString(), connection);
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Category category = new Category();
                    category.Categoryid = reader.GetInt32(0);
                    category.CategoryName = reader["CategoryName"].ToString();
                    category.Description = reader["Description"].ToString();

                    categories.Add(category);
                }
                reader.Close();
                cmd.Dispose();
            }
            return categories;
        }

        public Category Get(string connString, int id)
        {
            var category = new Category();
            using (SqlConnection connection = new SqlConnection(connString))
            {
                StringBuilder qry = new StringBuilder();
                qry.Append(" SELECT CategoryID, CategoryName, Description FROM Categories");
                qry.Append($" WHERE CategoryID = {id}");

                SqlCommand cmd = new SqlCommand(qry.ToString(), connection);
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    category.Categoryid = reader.GetInt32(0);
                    category.CategoryName = reader["CategoryName"].ToString();
                    category.Description = reader["Description"].ToString();
                    break;
                }
                reader.Close();
                cmd.Dispose();
            }
            return category;
        }

        public Category Create(string connString, Category category)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                StringBuilder qry = new StringBuilder();
                qry.Append(" INSERT INTO Categories");
                qry.Append(" (CategoryName, Description)");
                qry.Append($" VALUES (");
                qry.Append($" '{category.CategoryName}',");
                qry.Append($" '{category.Description}');");
                qry.Append($" SELECT @@IDENTITY;");
                Console.WriteLine(qry.ToString());

                SqlCommand cmd = new SqlCommand(qry.ToString(), connection);
                connection.Open();
                int newId = Convert.ToInt32(cmd.ExecuteScalar());
                category.Categoryid = newId;
                cmd.Dispose();
            }
            return category;
        }

        public void Update(string connString, int id, Category category)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                StringBuilder qry = new StringBuilder();
                qry.Append(" UPDATE Categories");
                qry.Append($" SET CategoryName = '{category.CategoryName}'");
                qry.Append($" , Description = '{category.Description}'");
                qry.Append($" WHERE CategoryId = {id}");
                Console.WriteLine(qry.ToString());

                SqlCommand cmd = new SqlCommand(qry.ToString(), connection);
                connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            return;
        }

        public void Delete(string connString, int id)
        {
            using (SqlConnection connection = new SqlConnection(connString))
            {
                StringBuilder qry = new StringBuilder();
                qry.Append(" DELETE FROM Categories");
                qry.Append($" WHERE CategoryId = {id}");
                Console.WriteLine(qry.ToString());

                SqlCommand cmd = new SqlCommand(qry.ToString(), connection);
                connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            return;
        }
    }
}
