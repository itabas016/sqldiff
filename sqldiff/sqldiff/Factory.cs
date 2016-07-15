using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sqldiff
{
    public class Factory
    {
        #region property

        public Machine PreviousVersion { get; set; }
        public Machine CurrentVersion { get; set; }

        #endregion

        #region Ctor

        public Factory(string previousFile, string currentFile)
        {
            PreviousVersion = new Machine(previousFile);
            CurrentVersion = new Machine(currentFile);
        }

        #endregion

        #region public Method

        public void Compare()
        {
            //compare diff

            //notification
        }

        public void Notify(string content)
        {
        }

        #endregion
    }
}
