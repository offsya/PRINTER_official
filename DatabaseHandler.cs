using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using Npgsql;

using TPExpCon;


public class DatabaseHandler
{
    private string serverName;
    private string userName;
    private string password;
    private string databaseName;
    private string table;

    private int column = 1;
    /*private string serverName = "ec2-34-203-114-67.compute-1.amazonaws.com";
    private string userName = "tcfwfhpwedkpzy";
    private string password = "37c77546465bb5696396625bd759467d0a829a4e9dcdddeb6db08ec233808690";
    private string databaseName = "dk2ijj7vth507";
    private string table = "codes";
    private int column = 1;*/

    public void getDatabaseData()
    {
        Console.WriteLine("Введите имя сервера:");
        serverName = Console.ReadLine();
        Console.WriteLine("Введите имя пользователя");
        userName = Console.ReadLine();
        Console.WriteLine("Введите пароль");
        password = Console.ReadLine();
        Console.WriteLine("Введите имя базы данных");
        databaseName = Console.ReadLine();
        Console.WriteLine("Введите имя таблицы, где хранятся коды");
        table = Console.ReadLine();
        do
        {
            Console.WriteLine("Введите номер колонки с кодами (номер первой -> 0)");
            if(Int32.TryParse(Console.ReadLine(), out column)) break;
            else Console.WriteLine("Некорректные данные");
        } while (true);
    }


    public NpgsqlConnection connect()
    {
       getDatabaseData();
       NpgsqlConnection connection = new NpgsqlConnection("Host="+serverName+";Username="+userName+";" +
                                                           "Password="+password+";Database="+databaseName+"");
       try
       {
            connection.Open();
            if (connection.FullState == ConnectionState.Broken || connection.FullState == ConnectionState.Closed)
            {
                throw new Exception("Ошибка подключения к базе данных");
            }
       }
       catch (Exception e)
       {
           throw new Exception("База данных не найдена или введены неправильные данные");
       }
       return connection;
    }

    public IEnumerable<string> getCodes()
    {
        int i = 0;
        var codes = new List<string>();
        while (true)
        {
            try
            {
                NpgsqlConnection obj = connect();
                NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM " + table + ";",obj);
                NpgsqlDataReader reader = command.ExecuteReader();
                if(reader.HasRows)
                {
                    while (reader.Read() && i < Codes.CodesChunkSize)
                    {
                        codes.Add(reader.GetString(column));
                        i++;
                    }
                }
                else
                {
                    Console.WriteLine("Нет кодов в базе данных");
                }
                break;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        return codes;
        
        
    }
    
}