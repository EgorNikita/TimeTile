using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Models;

namespace TimeTile.Storage.Seeders.Fakers
{
    internal class MessageFaker : BaseFaker<Message>
    {
        // User constraints
        private const float MESSAGE_FROM_TEACHER_POSSIBILITY = 0.8f;

        // User constraints
        private const float EDITION_POSSIBILITY = 0.1f;

        public MessageFaker(List<Course> courses)
        {
            var suitableCourses = courses
                .Where(c => c.Term.StartDate < DateTimeOffset.UtcNow)
                .ToList();

            _faker
                .Rules((faker, message) =>
                {
                    var course = faker.PickRandom(suitableCourses);

                    message.Course = course;

                    if (faker.Random.Bool(MESSAGE_FROM_TEACHER_POSSIBILITY))
                    {
                        message.UserId = course.TeacherId;
                    }
                    else
                    {
                        message.UserId = faker.PickRandom(course.Students).Id;
                    }
                })
                .RuleFor(m => m.Content, f => f.Random.ArrayElement(
                [
                    f.Lorem.Sentence(),
                    f.Lorem.Text(),
                    f.Hacker.Phrase()
                ]))
                .Rules((faker, message) =>
                {
                    var term = message.Course.Term;
                    var end = term.EndDate < DateTimeOffset.UtcNow 
                        ? term.EndDate 
                        : DateTimeOffset.UtcNow;

                    message.SentAt = faker.Date.BetweenOffset(term.StartDate, end);

                    if (faker.Random.Bool(EDITION_POSSIBILITY))
                    {
                        message.EditedAt = faker.Date.BetweenOffset(message.SentAt.AddSeconds(1), end);
                    }
                });
        }
    }
}
