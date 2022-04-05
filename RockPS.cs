using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using SHA3.Net;

namespace RockPS
{
    //Create a help table to define winning turns
    class HelpTable
    {
        public string getDefLength(string str, int length)
        {
            if (str.Length > length)
            {
                str = str.Substring(0, length);
                return str;
            }

            //Difference between strings
            int delta = length - str.Length;

            for (int i = 0; i < delta; i++)
            {
                str += " ";
            }
            return str;
        }

        public void getTable(string[] inputArray)
        {
            string[,] table = new string[inputArray.Length + 1, inputArray.Length + 1];

            table[0, 0] = "          ";

            //Fill first row and column
            for (int i = 1; i < inputArray.Length + 1; i++)
            {
                table[0, i] = getDefLength(inputArray[i - 1], 10);
                table[i, 0] = getDefLength(inputArray[i - 1], 10);
            }

            Rules rules = new Rules();

            for (int i = 1; i < inputArray.Length + 1; i++)
            {
                for (int j = 1; j < inputArray.Length + 1; j++)
                {
                    table[i, j] = getDefLength(rules.checkWinner(i, j, inputArray.Length), 10);
                }
            }

            for (int i = 0; i < inputArray.Length + 1; i++)
            {
                for (int j = 0; j < inputArray.Length + 1; j++)
                {
                    Console.Write(table[i, j]);
                }
                Console.WriteLine("");
            }
        }
    }

    //Define who win
    class Rules
    {
        public string checkWinner(int myIndex, int compIndex, int length)
        {
            int shift = (length - 1) / 2;

            if (myIndex == compIndex)
                return "Draw";

            int result = compIndex - myIndex;
            if (result < 0)
                result = length + result;

            result = result % length;

            if (result <= shift)
                return "Win";

            return "Lose";
        }
    }

    //Create key and HMAC
    class Generator
    {
        byte[] key = new byte[64];

        public void getKey()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(key);
        }
        public void getHMAC(string compTurn)
        {
            byte[] hash;
            using (var shaAlg = Sha3.Sha3256())
            {
                string byteArray = string.Join("", key);
                //Getting HMAC of computer's turn and the key
                hash = shaAlg.ComputeHash(Encoding.UTF8.GetBytes(compTurn + byteArray));
            }

            var printHash = string.Join("", hash);
            Console.WriteLine(printHash);
        }

        public void printKey()
        {
            string keyToPrint = string.Join("", key);
            Console.WriteLine(keyToPrint);
        }
    }

    class Programm
    {
        static void Main(string[] args)
        {
            //Checking input args
            if (args.Length < 3)
            {
                Console.WriteLine("Error! Input data must include 3 and more arguments");
                Environment.Exit(1);
            }

            if (args.Length % 2 == 0)
            {
                Console.WriteLine("Error! Number of arguments must be odd");
                Environment.Exit(1);
            }

            List<string> list = new List<string>(args);

            IEnumerable<string> distinctList = list.Distinct();

            if (list.Count() != distinctList.Count())
            {
                Console.WriteLine("Error! You must enter unique arguments");
                Environment.Exit(1);
            }

            //Generate computer's turn
            Generator generator = new Generator();
            generator.getKey();

            Random randomTurn = new Random();
            int numCompTurn = randomTurn.Next(1, args.Length + 1);
            //Computer choosing
            string compTurn = args[numCompTurn - 1];

            generator.getHMAC(compTurn);

            //Menu
            string choice = "";
            
            while (true)
            {
                for (int i = 1; i < args.Length + 1; i++)
                {
                    Console.WriteLine("{0} - {1}", i, args[i - 1]);
                }

                Console.WriteLine("0 - exit");
                Console.WriteLine("? - help");
                Console.Write("Enter your move: ");

                choice = Console.ReadLine();

                if (choice == "?")
                {
                    HelpTable table = new HelpTable();
                    table.getTable(args);
                    Console.Write("Enter your move: ");
                    choice = Console.ReadLine();
                }

                int res;
                if (int.TryParse(choice, out res) == false)
                    continue;

                if (Convert.ToInt32(choice) < args.Length + 1 && Convert.ToInt32(choice) >= 0)
                    break;
            }

            if (choice == "0")
                Environment.Exit(0);

            //Show your move
            Console.WriteLine("Your move: {0}", args[Convert.ToInt32(choice) - 1]);

            //Show computer move
            Console.WriteLine("Computer move: {0}", compTurn);

            Rules rules = new Rules();

            string result = rules.checkWinner(Convert.ToInt32(choice), numCompTurn, args.Length);

            //Check who won
            if (result == "Win")
                Console.WriteLine("You win!");
            else
                if (result == "Draw")
                    Console.WriteLine("It's only draw..");
                else
                    if (result == "Lose")
                        Console.WriteLine("You lost:(");

            Console.WriteLine("HMAC key:");

            generator.printKey();
        }

    }
}
