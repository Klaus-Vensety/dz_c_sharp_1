using System;
using Microsoft.Data.Sqlite;

class Program
{
    static string dbName = "Students.db";

    static void Main(string[] args)
    {
        CreateTable();

        if (args.Length == 0)
        {
            Console.WriteLine("Ошибка: нет аргументов");
            return;
        }

        string op = args[0];

        try
        {
            switch (op)
            {
                case "-add":
                    if (args.Length != 4)
                    {
                        Console.WriteLine("Использование: -add Фамилия Имя Группа");
                        return;
                    }
                    Add(args[1], args[2], args[3]);
                    break;

                case "-delete_name":
                    CheckArgs(args, 2);
                    Delete("Name", args[1]);
                    break;

                case "-delete_surname":
                    CheckArgs(args, 2);
                    Delete("Surname", args[1]);
                    break;

                case "-modify_name":
                    CheckArgs(args, 3);
                    Modify("Name", args[1], args[2]);
                    break;

                case "-modify_surname":
                    CheckArgs(args, 3);
                    Modify("Surname", args[1], args[2]);
                    break;

                case "-show_all":
                    ShowAll();
                    break;

                case "-show_by_name":
                    CheckArgs(args, 2);
                    Show("Name", args[1]);
                    break;

                case "-show_by_surname":
                    CheckArgs(args, 2);
                    Show("Surname", args[1]);
                    break;

                default:
                    Console.WriteLine("Неизвестная команда");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка: " + ex.Message);
        }
    }

    static void CheckArgs(string[] args, int count)
    {
        if (args.Length < count)
            throw new Exception("Недостаточно аргументов");
    }

    static void CreateTable()
    {
        using var conn = new SqliteConnection($"Data Source={dbName}");
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText =
        @"CREATE TABLE IF NOT EXISTS Students(
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Surname TEXT,
            Name TEXT,
            GroupName TEXT
        );";

        cmd.ExecuteNonQuery();
    }

    static void Add(string surname, string name, string group)
    {
        using var conn = new SqliteConnection($"Data Source={dbName}");
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText =
        @"INSERT INTO Students (Surname, Name, GroupName)
          VALUES ($s, $n, $g);";

        cmd.Parameters.AddWithValue("$s", surname);
        cmd.Parameters.AddWithValue("$n", name);
        cmd.Parameters.AddWithValue("$g", group);

        cmd.ExecuteNonQuery();
        Console.WriteLine("Добавлено");
    }

    static void Delete(string field, string value)
    {
        using var conn = new SqliteConnection($"Data Source={dbName}");
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = $"DELETE FROM Students WHERE {field}=$v";
        cmd.Parameters.AddWithValue("$v", value);

        int rows = cmd.ExecuteNonQuery();
        Console.WriteLine($"Удалено записей: {rows}");
    }

    static void Modify(string field, string oldValue, string newValue)
    {
        using var conn = new SqliteConnection($"Data Source={dbName}");
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText =
        $"UPDATE Students SET {field}=$new WHERE {field}=$old";

        cmd.Parameters.AddWithValue("$old", oldValue);
        cmd.Parameters.AddWithValue("$new", newValue);

        int rows = cmd.ExecuteNonQuery();
        Console.WriteLine($"Изменено записей: {rows}");
    }

    static void Show(string field, string value)
    {
        using var conn = new SqliteConnection($"Data Source={dbName}");
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = $"SELECT * FROM Students WHERE {field}=$v";
        cmd.Parameters.AddWithValue("$v", value);

        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            Print(reader);
        }
    }

    static void ShowAll()
    {
        using var conn = new SqliteConnection($"Data Source={dbName}");
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT * FROM Students";

        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            Print(reader);
        }
    }

    static void Print(SqliteDataReader r)
    {
        Console.WriteLine($"{r["Id"]}: {r["Surname"]} {r["Name"]} {r["GroupName"]}");
    }
}