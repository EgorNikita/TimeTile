using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Enums;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Seeders.Fakers
{
    internal class SubmissionFaker : BaseFaker<Submission>
    {
        // Pre generate all possible combinations
        private readonly List<(int LessonId, int StudentId)> _possiblePairs = new();
        private int _actualIndex = 0;

        private readonly GradeFaker _gradeFaker = new GradeFaker(GradeType.Homework);

        public SubmissionFaker(List<Lesson> lessons)
        {
            var validLessons = lessons.Where(l => l.AssignmentId != null);

            foreach (var lesson in validLessons)
            {
                foreach (var lessonToStudent in lesson.LessonsToStudents)
                {
                    _possiblePairs.Add((lesson.Id, lessonToStudent.StudentId));
                }
            }

            _possiblePairs = _possiblePairs.OrderBy(_ => Guid.NewGuid()).ToList();

            _faker
                .Rules((faker, submission) =>
                {
                    (int, int) element = _possiblePairs.ElementAt(_actualIndex);

                    submission.Assignment = validLessons.First(l => l.Id == element.Item1).Assignment!;
                    submission.StudentId = element.Item2;

                    _actualIndex++;
                })
                .RuleFor(x => x.StudentNote, f => f.Lorem.Sentences(2))
                .RuleFor(x => x.Feedback, f => f.Lorem.Paragraphs(1))
                .RuleFor(x => x.Status, f => f.PickRandom<SubmissionStatus>())
                .RuleFor(x => x.Grade, (faker, submission) =>
                {
                    if (submission.Status == SubmissionStatus.Accepted)
                    {
                        return _gradeFaker.Generate(1).First();
                    }

                    if (submission.Status == SubmissionStatus.Rejected)
                    {
                        _possiblePairs.Add((submission.Assignment.Lesson.Id, submission.StudentId));
                    }

                    return null;
                });
        }

        public override List<Submission> Generate(int count)
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
