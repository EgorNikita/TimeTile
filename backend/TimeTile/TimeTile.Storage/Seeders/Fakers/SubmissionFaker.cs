using Bogus;
using Bogus.DataSets;
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
        // StudentNote constraints
        private const float STUDENT_NOTE_PRESENCE_POSSIBILITY = 0.3f;

        // Feedback constraints
        private const float FEEDBACK_PRESENCE_POSSIBILITY = 0.75f;

        // Pre generate all possible combinations
        private readonly List<Submission> _submissions = new();
        private int _actualIndex = 0;

        private readonly GradeFaker _gradeFaker = new GradeFaker(GradeType.Homework);
        private new readonly Faker _faker = new();

        public SubmissionFaker(List<Lesson> lessons)
        {
            var submissions = lessons
                .Where(l => l.AssignmentId != null)
                .Select(l => l.Assignment)
                .SelectMany(a => a.Submissions);

            _submissions = submissions.OrderBy(_ => Guid.NewGuid()).ToList();
        }

        public override List<Submission> Generate(int count)
        {
            var newSubmissions = new List<Submission>();

            int rest = _submissions.Count - _actualIndex;

            var iterationsCount = Math.Min(rest, count);

            for (int i = 0; i < iterationsCount; i++)
            {
                var submission = _submissions[_actualIndex++];

                submission.Status = _faker.PickRandom<SubmissionStatus>();

                if (submission.Status == SubmissionStatus.Rejected)
                {
                    var newSubmission = new Submission
                    {
                        AssignmentId = submission.AssignmentId,
                        StudentId = submission.StudentId,
                        Status = SubmissionStatus.NotSubmitted
                    };

                    if (iterationsCount < count)
                    {
                        iterationsCount++;
                    }

                    _submissions.Add(newSubmission);
                    newSubmissions.Add(newSubmission);
                }

                submission.StudentNote = GenerateValidStudentNote(submission);
                submission.Feedback = GenerateValidFeedback(submission);
                submission.Grade = GenerateValidGrade(submission);
            }

            return newSubmissions;
        }

        private string? GenerateValidStudentNote(Submission submission)
        {
            if (submission.Status == SubmissionStatus.NotSubmitted)
            {
                return null;
            }

            if (_faker.Random.Bool(STUDENT_NOTE_PRESENCE_POSSIBILITY))
            {
                return _faker.Lorem.Sentences(2);
            }

            return null;
        }

        private string? GenerateValidFeedback(Submission submission)
        {
            if (submission.Status != SubmissionStatus.Accepted && submission.Status != SubmissionStatus.Rejected)
            {
                return null;
            }

            if (_faker.Random.Bool(FEEDBACK_PRESENCE_POSSIBILITY))
            {
                return _faker.Lorem.Paragraphs(1);
            }

            return null;
        }

        private Grade? GenerateValidGrade(Submission submission)
        {
            if (submission.Status == SubmissionStatus.Accepted)
            {
                return _gradeFaker.Generate(1).First();
            }

            return null;
        }
    }
}
