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
                var collection = fileContent.Split(new string[] { ";\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                if (collection != null && collection.Count() > 0)
                {
                    foreach (var item in collection)
                    {
                        this.LineUp(item.TrimStart(new char[] { '\r', '\n' }));
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
                var currentLineArray = lineContent.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (currentLineArray != null && currentLineArray.Count() > 0)
                {
                    foreach (var item in currentLineArray)
                    {
                        if (item.StartsWith(Const.LINE_COMMENTS_PREFIX))
                        {
                            STATE_COMMENTS.Add(item);
                        }
                        if (item.StartsWith(Const.LINE_DELETE_PREFIX))
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

        /// <summary>
        /// (925, 'Confirm Receipt of Shipment (Partial)', 'order', 'Confirm that a stock handler received only some of the items on his shipping order.', 'H',
        /// NULL, 0, 1, 0, 0,
        /// 0, 1, 1, 0, 1,
        /// 0, 11, 0, 0, 0,
        /// 0, 0)
        /// </summary>
        /// <param name="insertStatement"></param>
        /// <returns></returns>
        private string GetInsertValue(string insertStatement)
        {
            var currentArray = insertStatement.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            var values_line_index = Array.FindIndex(currentArray, w => w.Contains("Values"));
            //var subArray = new string[currentArray.Length - values_line_index - 1];
            //Array.Copy(currentArray, values_line_index + 1, subArray, 0, currentArray.Length - values_line_index - 1);

            var subArray = SubArray(currentArray, values_line_index + 1, currentArray.Length - values_line_index - 1);
            return string.Join("", subArray);
        }

        /// <summary>
        /// table name is stable and table column name is stable
        /// </summary>
        /// <param name="insertValue"></param>
        /// <returns></returns>
        private SystemEvent Serialize(string insertValue)
        {
            insertValue = insertValue.TrimStart(new char[] { ' ', '(' }).TrimEnd(')');
            var collection = insertValue.Split(new string[] { ", " }, StringSplitOptions.None);

            var properties = new SystemEvent().GetType().GetProperties();

            //normal split by comma
            if (collection.Count() == properties.Count())
            {
                var obj = new SystemEvent();
                for (int i = 0; i < collection.Length; i++)
                {
                    var value = collection[i].Trim(new char[] { '\r', '\n', '\'' });
                    if (properties[i].Name != "Name" || properties[i].Name != "Description")
                    {
                        value = value.Trim(' ');
                    }
                    properties[i].SetValue(obj, value);
                }
                return obj;
            }
            else
            {
                //special split result count > property count
                //1 handle by sort key, base on key value is not empty
                //2 extreme case, sort key is null or start with empty
                //3 descript is null
                // event id: 532, 292, 293 "" ""
                // event id: 3000, 372, 3001 "" "blabla..."
                // event id: 90, 91, 92 " note" "blabla..."
                var instance = new SystemEvent();

                try
                {
                    var sort_key_index = Array.FindIndex(collection, 1, x => !x.Contains(" "));
                    var categoryArray = new string[] { "H", "P", "F" };
                    var category1_index = Array.FindIndex(collection, x => categoryArray.Contains(x.Trim('\'')));

                    if (sort_key_index == category1_index)
                    {
                        sort_key_index = Array.FindIndex(collection, 1, x => x.EndsWith(" \'"));
                    }

                    if (sort_key_index < 0 || category1_index < 0)
                    {
                        throw new Exception("get sort key index or category1 index occur some error....");
                    }
                    var nameArrary = SubArray(collection, 1, sort_key_index - 1);
                    var descriptionArray = SubArray(collection, sort_key_index + 1, category1_index - sort_key_index - 1);

                    var id = collection[0];
                    var name = string.Join("", nameArrary);
                    var sort_key = collection[sort_key_index];
                    var description = string.Join(" ", descriptionArray);
                    var category1 = collection[category1_index];

                    instance.Id = id.Trim(new char[] { '\r', '\n', '\'' });
                    instance.Name = name.Trim(new char[] { '\r', '\n', '\'' });
                    instance.SortKey = sort_key.Trim(new char[] { '\r', '\n', '\'' });
                    instance.Description = description.Trim(new char[] { '\r', '\n', '\'' });
                    instance.Category1 = category1.Trim(new char[] { '\r', '\n', '\'' });

                    for (int i = category1_index + 1; i < collection.Length; i++)
                    {
                        var value = collection[i].Trim(new char[] { '\r', '\n', '\'' });
                        var extraIndex = (nameArrary.Length - 1) + (descriptionArray.Length - 1);
                        properties[i - extraIndex].SetValue(instance, value.Trim(' '));
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Insert value: " + insertValue + "\r\nSee the Details: " + ex.Message);
                }

                return instance;
            }
        }

        private string[] SubArray(string[] source, int index, int length)
        {
            string[] result = new string[length];
            Array.Copy(source, index, result, 0, length);
            return result;
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
