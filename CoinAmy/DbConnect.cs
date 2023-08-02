using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace CoinAmy
{
    internal class DbConnect
    {
        private MySqlConnection con;

        public DbConnect(string host, string dbname, string ui, string pw)
        {
            con = new MySqlConnection($"Data Source = {host}; Database = {dbname}; User Id = {ui}; Password = {pw};");
        }

        //A függvény hívása esetén megnyílik az adatbáziskapcsolat.
        private bool Connect()
        {
            try
            {
                con.Open();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //A függvény hívása esetén bezáródik az adatbáziskapcsolat.
        private bool Connect_Close()
        {
            try
            {
                con.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        
        //***Az SQL Injection elleni védekezés jegyében minden lekérdezéshez paraméterként kerülnek rögzítésre az adatok.***

        //A felhasználó által a beviteli mezőben rögzítésre kerülnek az adatbázisban. 
        public void RegisterUser(UserDataModel UDmodel)
        {
            if (Connect())
            {
                string query = "INSERT INTO users(username, email, password) VALUES(@username, @email, @password)";

                MySqlCommand cmd = new MySqlCommand(query, con);

                cmd.Parameters.AddWithValue("@username", UDmodel.username);
                cmd.Parameters.AddWithValue("@email", UDmodel.email);
                cmd.Parameters.AddWithValue ("@password", UDmodel.password);

                cmd.ExecuteNonQuery();

                Connect_Close();
            }
        }

        //Vizsgálatra kerül, hogy hány darab megadott felhasználónév létezik az adatbázisban. Amennyiben egy darab, tehát létezik,
        //igaz értékkel tér vissza a függvény, ellenkező esetben hamis értéket ad vissza.
        public bool IsUsernameExist(string username)
        {
            int count = 0;

            if (Connect())
            {
                string query = "SELECT COUNT(username) FROM users WHERE username LIKE @username";

                MySqlCommand cmd = new MySqlCommand(query, con);

                cmd.Parameters.AddWithValue("@username", username);

                if (cmd.ExecuteScalar() != null && cmd.ExecuteScalar().GetType() != typeof(DBNull))
                {
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                }

                Connect_Close();

                if (count == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        //Vizsgálatra kerül, hogy hány darab megadott e-mail cím létezik az adatbázisban. Amennyiben egy darab, tehát létezik,
        //igaz értékkel tér vissza a függvény, ellenkező esetben hamis értéket ad vissza.
        public bool IsEmailExist(string email)
        {
            int count = 0;

            if (Connect())
            {
                string query = "SELECT COUNT(email) FROM users WHERE email LIKE @email";

                MySqlCommand cmd = new MySqlCommand(query, con);

                cmd.Parameters.AddWithValue("@email", email);

                if (cmd.ExecuteScalar() != null && cmd.ExecuteScalar().GetType() != typeof(DBNull))
                {
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                }

                Connect_Close();

                if (count == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        //Megszámlálásra kerül, hogy létezik-e a megadott felhasználónév/e-mail cím és jelszó kombináció a relációban. Amennyiben egy darab, tehát létezik,
        //igaz értékkel tér vissza a függvény, ellenkező esetben hamis értéket ad vissza.
        public bool LoginUser(UserDataModel UDmodel)
        {
            if (Connect())
            {
                string query = "SELECT COUNT(*) FROM users WHERE (username LIKE @username OR email LIKE @email) AND password LIKE @password";
                
                int count = 0;
                
                MySqlCommand cmd = new MySqlCommand(query, con);

                cmd.Parameters.AddWithValue("@username", UDmodel.username);
                cmd.Parameters.AddWithValue("@email", UDmodel.email);
                cmd.Parameters.AddWithValue("@password", UDmodel.password);

                if (cmd.ExecuteScalar() != null && cmd.ExecuteScalar().GetType() != typeof(DBNull))
                {
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                }

                Connect_Close();

                if (count == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        //A bejelentkezés során megadott adatok alapján vizsgálatra kerül, hogy az adott felhasználónévhez/e-mail címel rendelkező felhasználói fiók
        //melyik user_ID-hoz tartozik a relációban.
        public string GetUserId(string usernameOrEmail)
        {
            string userId = string.Empty;

            if (Connect())
            {
                string query = "SELECT user_ID FROM users WHERE username LIKE @username OR email LIKE @email";

                MySqlCommand cmd = new MySqlCommand(query, con);

                cmd.Parameters.AddWithValue("@username", usernameOrEmail);
                cmd.Parameters.AddWithValue("@email", usernameOrEmail);

                if (cmd.ExecuteScalar() != null && cmd.ExecuteScalar().GetType() != typeof(DBNull))
                {
                    userId = Convert.ToString(cmd.ExecuteScalar());
                }
            }

            Connect_Close();

            return userId;
        }

        //A user_ID alapján lekérdezésre kerül, hogy az adott azonosítóhoz melyik felhasználónév tartozik.
        public string GetUsername()
        {
            string username = string.Empty;

            if (Connect())
            {
                string query = "SELECT username FROM users WHERE users.user_ID = @userID";

                MySqlCommand cmd = new MySqlCommand(query, con);

                cmd.Parameters.AddWithValue("@userID", Login.UserID);

                if (cmd.ExecuteScalar() != null && cmd.ExecuteScalar().GetType() != typeof(DBNull))
                {
                    username = Convert.ToString(cmd.ExecuteScalar());
                }
            }

            Connect_Close();

            return username;
        }

        //Megvizsgálásra kerül, hogy az adott felhasználói azonosítóhoz (user_ID) milyen jelszó tartozik.
        public string IsPasswordMatches()
        {
            string password = string.Empty;

            if (Connect())
            {
                string query = "SELECT password FROM users WHERE user_ID = @userID";

                MySqlCommand cmd = new MySqlCommand(query, con);

                cmd.Parameters.AddWithValue("@userID", Login.UserID);

                if (cmd.ExecuteScalar() != null && cmd.ExecuteScalar().GetType() != typeof(DBNull))
                {
                    password = Convert.ToString(cmd.ExecuteScalar());
                }
            }

            Connect_Close();

            return password;
        }

        //Megszámolásra kerül, hogy van-e olyan felhasználói fiók, ahol a jelszó és a hozzá tartozó user_ID megegyezik.
        //Amennyiben igen, az adott felhasználói fiók user_ID-ja alapján megváltoztatásra kerül a jelszó.
        public void ChangePassword(string oldPassword, string newPassword)
        {
            int count = 0;

            if (Connect())
            {
                string query = "SELECT COUNT(*) FROM users WHERE password LIKE @password AND user_ID = @userID";

                MySqlCommand cmd = new MySqlCommand(query, con);

                cmd.Parameters.AddWithValue("@password", oldPassword);
                cmd.Parameters.AddWithValue("@userID", Login.UserID);

                if (cmd.ExecuteScalar() != null && cmd.ExecuteScalar().GetType() != typeof(DBNull))
                {
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                }

                if (count == 1)
                {
                    string updateQuery = "UPDATE users SET password = @password WHERE user_ID = @userID";

                    MySqlCommand updateCmd = new MySqlCommand(updateQuery, con);

                    updateCmd.Parameters.AddWithValue("@password", newPassword);
                    updateCmd.Parameters.AddWithValue("@userID", Login.UserID);

                    updateCmd.ExecuteNonQuery();
                }

                Connect_Close();
            }
        }

        //Visszaadja a user_ID-hoz tartozó jutalompontok számát.
        public double GetRewardPoints()
        {
            double rewardPoints = 0;

            if (Connect())
            {
                string query = "SELECT rewardPoints FROM users WHERE user_ID = @userID";

                MySqlCommand cmd = new MySqlCommand(query, con);

                cmd.Parameters.AddWithValue("@userID", Login.UserID);

                if (cmd.ExecuteScalar() != null && cmd.ExecuteScalar().GetType() != typeof(DBNull))
                {
                    rewardPoints = Convert.ToDouble(cmd.ExecuteScalar());
                }

                Connect_Close();
            }

            return rewardPoints;
        }

        //Egy befektetés után hozzáadja a jutalompontokat a felhasználó meglévő pontjaihoz.
        public void UpdateRewardPoints(double rewardPoints)
        {
            if (Connect())
            {
                string query = "UPDATE users SET rewardPoints = rewardPoints + @rewardPoints WHERE user_ID = @userID";

                MySqlCommand cmd = new MySqlCommand(query, con);
                
                cmd.Parameters.AddWithValue("@rewardPoints", rewardPoints);
                cmd.Parameters.AddWithValue("@userID", Login.UserID);

                cmd.ExecuteNonQuery();

                Connect_Close();
            }
        }

        //Az adott felhasználói fiókhoz tartozó user_ID alapján beilleszésre kerül a relációba a megadott befektetés.
        public void InsertInvestmentData(AddUserInvestmentModel AUImodel)
        {
            if (Connect())
            {
                string query = "INSERT INTO investments(user_ID, ticker, name, moneyInvested, priceOnBought, dateOnBought) VALUES(@user_ID, @ticker, @name, @moneyInvested, @priceOnBought, @dateOnBought)";

                MySqlCommand cmd = new MySqlCommand(query, con);

                cmd.Parameters.AddWithValue("@user_ID", Login.UserID);
                cmd.Parameters.AddWithValue("@ticker", AUImodel.ticker);
                cmd.Parameters.AddWithValue("@name", AUImodel.name);
                cmd.Parameters.AddWithValue("@moneyInvested", AUImodel.moneyInvested);
                cmd.Parameters.AddWithValue("@priceOnBought", AUImodel.priceOnBought);
                cmd.Parameters.AddWithValue("@dateOnBought", AUImodel.dateOnBought);

                cmd.ExecuteNonQuery();

                Connect_Close();
            }
        }

        //Törli a bejelentkezett felhaszálóhoz tartozó befektetések közül, user_ID alapján, a ListView vezérlő SelectedIndex helyén találhatót.
        //Megkeresi a user_ID-hoz tartozó befektetéseket, majd a választott indexen levőt adja a DELETE utasításnak.
        public void DeleteInvestmentData(int selectedIndex)
        {
            if (Connect())
            {
                string query = "DELETE FROM investments WHERE investment_ID = (SELECT investment_ID FROM investments WHERE user_ID = @userID ORDER BY investment_ID LIMIT @selectedIndex, 1)";

                MySqlCommand cmd = new MySqlCommand(query, con);

                cmd.Parameters.AddWithValue("@userID", Login.UserID);
                cmd.Parameters.AddWithValue("@selectedIndex", selectedIndex);

                cmd.ExecuteNonQuery();

                Connect_Close();
            }
        }

        //A befektetés szerkesztéséhez betölti az ablakba a szerkesztendő befeketés meglévő adatait.
        public List<AddUserInvestmentModel> LoadDataToEditInvestment(int selectedIndex)
        {
            List<AddUserInvestmentModel> selectedInvestment = new List<AddUserInvestmentModel>();

            if (Connect())
            {
                string query = "SELECT ticker, name, moneyInvested, priceOnBought, dateOnBought FROM investments INNER JOIN users ON investments.user_ID = users.user_ID WHERE investments.investment_ID = (SELECT investment_ID FROM investments WHERE user_ID = @userID ORDER BY investment_ID LIMIT @selectedIndex, 1);";
            
                MySqlCommand cmd = new MySqlCommand(query, con);

                cmd.Parameters.AddWithValue("@userID", Login.UserID);
                cmd.Parameters.AddWithValue("@selectedIndex", selectedIndex);

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    selectedInvestment.Add(new AddUserInvestmentModel(reader.GetString(0), reader.GetString(1), reader.GetDouble(2), reader.GetDouble(3), reader.GetDateTime(4)));
                }

                Connect_Close();
            }

            return selectedInvestment;
        }

        //Frissíti a a felhasználó befektetési adatait a ListView-ból választott indexen.
        public void EditInvestmentData(AddUserInvestmentModel AUImodel, int selectedIndex)
        {
            if (Connect())
            {
                string query = "UPDATE investments SET ticker = @ticker, name = @name, moneyInvested = @moneyInvested, priceOnBought = @priceOnBought, dateOnBought = @dateOnBought WHERE investments.investment_ID = (SELECT investment_ID FROM investments WHERE user_ID = @userID ORDER BY investment_ID LIMIT @selectedIndex, 1);";

                MySqlCommand cmd = new MySqlCommand(query, con);

                cmd.Parameters.AddWithValue("@ticker", AUImodel.ticker);
                cmd.Parameters.AddWithValue("@name", AUImodel.name);
                cmd.Parameters.AddWithValue("@moneyInvested", AUImodel.moneyInvested);
                cmd.Parameters.AddWithValue("@priceOnBought", AUImodel.priceOnBought);
                cmd.Parameters.AddWithValue("@dateOnBought", AUImodel.dateOnBought);
                cmd.Parameters.AddWithValue("@userID", Login.UserID);
                cmd.Parameters.AddWithValue("@selectedIndex", selectedIndex);

                cmd.ExecuteNonQuery();
            }

            Connect_Close();
        }

        //A user_ID szerint szűrésre kerülnek a befektetések, tehát a lekérdezés az adott felhasználó befektetéseit adja vissza.
        public List<AddUserInvestmentModel> GetInvestmentDatas()
        {
            List<AddUserInvestmentModel> userInvestments = new List<AddUserInvestmentModel>();

            if (Connect())
            {
                string query = "SELECT ticker, name, moneyInvested, priceOnBought, moneyInvested / priceOnBought AS amountBought, dateOnBought FROM investments INNER JOIN users ON users.user_ID = investments.user_ID WHERE users.user_ID = @userID";

                MySqlCommand cmd = new MySqlCommand(query, con);

                cmd.Parameters.AddWithValue("@userID", Login.UserID);

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    userInvestments.Add(new AddUserInvestmentModel(reader.GetString(0), reader.GetString(1), reader.GetDouble(2), reader.GetDouble(3), reader.GetDouble(4), reader.GetDateTime(5)));
                }

                Connect_Close();
            }

            return userInvestments;
        }

        //A user_ID szerint szűrésre kerülnek a befektetések, tehát a lekérdezés az adott felhasználó befektetéseit adja vissza a PDF generáláshoz szükséges típusként.
        public PdfPTable LoadDataToExport()
        {
            PdfPTable investmentsTable = new PdfPTable(6);

            if (Connect())
            {
                string query = "SELECT ticker, name, moneyInvested, priceOnBought, moneyInvested / priceOnBought AS amountBought, dateOnBought FROM investments INNER JOIN users ON users.user_ID = investments.user_ID WHERE users.user_ID = @userID";

                MySqlCommand cmd = new MySqlCommand(query, con);

                cmd.Parameters.AddWithValue("@userID", Login.UserID);

                MySqlDataReader reader = cmd.ExecuteReader();

                investmentsTable = new PdfPTable(reader.FieldCount);

                investmentsTable.AddCell(new PdfPCell(new Phrase("Ticker")));
                investmentsTable.AddCell(new PdfPCell(new Phrase("Megnevezés")));
                investmentsTable.AddCell(new PdfPCell(new Phrase("Befektetett összeg")));
                investmentsTable.AddCell(new PdfPCell(new Phrase("Vásárlás árfolyama")));
                investmentsTable.AddCell(new PdfPCell(new Phrase("Vásárolt mennyiség")));
                investmentsTable.AddCell(new PdfPCell(new Phrase("Vásárlás dátuma")));

                while (reader.Read())
                {
                    investmentsTable.AddCell(reader.GetString(0));
                    investmentsTable.AddCell(reader.GetString(1));
                    investmentsTable.AddCell($"{reader.GetDouble(2)} Ft");
                    investmentsTable.AddCell($"{reader.GetDouble(3)} Ft");
                    investmentsTable.AddCell($"{reader.GetDouble(4)} {reader.GetString(0)}");
                    investmentsTable.AddCell(reader.GetDateTime(5).ToString());
                }

                Connect_Close();
            }

            return investmentsTable;
        }

        //Megvizsgája, hogy a felhasználóhoz tartoznak-e befektetések. Ezzel elkerülhető, hogy a PDF exportálás során üres dokumentum kerüljön elkészítésre.
        public bool HasUserHaveInvestments()
        {
            int count = 0;

            if (Connect())
            {
                string query = "SELECT COUNT(*) FROM investments INNER JOIN users ON users.user_ID = investments.user_ID WHERE users.user_ID = @userID";

                MySqlCommand cmd = new MySqlCommand(query, con);

                cmd.Parameters.AddWithValue("@userID", Login.UserID);

                if (cmd.ExecuteScalar() != null && cmd.ExecuteScalar().GetType() != typeof(DBNull))
                {
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                }

                Connect_Close();

                if (count == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        //Megadja az adott felhasználóhoz tartozó befektetetett FIAT valuták (Ft) összegét user_ID-ra szűrve.
        public double SumOfInvestments()
        {
            double summary = 0;

            if (Connect())
            {
                string query = "SELECT SUM(moneyInvested) FROM investments INNER JOIN users ON investments.user_ID = users.user_ID WHERE users.user_ID = @userID;";

                MySqlCommand cmd = new MySqlCommand(query, con);

                cmd.Parameters.AddWithValue("@userID", Login.UserID);

                if (cmd.ExecuteScalar() != null && cmd.ExecuteScalar().GetType() != typeof(DBNull))
                {
                    summary = Convert.ToDouble(cmd.ExecuteScalar());
                }

                Connect_Close();
            }

            return summary;
        }

        //Megadja az adott felhasználóhoz tartozó befektetetett FIAT valuták (Ft) átlagát user_ID-ra szűrve.
        public double AverageOfInvestments()
        {
            double average = 0;

            if (Connect())
            {
                string query = "SELECT AVG(moneyInvested) FROM investments INNER JOIN users ON investments.user_ID = users.user_ID WHERE users.user_ID = @userID;";

                MySqlCommand cmd = new MySqlCommand(query, con);

                cmd.Parameters.AddWithValue("@userID", Login.UserID);

                if (cmd.ExecuteScalar() != null && cmd.ExecuteScalar().GetType() != typeof(DBNull))
                {
                    average = Convert.ToDouble(cmd.ExecuteScalar());
                }

                Connect_Close();
            }

            return average;
        }

        public string LastInvestment()
        {
            string currencyName = string.Empty;

            if (Connect())
            {
                string query = "SELECT name FROM investments WHERE user_ID = @userID ORDER BY dateOnBought DESC LIMIT 1";

                MySqlCommand cmd = new MySqlCommand(query, con);

                cmd.Parameters.AddWithValue("@userID", Login.UserID);

                if (cmd.ExecuteScalar() != null && cmd.ExecuteScalar().GetType() != typeof(DBNull))
                {
                    currencyName = cmd.ExecuteScalar().ToString();
                }

                Connect_Close();
            }

            return currencyName;
        }
    }
}
