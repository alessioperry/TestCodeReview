using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCodeReview.Refactor20130729
{

    namespace TestCodeReview.Refactor20130729
    {
        /// <summary>
        /// A junior developer was tasked with writing a reusable solution for an application to read and write text files that hold tab separated data.
        /// His implementation, although it works and meets the needs of the application, is of very low quality.
        /// Your task:
        ///     - Identify and annotate the shortcomings in the current implementation as if you were doing a code review, using comments in this file.
        ///     - In a fresh solution, refactor this implementation into clean,  elegant, rock-solid & well performing code, without over-engineering. 
        ///     - Where you make trade offs, comment & explain.
        ///     - Assume this code is in production and it needs to be backwards compatible. Therefore if you decide to change the public interface, 
        ///       please deprecate the existing methods. Feel free to evolve the code in other ways though.   
        /// </summary>

        ///The Single Responsibility Principle: There should never be more than one reason for a class to change.
        /// You need CvsReader Class and CvsWriter Class here
        public class CsvReaderWriter
        {
            private StreamReader _readerStream = null;
            private StreamWriter _writerStream = null;

            [Flags]
            public enum Mode { Read = 1, Write = 2 };


            /// <summary>
            /// separate the logic related to read from the logic related to write
            /// </summary>
            public void Open(string fileName, Mode mode)
            {
                //this code block smels like a Factory Method pattern but only if theres a common interface
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
            /// </summary>
            public bool Read(string column1, string column2)
            {
                const int FIRST_COLUMN = 0; //make variable 
                const int SECOND_COLUMN = 1;

                string line; //use inline inizialization
                string[] columns; //use inline inizialization

                char[] separator = { '\t' }; //separator are a constant

                line = ReadLine();

                columns = line.Split(separator);//performance issue splitted string allocate heap memory and can cause Out Of Memory exception

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

            private string ReadLine()
            {
                return _readerStream.ReadLine();
            }


            public void Write(params string[] columns)
            {
                string outPut = "";

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

            //a wrapper to native method if you need to add some additional logic it is ok
            //even if this class has to me derived
            private void WriteLine(string line)
            {
                _writerStream.WriteLine(line);
            }
        }

        //my refactoring


        public interface ICommonFileOperation : IDisposable
        {
            void Open();
            void Close();
        }

        [Flags]
        public enum Mode { Read = 1, Write = 2 };

        public class CvsReader : ICommonFileOperation
        {
            private StreamReader reader;
            private readonly string fileName;

            public CvsReader(string fileName)
            {
                this.fileName = fileName;
            }

            public void Open()
            {
                if (!File.Exists(fileName))
                    throw new ArgumentException("File not found!");

                reader = File.OpenText(fileName);
            }

            public void Close()
            {
                reader.Close();
            }

            public void Dispose()
            {
                if (reader == null) return;

                reader.Dispose();
            }

            public bool Read()
            {
                var line = reader.ReadLine();

                if (line == null)
                    return false;
               
                var columns = line.Split('\t');

                return (columns.Length > 0);
            }

 
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

            public string ReadLine()
            {
                return reader.ReadLine();
            }
        }

        //[System.Obsolete("use class B")]

        public class Writer : ICommonFileOperation
        {
            private StreamWriter writer;
            private readonly string fileName;

            public Writer(string fileName)
            {
                this.fileName = fileName;
            }

            public void Open()
            {
                if (File.Exists(fileName))
                    throw new ArgumentException("File already exist!");

                var fileInfo = new FileInfo(fileName);

                writer = fileInfo.CreateText();
            }

            public void Close()
            {
                writer.Close();
            }

            public void Dispose()
            {
                if (writer == null) return;

                writer.Dispose();
            }

            public void WriteLine(string line)
            {
                writer.WriteLine(line);
            }

        }



    }
}
