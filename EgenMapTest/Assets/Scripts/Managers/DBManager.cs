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

    #region Const Fields
    private const string DATABASE_NAME = "/TECHRUNDB.db";
    private const string PROFILE_TABLE_NAME = "PROFILE";
    #endregion

    #region Fields
    private IDbConnection dbcon;
    private IDbCommand command;
    private string connection;
    private string sql;
    #endregion

    /// <summary>
    /// Unity method, runs in the beginning before Start()
    /// </summary>
	private void Awake () 
    {
        OpenDB();
        CreateTables();
	}

    /// <summary>
    /// Opens a connection to the database, and creates the database if it is not already created
    /// </summary>
    private void OpenDB()
    {
        connection = "URI=file:" + Application.persistentDataPath + DATABASE_NAME;

        print(connection);

        dbcon = new SqliteConnection(connection);
        command = dbcon.CreateCommand(); // Used to execute commands to the database.

        dbcon.Open();
    }

    /// <summary>
    /// Creates all the needed tables in the database, if they don't already exist.
    /// </summary>
    private void CreateTables()
    {
        sql = "CREATE TABLE IF NOT EXISTS " + PROFILE_TABLE_NAME + " (id INTEGER PRIMARY KEY, username TEXT, password TEXT)";
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Inserts a new profile into the database
    /// </summary>
    /// <param name="_username">Username for the profile</param>
    /// <param name="_password">Password for the profile</param>
    public void InsertNewUser(string _username, string _password)
    {
        sql = "INSERT INTO " + PROFILE_TABLE_NAME + " (username, password) VALUES ('" + _username + "', '" + _password + "')";
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Checks to see if the username is already in use.
    /// </summary>
    /// <param name="_username">Username to check</param>
    /// <returns>True if the username is already in use, false if it isn't</returns>
    public bool ExistingUser(string _username)
    {
        sql = "SELECT * FROM " + PROFILE_TABLE_NAME + " WHERE username = '" + _username + "'";
        command.CommandText = sql;
        IDataReader reader = command.ExecuteReader();

        string usernameText = reader["username"].ToString();

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

    /// <summary>
    /// Checks if the logon information given matches a profile in the database
    /// </summary>
    /// <param name="_username">Username to check</param>
    /// <param name="_password">Password to check</param>
    /// <returns>True if match, false if not</returns>
    public bool LoginToExistingUser(string _username, string _password)
    {
        sql = "SELECT * FROM " + PROFILE_TABLE_NAME + " WHERE username = '" + _username + "'";
        command.CommandText = sql;
        IDataReader reader = command.ExecuteReader();

        string usernameText = reader["username"].ToString();

        if (usernameText == _username) // Is there a user with the given username?
        {   
            if (_password == reader["password"].ToString()) // Is the password correct for the given profile?
            {
                reader.Close();
                return true;
            }
        }

        reader.Close();
        return false;
    }
}
