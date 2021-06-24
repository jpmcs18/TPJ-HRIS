using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ProcessLayer.Entities
{
    public class PersonnelMemo : PersonnelBase
    {
        public long? MemoID { get; set; }

        public MemoArchives _MemoArchive { get; set; } = new MemoArchives();
    }
}
