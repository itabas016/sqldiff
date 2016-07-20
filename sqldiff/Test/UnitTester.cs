using NUnit.Framework;
using sqldiff;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    [TestFixture]
    public class UnitTester
    {
        [Test]
        public void test_compare()
        {
            var execute_directory = Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(UnitTester)).Location);

            var previous = string.Format(@"{0}\{1}", execute_directory, "pre_ins_smp_SYSTEM_EVENT.sqlx");
            var current = string.Format(@"{0}\{1}", execute_directory, "cur_ins_smp_SYSTEM_EVENT.sqlx");

            var factory = new Factory(previous, current);
            factory.Compare();

            Assert.AreEqual(3, factory.DiffDictionary.Count);

            var delete_diff_ht = FakeDeleteStatementTable();
            Assert.AreEqual(factory.DiffDictionary[Const.LINE_DELETE_PREFIX], delete_diff_ht);

            var comments_diff_ht = FakeCommentsTable();
            Assert.AreEqual(factory.DiffDictionary[Const.LINE_COMMENTS_PREFIX], comments_diff_ht);

            var insert_diff_ht = FakeInsertTable();
            Assert.AreEqual(factory.DiffDictionary[Const.LINE_INSERT_PREFIX], insert_diff_ht);

        }

        private Hashtable FakeDeleteStatementTable()
        {
            var delete_diff_ht = new Hashtable();

            var previous = "DELETE FROM SYSTEM_EVENT where ID < 80000 or ID > 90000; ";
            var current = "DELETE FROM SYSTEM_EVENT where ID < 85000 or ID > 90000; ";
            delete_diff_ht.Add(previous, current);

            return delete_diff_ht;
        }

        private Hashtable FakeCommentsTable()
        {
            var comments_diff_ht = new Hashtable();

            var previous_exsit_line = "--Remove this previous test comments..";

            var previous_origin_line = "--The events in the range 80,000- 90,000 are custom, so we don't want to delete those.";
            var current_update_line = "--The events in the range 85,000- 90,000 are custom, so we don't want to delete those.";

            comments_diff_ht.Add(previous_exsit_line, null);
            comments_diff_ht.Add(previous_origin_line, current_update_line);

            return comments_diff_ht;
        }

        private Hashtable FakeInsertTable()
        {
            var insert_diff_ht = new Hashtable();

            /*
            //previous

            (500216, 'Test previous add new record', 'test_sortkey', 'test description blalalalal. for previous version', 'H', 
    NULL, 0, 3, 1, 0, 
    0, 1, 1, 0, 1, 
    0, 28, 0, 0, 0, 
    1, 2);

            (1015, 'OBS Add Channel Plan, and name will change', 'acces', 'Create a channel plan. this descrption will change in next version, and name will change', 'H', 
    NULL, 0, 1, 0, 0, 
    0, 1, 1, 0, 0, 
    0, NULL, 1, 0, 0, 
    0, 0);

            (99, 'OBS Change Proxy Code on Financial Account', 'Acc  ', 'Change the proxy code on a customer''s financial account.', 'H', 
    NULL, 0, 0, 0, 0, 
    0, 1, 1, 0, 0, 
    1, NULL, 1, 0, 0, 
    0, 0);

            (110, 'Unlink Product from Device', 'sspr ', 'Remove the link between a software product and a device.', 'H', 
    NULL, 0, 0, 0, 0, 
    0, 1, 1, 0, 0, 
    0, 34, 0, 0, 0, 
    0, 0);

            //current

            (500219, 'Test current add new record', 'test_sort_key', 'test description blalalalal. for current version', 'H', 
    NULL, 0, 3, 1, 0, 
    0, 1, 1, 0, 1, 
    0, 28, 0, 0, 0, 
    1, 2);

            (1015, 'OBS Add Channel Plan', 'acces', 'Create a channel plan.', 'H', 
    NULL, 0, 1, 0, 0, 
    0, 1, 1, 0, 0, 
    0, NULL, 1, 0, 0, 
    0, 0);

            (99, 'OBS Change Proxy Code on Financial Account, and name change', 'Acc  ', 'Change the proxy code on a customer, update description in current version''s financial account.', 'H', 
    NULL, 0, 0, 0, 0, 
    0, 1, 1, 40, 0, 
    1, NULL, 1, 0, 0, 
    0, 10);

            (110, 'Unlink Product from Device', 'sspr ', 'Remove the link between a software product and a device. update description in this current version', 'H', 
    NULL, 0, 0, 0, 0, 
    0, 1, 1, 0, 0, 
    0, 34, 0, 0, 0, 
    0, 0);

            */

            return insert_diff_ht;
        }
    }
}
