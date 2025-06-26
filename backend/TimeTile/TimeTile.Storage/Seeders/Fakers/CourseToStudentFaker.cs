using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Enums;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Seeders.Fakers
{
    internal class CourseToStudentFaker : BaseFaker<CourseToStudent>
    {
        // HasExam constraints
        private const float EXAM_PRESENCE = 0.6f;

        // ExamGrade constraints
        private const float GRADE_PRESENCE = 0.7f;

        // Pre-generate all pairs logic
        private readonly List<(int CourseId, int StudentId)> _possiblePairs = new();
        private int _actualIndex = 0;

        private GradeFaker _gradeFaker = new GradeFaker(GradeType.Exam);

        public CourseToStudentFaker(List<Course> courses, List<Student> students)
        {
            FindAllPossibleCombinations(courses, students);

            _possiblePairs = _possiblePairs.OrderBy(_ => Guid.NewGuid()).ToList();

            _faker
                .Rules((faker, entity) =>
                {
                    (int, int) element = _possiblePairs.ElementAt(_actualIndex);

                    entity.CourseId = element.Item1;
                    entity.StudentId = element.Item2;

                    _actualIndex++;
                })
                .RuleFor(cs => cs.HasExam, f => f.Random.Bool(EXAM_PRESENCE))
                .RuleFor(cs => cs.ExamGrade, (f, cs) =>
                {
                    if (! cs.HasExam)
                    {
                        return null;
                    }

                    if (f.Random.Bool(GRADE_PRESENCE))
                    {
                        return _gradeFaker.Generate(1).First();
                    }

                    return null;
                })
                .RuleFor(cs => cs.PositionX, (short)0)                 // TODO: real positions
                .RuleFor(cs => cs.PositionY, (short)0);
        }

        private void FindAllPossibleCombinations(List<Course> courses, List<Student> students)
        {
            var commonInstitutionsIds = FindCommonInstitutionsIds(courses, students);

            if (commonInstitutionsIds.Count() == 0)
                throw new InvalidOperationException("There are no associations between courses and students with the same institution.");

            var suitableCourses = courses
                .Where(c => commonInstitutionsIds.Contains(c.InstitutionId));

            var suitableStudents = students
                .Where(s => commonInstitutionsIds.Contains((int) s.InstitutionId));

            AddAllPossibleCombinations(suitableCourses, suitableStudents);
        }

        private IEnumerable<int> FindCommonInstitutionsIds(List<Course> courses, List<Student> students)
        {
            HashSet<int> coursesInstitutionsIds = new();
            foreach (var course in courses)
                coursesInstitutionsIds.Add(course.InstitutionId);

            HashSet<int> studentsInstitutionsIds = new();
            foreach (var student in students)
                studentsInstitutionsIds.Add((int) student.InstitutionId);

            return coursesInstitutionsIds.Intersect(studentsInstitutionsIds);
        }

        private void AddAllPossibleCombinations(IEnumerable<Course> suitableCourses, IEnumerable<Student> suitableStudents)
        {
            foreach (var course in suitableCourses)
            {
                foreach (var student in suitableStudents)
                {
                    if (course.InstitutionId == student.InstitutionId)
                    {
                        _possiblePairs.Add((course.Id, student.Id));
                    }
                }
            }
        }

        public override List<CourseToStudent> Generate(int count)
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
