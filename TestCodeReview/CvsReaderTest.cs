using NUnit.Framework;
using TestCodeReview.Refactor20130729;
using TestCodeReview.RefcatoredAndRetrocompatible;

namespace TestCodeReview
{
    [TestFixture]
    public class CvsReaderTest
    {
        //[SetUp]
        //public void SetUp()
        //{
        //    var fileInfo = new FileInfo(@"..\..\TestFiles\testTempRead.csv");

            
        //    var streamWriter = fileInfo.CreateText();

        //    streamWriter.WriteLine(@"value1\tvalue2\tvalue3\tvalue4");
        //    streamWriter.WriteLine(@"value5\tvalue6\tvalue7\tvalue8");
        //    streamWriter.WriteLine(@"value9\tvalue10\tvalue11\tvalue12");
        //    streamWriter.WriteLine(@"value13\tvalue14\tvalue15\tvalue16");

        //    streamWriter.Close();
        //    streamWriter.Dispose();
        //}

        //[TearDown]
        //public void TearDown()
        //{
        //    var fileInfo = new FileInfo(@"..\..\TestFiles\testTempRead.csv");

        //    if(fileInfo.Exists)
        //        fileInfo.Delete();
        //}


        [Test]
        public void ReadCommentedCsvWriterReader()
        {
            var commented = new CsvReaderWriterCommented();
          
            commented.Open(@"..\..\TestFiles\testRead.csv", CsvReaderWriterCommented.Mode.Read);

            var column1 = string.Empty;
            var column2 = string.Empty;

            var read = commented.Read(out column1, out column2);

            Assert.AreEqual("value1",column1);

            Assert.AreEqual("value2", column2);

            Assert.IsTrue(read);

            commented.Close();


        }

        [Test]
        public void ReaderWriterRefactoredButRetrocompatible()
        {
            using (var refactored =  new CsvReaderWriterRefactoredButRetrocompatible())
            {
                refactored.Open(@"..\..\TestFiles\testRead.csv", Mode.Read);

                var column1 = string.Empty;
                var column2 = string.Empty;

                var result = refactored.Read(out column1, out column2);

                Assert.IsTrue(result);

                Assert.AreEqual("value1", column1);

                Assert.AreEqual("value2", column2);

                refactored.Close();
            }
        }
     }
}