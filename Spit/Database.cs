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
        SQLiteConnection sqlite_conn;

        const int PICK_UP_PILE = -1;

        Game game;
        public Database(Game game)
        {
            this.game = game;
            sqlite_conn = CreateConnection();

            //DeleteTables(sqlite_conn);

            //CreateTable(sqlite_conn);
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
                Console.WriteLine("Failed to create a connect to the database.");
            }
            return conn;
        }

        private static void CreateTable(SQLiteConnection conn)
        {
            SQLiteCommand cmd = conn.CreateCommand();

            string createHumanPlayerHandTableCmd = "CREATE TABLE HumanPlayerHand(cardNum INT, cardSuit VARCHAR(7), pileNum INT)";
            string createAIPlayerHandTableCmd = "CREATE TABLE AIPlayerHand(cardNum INT, cardSuit VARCHAR(7), pileNum INT)";
            string createPlacePilesTableCmd = "CREATE TABLE PlacePile(cardNum INT, cardSuit VARCHAR(7), pileNum INT)";
            string createAIDifficultyTableCmd = "CREATE TABLE AIDifficulty(cardNum INT)";

            cmd.CommandText = createHumanPlayerHandTableCmd;
            cmd.ExecuteNonQuery();
            cmd.CommandText = createAIPlayerHandTableCmd;
            cmd.ExecuteNonQuery();
            cmd.CommandText = createPlacePilesTableCmd;
            cmd.ExecuteNonQuery();
            cmd.CommandText = createAIDifficultyTableCmd;
            cmd.ExecuteNonQuery();
        }
        private static void DeleteTables(SQLiteConnection conn)
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

        private void InsertData(SQLiteConnection conn)
        {
            SavePlayerHand(conn);
            SaveAI(conn);
            SavePlacePiles(conn);
        }

        private void ReadData(SQLiteConnection conn)
        {
            CreatePlayers(conn);
            ExtractPlayerHand(conn);
            ExtractAIHand(conn);
            ExtractPlacePiles(conn);            

            conn.Close();
        }

        public void SaveGameState()
        {
            InsertData(sqlite_conn);
        }

        public void LoadGameState()
        {
            ReadData(sqlite_conn);
        }

        public void CreatePlayers(SQLiteConnection conn)
        {
            SQLiteCommand cmd = conn.CreateCommand();
            SQLiteDataReader datareader;
            cmd.CommandText = "SELECT * FROM AIDifficulty";
            datareader = cmd.ExecuteReader();
            datareader.Read();

            int difficulty = datareader.GetInt32(0);
            game.CreatePlayers(difficulty);
        }

        public void ExtractPlayerHand(SQLiteConnection conn)
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
        public void ExtractAIHand(SQLiteConnection conn)
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
        public void ExtractPlacePiles(SQLiteConnection conn)
        {
            SQLiteCommand cmd = conn.CreateCommand();
            SQLiteDataReader datareader;

            cmd.CommandText = "SELECT * FROM PlacePile";
            datareader = cmd.ExecuteReader();
            while (datareader.Read())
            {
                string cardSuit = datareader.GetString(1);
                int cardNum = datareader.GetInt32(0);
                int pileNum = datareader.GetInt32(2);

                Card card = new Card(cardSuit, cardNum);

                game.placePiles[pileNum].pile.Push(card);
            }
            datareader.Close();
        }

        public void SavePlayerHand(SQLiteConnection conn)
        {
            SQLiteCommand cmd = conn.CreateCommand();

            //Saves players hand 
            for (int i = 0; i < game.players[Game.HUMAN].hand.piles.Length; i++)
            {
                for (int x = 0; x < game.players[Game.HUMAN].hand.piles[i].pile.Length(); x++)
                {
                    Card data = game.players[Game.HUMAN].hand.piles[i].pile.Pop();

                    string test = "INSERT INTO HumanPlayerHand(cardNum, cardSuit, pileNum) VALUES(" + data.GetNumber() + ", '" + data.GetSuit() + "', " + i + ")";

                    cmd.CommandText = test;
                    cmd.ExecuteNonQuery();
                }
            }
            for (int i = 0; i < game.players[Game.HUMAN].hand.pickUpPile.pile.Length(); i++)
            {
                Card data = game.players[Game.HUMAN].hand.pickUpPile.pile.Pop();

                cmd.CommandText = "INSERT INTO HumanPlayerHand(cardNum, cardSuit, pileNum) VALUES(" + data.GetNumber() + ", '" + data.GetSuit() + "', " + PICK_UP_PILE + ")";
                cmd.ExecuteNonQuery();
            }

            //Saves AIs difficulty level
            cmd.CommandText = "INSERT INTO AIDifficulty(cardNum) VALUES(" + game.players[Game.AI].GetDifficulty() + ")";
            cmd.ExecuteNonQuery();
        }
        public void SaveAI(SQLiteConnection conn)
        {
            SQLiteCommand cmd = conn.CreateCommand();

            //Saves AIs hand
            for (int i = 0; i < game.players[Game.AI].hand.piles.Length; i++)
            {
                for (int x = 0; x < game.players[Game.AI].hand.piles[i].pile.Length(); x++)
                {
                    Card data = game.players[Game.AI].hand.piles[i].pile.Pop();

                    cmd.CommandText = "INSERT INTO AIPlayerHand(cardNum, cardSuit, pileNum) VALUES(" + data.GetNumber() + ", '" + data.GetSuit() + "', " + i + ")";
                    cmd.ExecuteNonQuery();
                }
            }
            for (int i = 0; i < game.players[Game.AI].hand.pickUpPile.pile.Length(); i++)
            {
                Card data = game.players[Game.AI].hand.pickUpPile.pile.Pop();

                cmd.CommandText = "INSERT INTO AIPlayerHand(cardNum, cardSuit, pileNum) VALUES(" + data.GetNumber() + ", '" + data.GetSuit() + "', " + PICK_UP_PILE + ")";
                cmd.ExecuteNonQuery();
            }
        }
        public void SavePlacePiles(SQLiteConnection conn)
        {
            SQLiteCommand cmd = conn.CreateCommand();

            //Saves place piles
            for (int i = 0; i < game.pile1.pile.Length(); i++)
            {
                Card data = game.pile1.pile.Pop();

                cmd.CommandText = "INSERT INTO PlacePile(cardNum, cardSuit, pileNum) VALUES(" + data.GetNumber() + ", '" + data.GetSuit() + "', " + 0 + ")";
                cmd.ExecuteNonQuery();
            }

            for (int i = 0; i < game.pile2.pile.Length(); i++)
            {
                Card data = game.pile2.pile.Pop();

                cmd.CommandText = "INSERT INTO PlacePile(cardNum, cardSuit, pileNum) VALUES(" + data.GetNumber() + ", '" + data.GetSuit() + "', " + 1 + ")";
                cmd.ExecuteNonQuery();
            }
        }
    }
}
