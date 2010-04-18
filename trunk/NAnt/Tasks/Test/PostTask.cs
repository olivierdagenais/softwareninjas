using System.Collections.Generic;

using NUnit.Framework;
using Parent = SoftwareNinjas.NAnt.Tasks;

namespace SoftwareNinjas.NAnt.Tasks.Test
{
    /// <summary>
    /// A class to test <see cref="Parent.PostTask"/>.
    /// </summary>
    [TestFixture]
    public class PostTask
    {
        /// <summary>
        /// Tests the <see cref="Parent.PostTask.CreateNameValueCollection(IEnumerable{FormField})" /> method with
        /// fields originally taken form a Google Spreadsheets form.
        /// </summary>
        [Test]
        public void CreateNameValueCollection_GoogleSpreadsheets()
        {
            var fields = new Parent.FormField[]
            {
                new FormField { FieldName = "entry.1.single", Value = "0"}, 
                new FormField { FieldName = "entry.2.single", Value = "StringExtensions"}, 
                new FormField { FieldName = "entry.3.single", Value = "base"}, 
                new FormField { FieldName = "entry.4.single", Value = "n/a"}, 
                new FormField { FieldName = "entry.5.single", Value = "15"}, 
                new FormField { FieldName = "entry.6.single", Value = "15"}, 
                new FormField { FieldName = "pageNumber", Value = "0"}, 
                new FormField { FieldName = "backupCache"}, 
                new FormField { FieldName = "submit", Value = "Submit"}, 
            };
            var actual = Parent.PostTask.CreateNameValueCollection(fields);
            Assert.AreEqual(9, actual.Count);
        }

    }
}
