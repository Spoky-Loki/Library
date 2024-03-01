using Npgsql;
using System.Web.Configuration;

namespace Library.Data
{
    public class DbContext
    {
        private static DbContext _context;

        private NpgsqlConnection DbConnection;
        private NpgsqlCommand DbCommand;

        private DbContext () 
        {
            DbConnection = new NpgsqlConnection(
                connectionString: WebConfigurationManager.ConnectionStrings["DbContext"].ConnectionString);

            DbCommand = new NpgsqlCommand();
            DbCommand.Connection = DbConnection;
        }

        public static DbContext GetContext () 
        {
            if (_context == null)
                _context = new DbContext ();
            return _context;
        }

        public NpgsqlCommand GetCommand() 
        { 
            return DbCommand; 
        }
    }
}