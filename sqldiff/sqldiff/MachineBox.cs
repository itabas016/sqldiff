using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sqldiff
{
    public class MachineBox
    {
        #region Property

        public string STATE_EXECUTE { get; set; }
        public string STATE_SET_DEFINE { get; set; }
        public string STATE_DELETE { get; set; }

        public List<string> STATE_COMMENTS = new List<string>();
        public List<string> STATE_INSERT = new List<string>();

        public List<SystemEvent> EventCollection = new List<SystemEvent>();

        #endregion

        #region Ctor

        public MachineBox(string filePath)
        {
            this.Separate(LoadResourceFile(filePath));
            this.EventCollection = Loading();
        }

        #endregion

        #region private methods

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
                var collection = fileContent.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                if (collection != null && collection.Count() > 0)
                {
                    foreach (var item in collection)
                    {
                        this.LineUp(item.TrimStart(new char[] { '\n', '\n' }));
                    }
                }
            }
        }

        private void LineUp(string lineContent)
        {
            if (lineContent.StartsWith(Const.LINE_EXECUTE_PREFIX))
            {
                STATE_EXECUTE = lineContent;
            }
            if (lineContent.StartsWith(Const.LINE_SET_DEFINE_PREFIX))
            {
                STATE_SET_DEFINE = lineContent;
            }
            if (lineContent.StartsWith(Const.LINE_DELETE_PREFIX))
            {
                STATE_DELETE = lineContent;
            }
            if (lineContent.StartsWith(Const.LINE_COMMENTS_PREFIX))
            {
                var currentLineArray = lineContent.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (currentLineArray != null && currentLineArray.Count() > 0)
                {
                    foreach (var item in currentLineArray)
                    {
                        if (item.StartsWith(Const.LINE_COMMENTS_PREFIX))
                        {
                            STATE_COMMENTS.Add(item);
                        }
                        if (lineContent.StartsWith(Const.LINE_DELETE_PREFIX))
                        {
                            STATE_DELETE = item;
                        }
                    }
                }
            }
            if (lineContent.StartsWith(Const.LINE_INSERT_PREFIX))
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
        private SystemEvent Serialize(string insertValue)
        {
            var collection = insertValue.Split(new string[] { ", " }, StringSplitOptions.None);

            var properties = new SystemEvent().GetType().GetProperties();

            if (collection.Count() == properties.Count())
            {
                var obj = new SystemEvent();
                for (int i = 0; i < collection.Length; i++)
                {
                    var value = collection[i].Trim(new char[] { '\n', '\'' });
                    if (properties[i].Name != "Name" || properties[i].Name != "Description")
                    {
                        value = value.Trim(' ');
                    }
                    properties[i].SetValue(obj, value);
                }
                return obj;
            }
            return null;
        }

        private List<SystemEvent> Loading()
        {
            var collection = new List<SystemEvent>();
            if (STATE_INSERT.Count > 0)
            {
                foreach (var item in STATE_INSERT)
                {
                    var obj = Serialize(GetInsertValue(item));
                    collection.Add(obj);
                }
            }
            return collection;
        }

        #endregion
    }
}
