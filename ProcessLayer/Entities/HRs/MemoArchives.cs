using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Entities
{
    public class MemoArchives
    {
        public long ID { get; set; }
        [DisplayName("Memo Type")]
        public short? MemoTypeID { get; set; }
        [DisplayName("Memo No.")]
	    public string MemoNo { get; set; }
        [DisplayName("Memo Date")]
        public DateTime? MemoDate { get; set; }
        public long? InReplyTo { get; set; }
        public bool? PersonnelReply { get; set; }
        public string Description { get; set; }
        public string Subject { get; set; }
        public string File { get; set; }
        public bool SaveOnly { get; set; }
        public int StatusId { get; set; }
        public long CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public Lookup _MemoStatus { get; set; } = new Lookup();
        public Lookup _MemoType { get; set; } = new Lookup();

        public List<PersonnelGroup> _Groups { get; set; } = new List<PersonnelGroup>();
        public List<Personnel> _Persons { get; set; } = new List<Personnel>();

        public PersonnelGroup _Group { get; set; } = new PersonnelGroup();
        public Personnel _Person { get; set; } = new Personnel();

        public bool IsFailed { get; set; } = false;
        public bool IsExistFile { get; set; } = false;

        public List<MemoArchives> _Replies { get; set; } = new List<MemoArchives>();
        public int Replies_Count { get; set; }
        public int Files_Count { get; set; }
        public Personnel _Sender { get; set; }
        public bool IsForward { get; set; }
    }
}
