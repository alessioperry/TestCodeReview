using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestCodeReview.RefcatoredAndRetrocompatible
{
    public class CsvReaderWriterRefactoredButRetrocompatible :IDisposable
    {
        private CsvReader csvRader;
        private CsvWriter csvWriter;


        /// <summary>
        /// need to create new object ad assigment here.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="mode"></param>
        public void Open(string fileName, Mode mode)
        {
            switch (mode)
            {
                case Mode.Read:
                    csvRader = new CsvReader();
                    csvRader.Open(fileName);
                    break;
                case Mode.Write:
                    csvWriter = new CsvWriter();
                    csvWriter.Open(fileName);
                    break;
            }
        }

        public void Close()
        {
            if(csvRader != null)
                csvRader.Close();
            if (csvWriter != null)
                csvWriter.Close();
        }

        /// <summary>
        /// Return a row splitted by tab
        /// </summary>
        /// <returns>Null for empty response</returns>
        public List<string> Read()
        {
            return csvRader.Read();
        }

        [Obsolete("Deprecated.", true)]
        public bool Read(string column1, string column2)
        {
            var result = csvRader.Read();

            return result != null;
        }

        [Obsolete("Deprecated.")]
        public bool Read(out string column1, out string column2)
        {
            var result = csvRader.Read();

            column1 = string.Empty;
            column2 = string.Empty;

            if (result == null)
                return false;

            column1 = result[0];
            column2 = result[1];

            return true;
        }

        public void Write(List<string> columns)
        {
            csvWriter.Write(columns);
        }

        [Obsolete("Deprecated.", true)]
        public void Write(params string[] columns)
        {
            csvWriter.Write(columns.ToList());
        }

        public void Dispose()
        {
            if (csvRader != null)
                csvRader.Dispose();
            if (csvWriter != null)
                csvWriter.Dispose();
        }
    }

    public interface ICommonFileOperation : IDisposable
    {
        void Open(string fileName);
        void Close();
    }


    [Flags]
    public enum Mode
    {
        Read = 1,
        Write = 2
    };

    public class CsvReader : ICommonFileOperation
    {
        private StreamReader reader;

        private const char Separator = '\t';

        public void Open(string fileName)
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

        public List<string> Read()
        {
            var line = reader.ReadLine();

            return line == null ? null : line.Split(Separator).ToList();
        }
    }

    public class CsvWriter : ICommonFileOperation
    {
        private StreamWriter writer;

        private const string Separator = "\t";


        public void Open(string fileName)
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

        public void Write(List<string> columns)
        {
            writer.WriteLine(string.Join(Separator, columns));
        }

    }
}
