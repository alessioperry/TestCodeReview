using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestCodeReview.RefcatoredAndRetrocompatible
{
    public class CSVReaderWriter
    {
        private CsvReader csvRader;
        private CsvWriter csvWriter;

        public void Open(string fileName, Mode mode)
        {
            switch (mode)
            {
                case Mode.Read:
                    csvRader = new CsvReader(fileName);
                    break;
                case Mode.Write:
                    csvWriter = new CsvWriter(fileName);
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

        public bool Read(string column1, string column2)
        {
            var result = csvRader.Read();

            return result != null;
        }

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

        public void Write(params string[] columns)
        {
            csvWriter.Write(columns);
        }
    }

    public interface ICommonFileOperation : IDisposable
    {
        void Open();
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

        private readonly string fileName;
        private const char Separator = '\t';

        public CsvReader(string fileName)
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


        public List<string> Read()
        {
            var line = reader.ReadLine();

            return line == null ? null : line.Split(Separator).ToList();
        }
    }

    public class CsvWriter : ICommonFileOperation
    {
        private StreamWriter writer;

        private readonly string fileName;
        private const string Separator = "\t";

        public CsvWriter(string fileName)
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

        public void Write(List<string> columns)
        {
            writer.WriteLine(string.Join(Separator, columns));
        }

    }
}
