using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sqldiff
{
    public class Factory
    {
        #region property

        public MachineBox PreviousVersion { get; set; }
        public MachineBox CurrentVersion { get; set; }

        public Dictionary<string, Hashtable> DiffDictionary = new Dictionary<string, Hashtable>();

        #endregion

        #region Ctor

        public Factory(string previousFile, string currentFile)
        {
            PreviousVersion = new MachineBox(previousFile);
            CurrentVersion = new MachineBox(currentFile);
        }

        #endregion

        #region public Method

        public void Compare()
        {
            //compare diff
            ConstStatementDiff();
            InsertStatementDiff();

            //notification
            if (DiffDictionary.Keys.Count > 0)
            {
                var emailMessage = ConvertEmailMessage();
                Notify(emailMessage);
            }
        }

        public void Notify(string content)
        {
            //init smtp email

            //send
        }

        #endregion

        #region private methods

        private void ConstStatementDiff()
        {
            //exec statement
            if (PreviousVersion.STATE_EXECUTE != CurrentVersion.STATE_EXECUTE)
            {
                DiffDictionary.Add(Const.LINE_EXECUTE_PREFIX, InitHashTable(PreviousVersion.STATE_EXECUTE, CurrentVersion.STATE_EXECUTE));
            }

            //comments
            CommentStatemenDiff();

            //delete statement
            if (PreviousVersion.STATE_DELETE != CurrentVersion.STATE_DELETE)
            {
                DiffDictionary.Add(Const.LINE_DELETE_PREFIX, InitHashTable(PreviousVersion.STATE_DELETE, CurrentVersion.STATE_DELETE));
            }
        }

        private void InsertStatementDiff()
        {
            DiffDictionary.Add(Const.LINE_INSERT_PREFIX, InitHashTable(PreviousVersion.EventCollection, CurrentVersion.EventCollection));
        }

        private void CommentStatemenDiff()
        {
            DiffDictionary.Add(Const.LINE_COMMENTS_PREFIX, InitHashTable(PreviousVersion.STATE_COMMENTS, CurrentVersion.STATE_COMMENTS));
        }

        private Hashtable InitHashTable(object objA, object objB)
        {
            var table = new Hashtable();

            #region const string

            if (objA.GetType() == typeof(string))
            {
                table.Add(objA, objB);
                return table;
            }

            #endregion

            #region list string

            if (objA.GetType() == typeof(IList<string>))
            {
                var a = ((IList<string>)objA);
                var b = ((IList<string>)objB);

                if (a.Count == b.Count)
                {
                    for (int i = 0; i < a.Count; i++)
                    {
                        if (a[i] != b[i])
                        {
                            table.Add(a[i], b[i]);
                        }
                    }
                }
                else
                {
                    //a item not in b
                    var diff = a.Except(b);
                    if (diff.Any())
                    {
                        foreach (var item in diff)
                        {
                            table.Add(item, null);
                        }
                    }

                    //b item not in a
                    diff = b.Except(a);
                    if (diff.Any())
                    {
                        foreach (var item in diff)
                        {
                            table.Add(null, item);
                        }
                    }
                }

                return table;
            }

            #endregion

            #region list events

            if (objA.GetType() == typeof(IList<SystemEvent>))
            {
                var a = ((IList<SystemEvent>)objA);
                var b = ((IList<SystemEvent>)objB);

                if (a.Count == b.Count)
                {
                    for (int i = 0; i < a.Count; i++)
                    {
                        var instA = a[i];
                        var instB = b[i];

                        var properties = new SystemEvent().GetType().GetProperties();

                        var summarytable = new Hashtable();
                        var summaryTableKey = properties.SingleOrDefault(x => x.Name == "Name").Name;
                        foreach (var property in properties)
                        {
                            var childtable = new Hashtable();

                            var valA = property.GetValue(instA);
                            var valB = property.GetValue(instB);
                            if (valA != valB)
                            {
                                var columntable = new Hashtable();
                                columntable.Add(valA, valB);
                                childtable.Add(property.Name, childtable);
                            }
                            summarytable.Add(summaryTableKey, childtable);
                        }
                        //table.Add("key?", summarytable);
                    }
                }
            }

            return table;

            #endregion

        }

        private string ConvertEmailMessage()
        {
            //prepare dictionary keys and hash tables

            return string.Empty;
        }

        #endregion
    }
}
