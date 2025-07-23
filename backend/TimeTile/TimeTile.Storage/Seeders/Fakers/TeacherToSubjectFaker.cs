using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Seeders.Fakers
{
    internal class TeacherToSubjectFaker : BaseFaker<TeacherToSubject>
    {
        private readonly List<(int TeacherId, int SubjectId)> _possiblePairs = new();

        private int _actualIndex = 0;

        public TeacherToSubjectFaker(List<InstitutionMember> teachers, List<Subject> subjects)
        {
            FindAllPossibleCombinations(teachers, subjects);

            _possiblePairs = _possiblePairs.OrderBy(_ => Guid.NewGuid()).ToList();

            _faker
                .Rules((faker, entity) =>
                {
                    (int, int) element = _possiblePairs.ElementAt(_actualIndex);

                    entity.TeacherId = element.Item1;
                    entity.SubjectId = element.Item2;

                    _actualIndex++;
                });
        }

        private void FindAllPossibleCombinations(List<InstitutionMember> teachers, List<Subject> subjects)
        {
            var commonInstitutionsIds = FindCommonInstitutionsIds(teachers, subjects);

            if (commonInstitutionsIds.Count() == 0)
                throw new InvalidOperationException("There are no associations between teachers and subjects with the same institution.");


            var suitableTeachers = teachers
                .Where(t => commonInstitutionsIds.Contains(t.InstitutionId!.Value));

            if (suitableTeachers.Count() == 0)
                throw new InvalidOperationException("There are no associations between teachers and subjects.");


            var suitableSubjects = subjects
                .Where(s => commonInstitutionsIds.Contains(s.InstitutionId));


            AddAllPossibleCombinations(suitableTeachers, suitableSubjects);
        }

        private IEnumerable<int> FindCommonInstitutionsIds(List<InstitutionMember> teachers, List<Subject> subjects)
        {
            HashSet<int> teachersInstitutionsIds = new();
            foreach (var teacher in teachers)
                teachersInstitutionsIds.Add(teacher.InstitutionId!.Value);

            HashSet<int> subjectsInstitutionsIds = new();
            foreach (var subject in subjects)
                subjectsInstitutionsIds.Add(subject.InstitutionId);

            return teachersInstitutionsIds.Intersect(subjectsInstitutionsIds);
        }

        private void AddAllPossibleCombinations(IEnumerable<InstitutionMember> suitableTeachers, IEnumerable<Subject> suitableSubjects)
        {
            foreach (var teacher in suitableTeachers)
            {
                foreach (var subject in suitableSubjects)
                {
                    if (teacher.InstitutionId == subject.InstitutionId)
                    {
                        _possiblePairs.Add((teacher.Id, subject.Id));
                    }
                }
            }
        }

        public override List<TeacherToSubject> Generate(int count)
        {
            int rest = _possiblePairs.Count - _actualIndex;

            if (count > rest)
            {
                return base.Generate(rest);
            }

            return base.Generate(count);
        }
    }
}
