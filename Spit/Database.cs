using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace Spit
{
    class Database
    {
        public SQLiteConnection sqlite_conn;

        const int PICK_UP_PILE = -1;

        Game game;
        public Database(Game game)
        {
            this.game = game;
            sqlite_conn = CreateConnection();

            //DeleteTables(sqlite_conn);

            //CreateTables(sqlite_conn);
        }

        private SQLiteConnection CreateConnection()
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source=database.db");

            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to create a connection to the database.");
            }
            return conn;
        }

        public void CreateTables(SQLiteConnection conn)
        {
            SQLiteCommand cmd = conn.CreateCommand();

            string createHumanPlayerHandTableCmd = "CREATE TABLE HumanPlayerHand(cardNum INT, cardSuit VARCHAR(7), pileNum INT, gameIndex INT)";
            string createAIPlayerHandTableCmd = "CREATE TABLE AIPlayerHand(cardNum INT, cardSuit VARCHAR(7), pileNum INT, gameIndex INT)";
            string createPlacePilesTableCmd = "CREATE TABLE PlacePile(cardNum INT, cardSuit VARCHAR(7), pileNum INT, gameIndex INT)";
            string createAIDifficultyTableCmd = "CREATE TABLE AIDifficulty(difficultyLevel INT, gameIndex INT)";

            cmd.CommandText = createHumanPlayerHandTableCmd;
            cmd.ExecuteNonQuery();
            cmd.CommandText = createAIPlayerHandTableCmd;
            cmd.ExecuteNonQuery();
            cmd.CommandText = createPlacePilesTableCmd;
            cmd.ExecuteNonQuery();
            cmd.CommandText = createAIDifficultyTableCmd;
            cmd.ExecuteNonQuery();
        }
        public void DeleteTables(SQLiteConnection conn)
        {
            SQLiteCommand cmd = conn.CreateCommand();

            string dropHumanPlayerHandTableCmd = "DROP TABLE HumanPlayerHand";
            string dropAIPlayerHandTableCmd = "DROP TABLE AIPlayerHand";
            string dropPlacePilesTableCmd = "DROP TABLE PlacePile";
            string dropAIDifficultyCmd = "DROP TABLE AIDifficulty";

            cmd.CommandText = dropHumanPlayerHandTableCmd;
            cmd.ExecuteNonQuery();
            cmd.CommandText = dropAIPlayerHandTableCmd;
            cmd.ExecuteNonQuery();
            cmd.CommandText = dropPlacePilesTableCmd;
            cmd.ExecuteNonQuery();
            cmd.CommandText = dropAIDifficultyCmd;
            cmd.ExecuteNonQuery();
        }

        private void InsertData(SQLiteConnection conn, int gameIndex)
        {
            SavePlayerHand(conn, gameIndex);
            SaveAI(conn, gameIndex);
            SavePlacePiles(conn, gameIndex);
        }

        private void ReadData(SQLiteConnection conn, int gameIndex)
        {
            CreatePlayers(conn, gameIndex);
            ExtractPlayerHand(conn, gameIndex);
            ExtractAIHand(conn, gameIndex);
            ExtractPlacePiles(conn, gameIndex);            
        }

        public void SaveGameState(int gameIndex)
        {
            InsertData(sqlite_conn, gameIndex);
        }

        public void LoadGameState(int gameIndex)
        {
            ReadData(sqlite_conn, gameIndex);
        }

        private void CreatePlayers(SQLiteConnection conn, int gameIndex)
        {
            SQLiteCommand cmd = conn.CreateCommand();
            SQLiteDataReader datareader;
            cmd.CommandText = "SELECT * FROM AIDifficulty";
            datareader = cmd.ExecuteReader();
            datareader.Read();

            int difficulty = datareader.GetInt32(0);
            game.CreatePlayers(difficulty);
            datareader.Close();
        }

        private void ExtractPlayerHand(SQLiteConnection conn, int gameIndex)
        {
            SQLiteCommand cmd = conn.CreateCommand();
            SQLiteDataReader datareader;

            Hand hand = new Hand();
            cmd.CommandText = "SELECT * FROM HumanPlayerHand";
            datareader = cmd.ExecuteReader();
            while (datareader.Read())
            {
                string cardSuit = datareader.GetString(1);
                int cardNum = datareader.GetInt32(0);
                int pileNum = datareader.GetInt32(2);

                Card card = new Card(cardSuit, cardNum);

                if (pileNum != -1)
                {
                    hand.piles[pileNum].pile.Push(card);
                }
                else
                {
                    hand.pickUpPile.pile.Push(card);
                }
            }
            game.players[Game.HUMAN].hand = hand;

            datareader.Close();
        }
        private void ExtractAIHand(SQLiteConnection conn, int gameIndex)
        {
            SQLiteCommand cmd = conn.CreateCommand();
            SQLiteDataReader datareader;

            Hand hand = new Hand();
            cmd.CommandText = "SELECT * FROM AIPlayerHand";
            datareader = cmd.ExecuteReader();
            while (datareader.Read())
            {
                string cardSuit = datareader.GetString(1);
                int cardNum = datareader.GetInt32(0);
                int pileNum = datareader.GetInt32(2);

                Card card = new Card(cardSuit, cardNum);

                if (pileNum != -1)
                {
                    hand.piles[pileNum].pile.Push(card);
                }
                else
                {
                    hand.pickUpPile.pile.Push(card);
                }
            }
            game.players[Game.AI].hand = hand;
        }
        private void ExtractPlacePiles(SQLiteConnection conn, int gameIndex)
        {
            SQLiteCommand cmd = conn.CreateCommand();
            SQLiteDataReader datareader;

            Pile pile1 = new Pile();
            Pile pile2 = new Pile();

            cmd.CommandText = "SELECT * FROM PlacePile";
            datareader = cmd.ExecuteReader();
            while (datareader.Read())
            {
                string cardSuit = datareader.GetString(1);
                int cardNum = datareader.GetInt32(0);
                int pileNum = datareader.GetInt32(2);

                Card card = new Card(cardSuit, cardNum);

                if(pileNum == 1)
                {
                    pile1.pile.Push(card);
                }
                else if (pileNum == 2)
                {
                    pile2.pile.Push(card);
                }
                game.placePiles[0] = pile1;
                game.placePiles[1] = pile2;
            }
            datareader.Close();
        }

        private void SavePlayerHand(SQLiteConnection conn, int gameIndex)
        {
            SQLiteCommand cmd = conn.CreateCommand();

            //Saves players hand 
            for (int i = 0; i < game.players[Game.HUMAN].hand.piles.Length; i++)
            {
                for (int x = 0; x < game.players[Game.HUMAN].hand.piles[i].pile.Length(); x++)
                {
                    Card data = game.players[Game.HUMAN].hand.piles[i].pile.Pop();

                    string test = "INSERT INTO HumanPlayerHand(cardNum, cardSuit, pileNum, gameIndex) VALUES(" + data.GetNumber() + ", '" + data.GetSuit() + "', " + i + "," + gameIndex + ")";

                    cmd.CommandText = test;
                    cmd.ExecuteNonQuery();
                }
            }
            for (int i = 0; i < game.players[Game.HUMAN].hand.pickUpPile.pile.Length(); i++)
            {
                Card data = game.players[Game.HUMAN].hand.pickUpPile.pile.Pop();

                cmd.CommandText = "INSERT INTO HumanPlayerHand(cardNum, cardSuit, pileNum, gameIndex) VALUES(" + data.GetNumber() + ", '" + data.GetSuit() + "', " + PICK_UP_PILE + "," + gameIndex + ")";
                cmd.ExecuteNonQuery();
            }
        }
        private void SaveAI(SQLiteConnection conn, int gameIndex)
        {
            SQLiteCommand cmd = conn.CreateCommand();

            //Saves AIs hand
            for (int i = 0; i < game.players[Game.AI].hand.piles.Length; i++)
            {
                for (int x = 0; x < game.players[Game.AI].hand.piles[i].pile.Length(); x++)
                {
                    Card data = game.players[Game.AI].hand.piles[i].pile.Pop();

                    cmd.CommandText = "INSERT INTO AIPlayerHand(cardNum, cardSuit, pileNum, gameIndex) VALUES(" + data.GetNumber() + ", '" + data.GetSuit() + "', " + i + "," + gameIndex + ")";
                    cmd.ExecuteNonQuery();
                }
            }
            for (int i = 0; i < game.players[Game.AI].hand.pickUpPile.pile.Length(); i++)
            {
                Card data = game.players[Game.AI].hand.pickUpPile.pile.Pop();

                cmd.CommandText = "INSERT INTO AIPlayerHand(cardNum, cardSuit, pileNum, gameIndex) VALUES(" + data.GetNumber() + ", '" + data.GetSuit() + "', " + PICK_UP_PILE + "," + gameIndex + ")";
                cmd.ExecuteNonQuery();
            }

            //Saves AIs difficulty level
            cmd.CommandText = "INSERT INTO AIDifficulty(difficultyLevel, gameIndex) VALUES(" + game.players[Game.AI].GetDifficulty() + "," + gameIndex + ")";
            cmd.ExecuteNonQuery();
        }
        private void SavePlacePiles(SQLiteConnection conn, int gameIndex)
        {
            SQLiteCommand cmd = conn.CreateCommand();

            //Saves place piles
            for (int i = 0; i < game.pile1.pile.Length(); i++)
            {
                Card data = game.pile1.pile.Pop();

                cmd.CommandText = "INSERT INTO PlacePile(cardNum, cardSuit, pileNum, gameIndex) VALUES(" + data.GetNumber() + ", '" + data.GetSuit() + "', " + 0 + "," + gameIndex + ")";
                cmd.ExecuteNonQuery();
            }

            for (int i = 0; i < game.pile2.pile.Length(); i++)
            {
                Card data = game.pile2.pile.Pop();

                cmd.CommandText = "INSERT INTO PlacePile(cardNum, cardSuit, pileNum, gameIndex) VALUES(" + data.GetNumber() + ", '" + data.GetSuit() + "', " + 1 + "," + gameIndex + ")";
                cmd.ExecuteNonQuery();
            }
        }

        public void OverrideSavedGame(int gameIndex)
        {
            SQLiteCommand cmd = sqlite_conn.CreateCommand();
            SQLiteDataReader datareader;
            cmd.CommandText = "SELECT * FROM AIDifficulty WHERE gameIndex = " + gameIndex;
            datareader = cmd.ExecuteReader();
            if (datareader.Read() == true)
            {
                datareader.Close();
                cmd.CommandText = "DELETE FROM HumanPlayerHand WHERE gameIndex = " + gameIndex;
                cmd.ExecuteNonQuery();
                cmd.CommandText = "DELETE FROM AIPlayerHand WHERE gameIndex = " + gameIndex;
                cmd.ExecuteNonQuery();
                cmd.CommandText = "DELETE FROM PlacePile WHERE gameIndex = " + gameIndex;
                cmd.ExecuteNonQuery();
                cmd.CommandText = "DELETE FROM AIDifficulty WHERE gameIndex = " + gameIndex;
                cmd.ExecuteNonQuery();
            }
            SaveGameState(gameIndex);
        }
    }
}
