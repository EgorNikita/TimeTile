using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Enums;
using TimeTile.Core.Models.Interfaces;

namespace TimeTile.Core.Models
{
    public class File : AuditableEntity, IEntity
    {
        public int Id { get; set; }

        public string OriginalName { get; set; } = null!;

        public FileExtension Extension { get; set; } 

        public long Size { get; set; }

        public string StoragePath { get; set; } = null!;

        public virtual User? User { get; set; }
        public virtual ClassroomType? ClassroomType { get; set; }
    }
}
