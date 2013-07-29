using System;
using System.IO;

namespace TestCodeReview.Refactor20130729
{

    ///Single Responsibility Principle Violation: There should never be more than one reason for a class to change.
 public class CsvReaderWriterCommented
    {
        ///I need to see the way this class interact with other object.
        
        private StreamReader _readerStream = null;
        private StreamWriter _writerStream = null;

        [Flags]
        public enum Mode { Read = 1, Write = 2 };


        /// <summary>
        /// separate the logic related to read from the logic related to write
        /// </summary>
        public void Open(string fileName, Mode mode)
        {
            //this code block smels like a Factory Method Design Pattern but only if theres a common interface you try to implemets it
            //using switch switch method
            if (mode == Mode.Read)
            {
                _readerStream = File.OpenText(fileName);
            }
            else if (mode == Mode.Write)
            {
                //use var
                FileInfo fileInfo = new FileInfo(fileName); //if the file already exist? manage this scenario.
                _writerStream = fileInfo.CreateText();
            }
            else
            {
                //throw is unnecessary if you use mode!
                throw new Exception("Unknown file mode for " + fileName);
            }
        }

        /// <summary>
        /// separate the logic related to read from the logic related to write
        /// </summary>
        public void Close()
        {
            if (_writerStream != null)
            {
                _writerStream.Close();
            }

            if (_readerStream != null)
            {
                _readerStream.Close();
            }
        }
        
        /// <summary>
        /// it is not clear whath this method do, it is called Read but it check if var columns are splittable by separator
        /// you colud Rename it in Check but code are duplicate with same function
        /// </summary>
        public bool Read(string column1, string column2)
        {
            const int FIRST_COLUMN = 0; //you do not need this const here.
            const int SECOND_COLUMN = 1;

            string line; //use var keyword
            string[] columns; //use var keyword

            char[] separator = { '\t' }; //'\it' is already use, delete duplications

            line = ReadLine(); //use native method here, not a private wrapper

            columns = line.Split(separator);

            //no one knows value of column1 and column2. why assign a value to them?
            if (columns.Length == 0)
            {
                column1 = null; //column1 are initialized but this value remain on the function scope and never returned 
                column2 = null; //same as for comlumn1

                return false;
            }
            else //you can delete else branch (obviously not the contained code)
            {
                column1 = columns[FIRST_COLUMN]; //do not use CONST here use integer
                column2 = columns[SECOND_COLUMN];

                return true;
            }
        }

        /// <summary>
        ///this function return a boolean and change the value of two string.
        ///this is an anti pattern. We need to refactor this method. 
        /// Return a list or array of string, null if there is no return value, push the control logic outside this class.
        /// or make this method obsolete.
        /// </summary>
        public bool Read(out string column1, out string column2)
        {
            const int FIRST_COLUMN = 0;
            const int SECOND_COLUMN = 1;

            string line;
            string[] columns;

            char[] separator = { '\t' };

            line = ReadLine();

            if (line == null)
            {
                column1 = null;
                column2 = null;

                return false;
            }

            columns = line.Split(separator);

            if (columns.Length == 0)
            {
                column1 = null;
                column2 = null;

                return false;
            }
            else
            {
                column1 = columns[FIRST_COLUMN];
                column2 = columns[SECOND_COLUMN];

                return true;
            }
        }

     /// <summary>
     /// we need it?
     /// This function is private, inside the class use directly the same method of the StreamReader
     /// </summary>
     /// <returns></returns>
        private string ReadLine()
        {
            return _readerStream.ReadLine();
        }


        public void Write(params string[] columns)
        {
            string outPut = ""; //use StringBuilder, instead of string

            //use linq we are in 2013
            for (int i = 0; i < columns.Length; i++)
            {
                outPut += columns[i];

                if ((columns.Length - 1) != i)
                {
                    //the separetor is defined two times delete duplications
                    outPut += "\t";
                }
            }

            //use directly a call to native object  
            //_writerStream.WriteLine(line);
            WriteLine(outPut);
        }

         /// <summary>
         /// Seriously we need this private method?
         /// </summary>
        private void WriteLine(string line)
        {
            _writerStream.WriteLine(line);
        }
    }
    
}