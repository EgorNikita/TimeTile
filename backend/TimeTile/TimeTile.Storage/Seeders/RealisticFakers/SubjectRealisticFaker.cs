using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Seeders.RealisticFakers
{
    internal class SubjectRealisticFaker
    {
        private static readonly string[] _possibleSubjects = new[]
        {
            "Mathematics", "Physics", "Biology", "Computer Science", "Economics",
            "Psychology", "Sociology", "History", "Political Science", "Philosophy",
            "Environmental Studies", "Business Administration", "Linguistics"
        };

        public static List<Subject> Generate(List<Institution> institutions)
        {
            List<Subject> subjects = new();

            foreach (var institution in institutions)
            {
                foreach (var title in _possibleSubjects)
                {
                    subjects.Add(new Subject
                    {
                        Title = title,
                        InstitutionId = institution.Id
                    });
                }
            }

            return subjects;
        }
    }
}
