using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMentor
{
    public class MessageApplyInsertion
    {
        public MessageApplyInsertion()
        {
            this.Active = false;
        }

        public bool Active { get; set; }
        public int OldClipDuration { get; set; }
        public int InsertLocation { get; set; }
    }
}
