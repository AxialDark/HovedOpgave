using UnityEngine;
using System.Collections;
using System.Data;
using Mono.Data.Sqlite;

public class DBManager : MonoBehaviour
{

    #region Singleton
    private static DBManager instance;

    public static DBManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("DBManager").AddComponent<DBManager>();
            }

            return instance;
        }
    }
    #endregion

    private const string DATABASE_NAME = "/TECHRUNDB.db";
    private const string PROFILE_TABLE_NAME = "PROFILE";

    private IDbConnection dbcon;
    private IDbCommand command;
    private string connection;
    private string sql;

    // Use this for initialization
	void Awake () 
    {
        OpenDB();
        CreateTables();
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    private void OpenDB()
    {
        connection = "URI=file:" + Application.persistentDataPath + DATABASE_NAME;

        print(connection);

        dbcon = new SqliteConnection(connection);
        command = dbcon.CreateCommand();

        dbcon.Open();
    }

    private void CreateTables()
    {
        sql = "CREATE TABLE IF NOT EXISTS " + PROFILE_TABLE_NAME + " (id INTEGER PRIMARY KEY, username TEXT, password TEXT)";
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    public void InsertNewUser(string _username, string _password)
    {
        sql = "INSERT INTO " + PROFILE_TABLE_NAME + " (username, password) VALUES ('" + _username + "', '" + _password + "')";
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    public bool ExistingUser(string _username)
    {
        sql = "SELECT * FROM " + PROFILE_TABLE_NAME + " WHERE username = '" + _username + "'";
        command.CommandText = sql;
        IDataReader reader = command.ExecuteReader();

        string usernameText = "";

        //while (reader.Read())
        //{
        //    usernameText += reader["username"];
        //}

        usernameText += reader["username"];

        if (usernameText == _username)
        {
            reader.Close();
            return true;
        }
        else
        {
            reader.Close();
            return false;
        }
    }

    public bool LoginToExistingUser(string _username, string _password)
    {
        sql = "SELECT * FROM " + PROFILE_TABLE_NAME + " WHERE username = '" + _username + "'";
        command.CommandText = sql;
        IDataReader reader = command.ExecuteReader();

        string usernameText = reader["username"].ToString();
        print("From DB: " + usernameText);
        print("From input: " + _username);

        if (usernameText == _username)
        {
            print("Correct username");

            print("From Input: " + _password);
            print("From DB: " + reader["password"]);
            
            if (_password == reader["password"].ToString())
            {
                print("Correct password");
                reader.Close();
                return true;
            }
        }

        reader.Close();
        return false;

    }
}
