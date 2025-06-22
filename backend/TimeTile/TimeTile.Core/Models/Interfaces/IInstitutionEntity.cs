using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTile.Core.Models.Interfaces
{
    public interface IInstitutionEntity : IEntity
    {
        int InstitutionId { get; }
    }
}
