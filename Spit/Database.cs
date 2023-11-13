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
                Console.WriteLine("Fail");
            }
            return conn;
        }

        private static void DeleteTables(SQLiteConnection conn)
        {
            SQLiteCommand cmd = conn.CreateCommand();

            string createHumanPlayerHandTableCmd = "DROP TABLE HumanPlayerHand";
            string createAIPlayerHandTableCmd = "DROP TABLE AIPlayerHand";
            string createPlacePilesTableCmd = "DROP TABLE PlacePile";

            cmd.CommandText = createHumanPlayerHandTableCmd;
            cmd.ExecuteNonQuery();
            cmd.CommandText = createAIPlayerHandTableCmd;
            cmd.ExecuteNonQuery();
            cmd.CommandText = createPlacePilesTableCmd;
            cmd.ExecuteNonQuery();
        }

        private static void CreateTable(SQLiteConnection conn)
        {
            SQLiteCommand cmd = conn.CreateCommand();

            string createHumanPlayerHandTableCmd = "CREATE TABLE HumanPlayerHand(Col1 INT, Col2 VARCHAR(7), Col3 INT)";
            string createAIPlayerHandTableCmd = "CREATE TABLE AIPlayerHand(Col1 INT, Col2 VARCHAR(7), Col3 INT)";
            string createPlacePilesTableCmd = "CREATE TABLE PlacePile(Col1 INT, Col2 VARCHAR(7), Col3 INT)";

            cmd.CommandText = createHumanPlayerHandTableCmd;
            cmd.ExecuteNonQuery();
            cmd.CommandText = createAIPlayerHandTableCmd;
            cmd.ExecuteNonQuery();
            cmd.CommandText = createPlacePilesTableCmd;
            cmd.ExecuteNonQuery();
        }

        private void InsertData(SQLiteConnection conn)
        {
            SQLiteCommand cmd = conn.CreateCommand();

            //Stores players hand in database
            for (int i = 0; i < game.players[Game.HUMAN].hand.piles.Length; i++)
            {
                for (int x = 0; x < game.players[Game.HUMAN].hand.piles[i].pile.Length(); x++)
                {
                    Card data = game.players[Game.HUMAN].hand.piles[i].pile.Pop();

                    string test = "INSERT INTO HumanPlayerHand(Col1, Col2, Col3) VALUES(" + data.GetNumber() + ", '" + data.GetSuit() + "', " + i + ")";

                    cmd.CommandText = test;
                    cmd.ExecuteNonQuery();
                }
            }
            for (int i = 0; i < game.players[Game.HUMAN].hand.pickUpPile.pile.Length(); i++)
            {
                Card data = game.players[Game.HUMAN].hand.pickUpPile.pile.Pop();

                cmd.CommandText = "INSERT INTO HumanPlayerHand(Col1, Col2, Col3) VALUES(" + data.GetNumber() + ", '" + data.GetSuit() + "', " + PICK_UP_PILE + ")";
                cmd.ExecuteNonQuery();
            }

            //Stores AIs hand in database
            for (int i = 0; i < game.players[Game.AI].hand.piles.Length; i++)
            {
                for (int x = 0; x < game.players[Game.AI].hand.piles[i].pile.Length(); x++)
                {
                    Card data = game.players[Game.AI].hand.piles[i].pile.Pop();

                    cmd.CommandText = "INSERT INTO AIPlayerHand(Col1, Col2, Col3) VALUES(" + data.GetNumber() + ", '" + data.GetSuit() + "', " + i + ")";
                    cmd.ExecuteNonQuery();
                }
            }
            for (int i = 0; i < game.players[Game.AI].hand.pickUpPile.pile.Length(); i++)
            {
                Card data = game.players[Game.AI].hand.pickUpPile.pile.Pop();

                cmd.CommandText = "INSERT INTO AIPlayerHand(Col1, Col2, Col3) VALUES(" + data.GetNumber() + ", '" + data.GetSuit() + "', " + PICK_UP_PILE + ")";
                cmd.ExecuteNonQuery();
            }


            for (int i = 0; i < game.pile1.pile.Length(); i++)
            {
                Card data = game.pile1.pile.Pop();

                cmd.CommandText = "INSERT INTO PlacePile(Col1, Col2, Col3) VALUES(" + data.GetNumber() + ", '" + data.GetSuit() + "', " + 0 + ")";
                cmd.ExecuteNonQuery();
            }

            for (int i = 0; i < game.pile2.pile.Length(); i++)
            {
                Card data = game.pile2.pile.Pop();

                cmd.CommandText = "INSERT INTO PlacePile(Col1, Col2, Col3) VALUES(" + data.GetNumber() + ", '" + data.GetSuit() + "', " + 1 + ")";
                cmd.ExecuteNonQuery();
            }
        }

        private void ReadData(SQLiteConnection conn)
        {
            Hand plHand = new Hand();
            SQLiteCommand cmd = conn.CreateCommand();
            SQLiteDataReader datareader;
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
                    plHand.piles[pileNum].pile.Push(card);
                }
                else
                {
                    plHand.pickUpPile.pile.Push(card);
                }
            }
            game.players[Game.HUMAN].hand = plHand;



            Hand aiHand = new Hand();
            cmd.CommandText = "SELECT * FROM AIPlayerHand";
            datareader = cmd.ExecuteReader();
            while (datareader.Read())
            {
                string cardSuit = datareader.GetString(1);
                int cardNum = Convert.ToInt32(datareader.GetString(0));
                int pileNum = Convert.ToInt32(datareader.GetString(2));

                Card card = new Card(cardSuit, cardNum);

                if (pileNum != -1)
                {
                    aiHand.piles[pileNum].pile.Push(card);
                }
                else
                {
                    aiHand.pickUpPile.pile.Push(card);
                }
            }
            game.players[Game.AI].hand = aiHand;



            Pile pile = new Pile();
            cmd.CommandText = "SELECT * FROM PlacePiles";
            datareader = cmd.ExecuteReader();
            while (datareader.Read())
            {
                string cardSuit = datareader.GetString(1);
                int cardNum = Convert.ToInt32(datareader.GetString(0));
                int pileNum = Convert.ToInt32(datareader.GetString(2));

                Card card = new Card(cardSuit, cardNum);

                game.placePiles[pileNum].pile.Push(card);
            }


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
    }
}
