using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

class Program
{
    public class InventoryDatabase
    {
        private string connectionString = "Data Source=inventory.db;Version=3;";

        public InventoryDatabase()
        {
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string createTableQuery = "CREATE TABLE IF NOT EXISTS Inventory (ID INTEGER PRIMARY KEY, Name TEXT, Quantity INTEGER)";
                    SQLiteCommand createTableCommand = new SQLiteCommand(createTableQuery, connection);
                    createTableCommand.ExecuteNonQuery();
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine("Database error: " + ex.Message);
                Environment.Exit(1);
            }
        }

        public bool AddItem(string name, int quantity)
        {
            if (string.IsNullOrWhiteSpace(name) || quantity <= 0)
            {
                Console.WriteLine("Invalid input. Name should not be empty, and quantity should be greater than 0.");
                return false;
            }
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string insertQuery = "INSERT INTO Inventory (Name, Quantity) VALUES (@Name, @Quantity)";
                    SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, connection);
                    insertCommand.Parameters.AddWithValue("@Name", name);
                    insertCommand.Parameters.AddWithValue("@Quantity", quantity);
                    insertCommand.ExecuteNonQuery();
                }
                return true;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine("Database error: " + ex.Message);
                return false;
            }
        }

        public bool UpdateItem(int id, string name, int quantity)
        {
            if (id <= 0)
            {
                Console.WriteLine("Invalid ID. ID should be greater than 0.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(name) || quantity <= 0)
            {
                Console.WriteLine("Invalid input. Name should not be empty, and quantity should be greater than 0.");
                return false;
            }

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string updateQuery = "UPDATE Inventory SET Name = @Name, Quantity = @Quantity WHERE ID = @ID";
                    SQLiteCommand updateCommand = new SQLiteCommand(updateQuery, connection);
                    updateCommand.Parameters.AddWithValue("@Name", name);
                    updateCommand.Parameters.AddWithValue("@Quantity", quantity);
                    updateCommand.Parameters.AddWithValue("@ID", id);
                    int rowsAffected = updateCommand.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        Console.WriteLine("Item with the specified ID not found.");
                        return false;
                    }
                }
                return true;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine("Database error: " + ex.Message);
                return false;
            }
        }       
        public bool DeleteItem(int id)
        {
            if (id <= 0)
            {
                Console.WriteLine("Invalid ID. ID should be greater than 0.");
                return false;
            }

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string deleteQuery = "DELETE FROM Inventory WHERE ID = @ID";
                    SQLiteCommand deleteCommand = new SQLiteCommand(deleteQuery, connection);
                    deleteCommand.Parameters.AddWithValue("@ID", id);
                    int rowsAffected = deleteCommand.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        Console.WriteLine("Item with the specified ID not found.");
                        return false;
                    }
                }
                return true;
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine("Database error: " + ex.Message);
                return false;
            }
        }
       
        public List<InventoryItem> ReadItems()
        {
            List<InventoryItem> items = new List<InventoryItem>();

            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string selectQuery = "SELECT ID, Name, Quantity FROM Inventory";
                    SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, connection);
                    SQLiteDataReader reader = selectCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string name = reader.GetString(1);
                        int quantity = reader.GetInt32(2);
                        items.Add(new InventoryItem(id, name, quantity));
                    }
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine("Database error: " + ex.Message);
            }

            return items;
        }
    }
    public class InventoryItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }

        public InventoryItem(int id, string name, int quantity)
        {
            ID = id;
            Name = name;
            Quantity = quantity;
        }
    }
    static void Main(string[] args)
    {
        InventoryDatabase database = new InventoryDatabase();

        while (true)
        {
            Console.WriteLine("Inventory Management System");
            Console.WriteLine("1. Add Item");
            Console.WriteLine("2. Update Item");
            Console.WriteLine("3. Delete Item");
            Console.WriteLine("4. View Items");
            Console.WriteLine("5. Exit");
            Console.Write("Enter your choice: ");

            int choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    Console.Write("Enter item name: ");
                    string itemName = Console.ReadLine();
                    Console.Write("Enter item quantity: ");
                    int itemQuantity = int.Parse(Console.ReadLine());
                    database.AddItem(itemName, itemQuantity);
                    Console.WriteLine("Item added successfully.");
                    break;

                case 2:
                    Console.Write("Enter item ID to update: ");
                    int updateId = int.Parse(Console.ReadLine());
                    Console.Write("Enter new item name: ");
                    string updateName = Console.ReadLine();
                    Console.Write("Enter new item quantity: ");
                    int updateQuantity = int.Parse(Console.ReadLine());
                    database.UpdateItem(updateId, updateName, updateQuantity);
                    Console.WriteLine("Item updated successfully.");
                    break;

                case 3:
                    Console.Write("Enter item ID to delete: ");
                    int deleteId = int.Parse(Console.ReadLine());
                    database.DeleteItem(deleteId);
                    Console.WriteLine("Item deleted successfully.");
                    break;

                case 4:
                    List<InventoryItem> items = database.ReadItems();
                    Console.WriteLine("Inventory Items:");
                    foreach (var item in items)
                    {
                        Console.WriteLine($"ID: {item.ID}, Name: {item.Name}, Quantity: {item.Quantity}");
                    }
                    break;

                case 5:
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    break;
            }
        }
    }
}
