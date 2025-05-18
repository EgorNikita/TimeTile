using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Enums;

namespace TimeTile.Core.Models
{
    public class File : AuditableEntity
    {
        public string OriginalName { get; set; } = null!;

        public FileExtension Extension { get; set; } 

        public long Size { get; set; }

        public string StoragePath { get; set; } = null!;

        public virtual ClassroomType? ClassroomType { get; set; }
    }
}
