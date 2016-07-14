using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sqldiff
{
    public class Factory
    {
        #region Prop Const and property

        public const string LINE_COMMENTS_PREFIX = "--";
        public const string LINE_EXECUTE_PREFIX = "exec ";
        public const string LINE_DELETE_PREFIX = "DELETE FROM ";
        public const string LINE_SET_DEFINE_PREFIX = "SET DEFINE ";
        public const string LINE_INSERT_PREFIX = "Insert into ";

        public string STATE_EXECUTE { get; set; }
        public string STATE_SET_DEFINE { get; set; }
        public string STATE_DELETE { get; set; }

        public List<string> STATE_COMMENTS { get; set; }
        public List<string> STATE_INSERT { get; set; }

        #endregion

        #region Ctor

        public Factory(string filePath)
        {
            this.Separate(LoadResourceFile(filePath));
        }

        #endregion

        #region public Method

        public void Run()
        {
            //each insert statement serialize to system event

            //compare diff

            //notification
        }

        #endregion

        #region Init public property methods

        private string LoadResourceFile(string filePath)
        {
            try
            {
                var result = System.IO.File.ReadAllText(filePath);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Occur some error when load file: {0}.\r\n{1}", filePath, ex.Message));
            }
        }

        private void Separate(string fileContent)
        {
            if (!string.IsNullOrEmpty(fileContent))
            {
                var collection = fileContent.Split(new string[] { ";", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                if (collection != null && collection.Count() > 0)
                {
                    foreach (var item in collection)
                    {
                        this.LineUp(item);
                    }
                }
            }
        }

        private void LineUp(string lineContent)
        {
            if (lineContent.StartsWith(LINE_EXECUTE_PREFIX))
            {
                STATE_EXECUTE = lineContent;
            }
            if (lineContent.StartsWith(LINE_SET_DEFINE_PREFIX))
            {
                STATE_SET_DEFINE = lineContent;
            }
            if (lineContent.StartsWith(LINE_DELETE_PREFIX))
            {
                STATE_DELETE = lineContent;
            }
            if (lineContent.StartsWith(LINE_COMMENTS_PREFIX))
            {
                STATE_COMMENTS.Add(lineContent);
            }
            if (lineContent.StartsWith(LINE_INSERT_PREFIX))
            {
                STATE_INSERT.Add(lineContent);
            }
        }

        #endregion

        private string GetInsertValue(string insertStatement)
        {
            var currentArray = insertStatement.Split(new string[] { "(", ")", ";", }, StringSplitOptions.RemoveEmptyEntries);
            return currentArray[currentArray.Length - 1];
        }

        /// <summary>
        /// table name is stable and table column name is stable
        /// </summary>
        /// <param name="insertValue"></param>
        /// <returns></returns>
        public SystemEvent Serialize(string insertValue)
        {
            var collection = insertValue.Split(new string[] { ", " }, StringSplitOptions.None);

            var properties = new SystemEvent().GetType().GetProperties();

            if (collection.Count() == properties.Count())
            {
                var obj = new SystemEvent();
            }
            return null;
        }
    }
}
