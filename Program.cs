using System;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinMat3
{
    class Program
    {
        private static DataTable LoadFileFromCSV(string Filepath)
        {
            DataTable dt = new DataTable();
            DataColumn[] cols =
            {
                new DataColumn("Q", typeof(string)),
                //new DataColumn("0", typeof(string)),
                //new DataColumn("1", typeof(string)),
                //new DataColumn("2", typeof(string)),
                //new DataColumn("3", typeof(string)),
                //new DataColumn("4", typeof(string)),
                new DataColumn("A", typeof(string)),
                new DataColumn("B", typeof(string)),
                //new DataColumn("C", typeof(string)),
                //new DataColumn("E", typeof(string)),
                new DataColumn("#", typeof(string)),
                new DataColumn("Accept", typeof(string)),
            };
            dt.Columns.AddRange(cols);

            string FileContent = File.ReadAllText(Filepath);

            string[] lines = FileContent.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.None
                );
            foreach (var line in lines)
            {
                DataRow dr = dt.NewRow();
                string[] fields = line.Split(',');
                for (int i = 0; i < fields.Length; i++)
                {
                    dr[i] = fields[i];
                }
                dt.Rows.Add(dr);
            }
            dt.Rows[0].Delete();
            return dt;

        }
        private static void PrintCurrentStatus(string currentState, int currentTapePosition, char[] tape)
        {
            Console.WriteLine("Current state: {0}", currentState);
            Console.WriteLine("Current tape position: {0}", currentTapePosition);
            Console.WriteLine("Current symbol on tape: {0}", tape[currentTapePosition].ToString());
            Console.Write("Current tape state: ");
            foreach (char c in tape)
                Console.Write(c);
            Console.WriteLine(Environment.NewLine);
        }

        static void Main()
        {
            //load the table of steps from an external file
            DataTable stepTable = LoadFileFromCSV(@"C:\Users\BartoszPaszkiewicz\Desktop\TabPrzejsc-LinMat3.csv");
            stepTable.CaseSensitive = false;
            
            //ask for input string
            Console.WriteLine("Reading order: L -> R; Provide your word:");
            string word = Console.ReadLine();
            Console.WriteLine(Environment.NewLine);
            
            //define initial state
            string currentState = "qINIT";
            int currentTapePosition = 0;
            char[] tape = word.ToCharArray();


            do
            {
                //display current state
                PrintCurrentStatus(currentState, currentTapePosition, tape);
                
                //find the description of current state (all moves etc. -> a row from table of steps)
                DataRow[] selection = stepTable.Select(String.Format("Q LIKE '{0}'", currentState));

                //is answer a single row? if so, that's correct
                if (selection.Length == 1)
                {
                    //store information about the move using current symbol on the tape and the information from table of steps
                    string delta = (string)selection[0][tape[currentTapePosition].ToString()];
                    string[] deltaDetails = delta.Split(';');
                    //overwrite tape position
                    if (deltaDetails[0] != "-")
                        tape[currentTapePosition] = deltaDetails[0].ToCharArray()[0];
                    
                    //store next state
                    if (deltaDetails[1] != "-")
                        currentState = deltaDetails[1];
                    
                    //move tape
                    if (deltaDetails[2] == "L")
                        currentTapePosition--;
                    else if (deltaDetails[2] == "P")
                        currentTapePosition++;

                }
                else
                    throw new SystemException();

                //wait for user interaction to proceed
                Console.ReadKey();

                //keep operating until it's either error state or the # symbol is found;
            } while (currentState != "qError" && tape[currentTapePosition] != '#');

            //print the last state
            PrintCurrentStatus(currentState, currentTapePosition, tape);
           
            //accepted?
            if (currentState == "qINIT")
                Console.WriteLine("Accepting state detected!");
            else
                Console.WriteLine("Non-accepting state detected!");
            
            //wait for user to quit
            Console.ReadKey();
        
        }
    }
}
