using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTile.Core.Models;
using static TimeTile.Core.Common.Constants.Permissions;

namespace TimeTile.Storage.Seeders.Fakers
{
    internal class AssignmentFaker
    {
        public string GenerateValidTitle(Lesson lesson)
        {
            var subjectTitle = lesson.Course.Subject.Title;
            var formattedDate = lesson.Date.ToString("dd.MM.yyyy");

            return $"{subjectTitle} {formattedDate}";
        }

        public string GenerateValidDescription(Faker faker, Lesson lesson)
        {
            var templates = new[]
            {
                $"This assignment focuses on {lesson.Description.ToLower()}. {faker.Lorem.Sentence()} " +
                $"Submit your work by {lesson.Date:dd.MM.yyyy}. " +
                $"Length: {faker.Random.Int(1, 5)} pages. Format: {faker.PickRandom("PDF", "Word doc", "handwritten")}.",

                $"Complete the following tasks related to {lesson.Description.ToLower()}: {faker.Lorem.Sentence()} " +
                $"Due date: {lesson.Date:dd.MM.yyyy}. " +
                $"Points: {faker.Random.Int(0, 15)}.",

                $"Research and analyze {lesson.Description.ToLower()}. {faker.Lorem.Sentence()} " +
                $"Include {faker.Random.Int(2, 8)} sources and proper citations. " +
                $"Deadline: {lesson.Date:dd.MM.yyyy}. Contact me with questions.",

                $"Your task is to explore {lesson.Description.ToLower()} in detail. {faker.Lorem.Sentence()} " +
                $"Minimum word count: {faker.Random.Int(300, 1500)}. " +
                $"Submit via {faker.PickRandom("email", "classroom portal", "printed copy")}."
            };

            return faker.PickRandom(templates);
        }
    }
}
