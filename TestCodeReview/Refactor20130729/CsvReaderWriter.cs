using System;
using System.IO;

namespace TestCodeReview.Refactor20130729
{
    /// <summary>
    /// A junior developer was tasked with writing a reusable solution for an application to read and write text files that hold tab separated data.
    /// His implementation, although it works and meets the needs of the application, is of very low quality.
    /// Your task:
    ///     - Identify and annotate the shortcomings in the current implementation as if you were doing a code review, using comments in this file.
    ///     - In a fresh solution, refactor this implementation into clean,  elegant, rock-solid & well performing code, without over-engineering. 
    ///     -   Where you make trade offs, comment & explain.
    ///     - Assume this code is in production and it needs to be backwards compatible. Therefore if you decide to change the public interface, 
    ///       please deprecate the existing methods. Feel free to evolve the code in other ways though.   
    /// </summary>

    ///Single Responsibility Principle Violation: There should never be more than one reason for a class to change.
 public class CsvReaderWriter
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
            //use switch method
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
        /// it is a code monster    
        /// </summary>
        public bool Read(string column1, string column2)
        {
            const int FIRST_COLUMN = 0; //you do not need this const here
            const int SECOND_COLUMN = 1;

            string line; //use var keyword
            string[] columns; //use var keyword

            char[] separator = { '\t' }; //separator are a constant you have use it in other part delete duplications

            line = ReadLine(); //use native method here, not a private wrapper

            columns = line.Split(separator);

            if (columns.Length == 0)
            {
                column1 = null; //column1 are initialized but this value remain on the function scope and never returned 
                column2 = null; //same as for comlumn1

                return false;
            }
            else
            {
                column1 = columns[FIRST_COLUMN]; //same issue as in the if case
                column2 = columns[SECOND_COLUMN];

                return true;
            }
        }

        /// <summary>
        ///this function return a boolean and change the value of two string.
        ///this is an anti pattern we need to refactor this method. Return a list or array of string, null if there is no return value, push the result control outside this class
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
     /// </summary>
     /// <returns></returns>
        private string ReadLine()
        {
            return _readerStream.ReadLine();
        }


        public void Write(params string[] columns)
        {
            string outPut = ""; //use StringBuilder.

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