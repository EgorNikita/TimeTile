using Bogus;
using Microsoft.EntityFrameworkCore;
using TimeTile.API.Common.Constants;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Enums;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;
using TimeTile.Storage.Seeders.Config;
using TimeTile.Storage.Seeders.Fakers;
using TimeTile.Storage.Seeders.RealisticFakers;

namespace TimeTile.Storage.Seeders
{
    public class DataSeeder
    {
        private const string CURRENT_FOLDER = "Seeders";
        public static readonly string CURRENT_DIRECTORY = Path.Combine(AppContext.BaseDirectory, CURRENT_FOLDER);


        // Influence Generation's volume
        private const int INSTITUTIONS_COUNT = 1;
        private const int CLASSROOM_TYPES_COUNT = 2;
        private const int TIMETABLE_UNITS_COUNT = 16;
        private const int SUBJECTS_COUNT = 20;
        private const int TERMS_COUNT = 4;
        private const int ROLES_COUNT = 2;
        private const int GROUPS_COUNT = 1;
        private const int ADMINS_COUNT = 2;
        private const int CLASSROOMS_COUNT = 12;
        private const int ROLES_TO_PERMISSIONS_COUNT = 15;
        private const int STUDENTS_COUNT = 2;
        private const int INSTITUTION_MEMBERS_COUNT = 20;
        private const int INSTITUTION_MEMBERS_TO_GROUPS_COUNT = 2;
        private const int TEACHERS_TO_SUBJECTS_COUNT = 35;
        private const int COURSES_COUNT = 20;
        private const int COURSES_TO_STUDENTS_COUNT = 40;
        private const int LESSONS_COUNT = 400;
        private const int SUBMISSIONS_COUNT = 5000;
        private const int MESSAGES_COUNT = 2000;

        private readonly TimetileDbContext _context;
        private readonly IUserService _userService;
        private readonly IFileService _fileService;
        private readonly ICourseService _courseService;

        public DataSeeder(TimetileDbContext context, IUserService userService, IFileService fileService, ICourseService courseService)
        {
            _context = context;
            _userService = userService;
            _fileService = fileService;
            _courseService = courseService;
        }

        public async Task Seed(CancellationToken cancellationToken = default)
        {
            Randomizer.Seed = new Random(2048);

            DataGenerationConfig.SetMode(DataGenerationMode.Realistic);

            // if database is not empty
            if (await _context.Institutions.AnyAsync(cancellationToken))
                return;

            if (!Directory.Exists(CURRENT_DIRECTORY))
            {
                Directory.CreateDirectory(CURRENT_DIRECTORY);
            }

            await ClearAllTxtFiles(cancellationToken);

            // Get already seeded permissions
            var permissions = await _context.Permissions.ToListAsync(cancellationToken);

            // Get already seeded lesson statuses
            var lessonStatuses = await _context.LessonStatuses.ToListAsync(cancellationToken);

            // Get already seeded roles
            var adminRole = await _context.Roles.FirstAsync(r => r.Title == GeneralRoles.Admin, cancellationToken);
            var studentRole = await _context.Roles.FirstAsync(r => r.Title == GeneralRoles.Student, cancellationToken);

            var institutions = new InstitutionFaker().Generate(INSTITUTIONS_COUNT);
            await _context.Institutions.AddRangeAsync(institutions, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            var classroomTypes = new ClassroomTypeFaker(institutions).Generate(CLASSROOM_TYPES_COUNT);
            await _context.ClassroomTypes.AddRangeAsync(classroomTypes, cancellationToken);

            var timetableUnits = DataGenerationConfig.GenerationMode == DataGenerationMode.Random
                ? new TimetableUnitFaker(institutions).Generate(TIMETABLE_UNITS_COUNT)
                : TimetableUnitRealisticFaker.Generate(institutions);

            await _context.TimetableUnits.AddRangeAsync(timetableUnits, cancellationToken);

            var subjects = DataGenerationConfig.GenerationMode == DataGenerationMode.Random
                ? new SubjectFaker(institutions).Generate(SUBJECTS_COUNT)
                : SubjectRealisticFaker.Generate(institutions);

            await _context.Subjects.AddRangeAsync(subjects, cancellationToken);

            var terms = DataGenerationConfig.GenerationMode == DataGenerationMode.Random
                ? new TermFaker(institutions).Generate(TERMS_COUNT)
                : TermRealisticFaker.Generate(institutions);

            await _context.Terms.AddRangeAsync(terms, cancellationToken);

            var roles = new RoleFaker(institutions).Generate(ROLES_COUNT);
            await _context.Roles.AddRangeAsync(roles, cancellationToken);

            var groups = new GroupFaker(institutions).Generate(GROUPS_COUNT);
            await _context.Groups.AddRangeAsync(groups, cancellationToken);

            var admins = await new AdminFaker(adminRole, _userService, _fileService).GenerateAsync(ADMINS_COUNT, cancellationToken);
            await _context.Users.AddRangeAsync(admins, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            var classrooms = new ClassroomFaker(institutions, classroomTypes).Generate(CLASSROOMS_COUNT);
            await _context.Classrooms.AddRangeAsync(classrooms, cancellationToken);

            var rolesToPermissions = new RoleToPermissionFaker(roles, permissions).Generate(ROLES_TO_PERMISSIONS_COUNT);
            await _context.RolesPermissions.AddRangeAsync(rolesToPermissions, cancellationToken);

            var students = await new StudentFaker(studentRole, institutions, groups, _userService, _fileService).GenerateAsync(STUDENTS_COUNT, cancellationToken);
            await _context.Students.AddRangeAsync(students, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            var institutionMembers = await new InstitutionMemberFaker(roles, institutions, classrooms, _userService, _fileService).GenerateAsync(INSTITUTION_MEMBERS_COUNT, cancellationToken);
            await _context.InstitutionMembers.AddRangeAsync(institutionMembers, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            var institutionMembersToGroups = new InstitutionMemberToGroupFaker(institutionMembers, groups).Generate(INSTITUTION_MEMBERS_TO_GROUPS_COUNT);
            await _context.InstitutionMembersGroups.AddRangeAsync(institutionMembersToGroups, cancellationToken);

            var teachersToSubjects = new TeacherToSubjectFaker(institutionMembers, subjects).Generate(TEACHERS_TO_SUBJECTS_COUNT);
            await _context.TeachersSubjects.AddRangeAsync(teachersToSubjects, cancellationToken);

            var courses = new CourseFaker(subjects, institutionMembers, institutions, terms, _courseService, _fileService).Generate(COURSES_COUNT);
            await _context.Courses.AddRangeAsync(courses, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            var coursesToStudents = new CourseToStudentFaker(courses, students).Generate(COURSES_TO_STUDENTS_COUNT);
            await _context.CoursesStudents.AddRangeAsync(coursesToStudents, cancellationToken);

            var lessons = new LessonFaker(institutions, classrooms, courses, lessonStatuses, timetableUnits).Generate(LESSONS_COUNT);
            await _context.Lessons.AddRangeAsync(lessons, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            var submissions = new SubmissionFaker(lessons).Generate(SUBMISSIONS_COUNT);
            await _context.Submissions.AddRangeAsync(submissions, cancellationToken);

            var messages = new MessageFaker(courses).Generate(MESSAGES_COUNT);
            await _context.Messages.AddRangeAsync(messages, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            await GenerateTermMarks(coursesToStudents, _context, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        private static async Task ClearAllTxtFiles(CancellationToken cancellationToken)
        {
            if (!Directory.Exists(CURRENT_DIRECTORY))
            {
                return;
            }
            
            var filesPaths = Directory.GetFiles(CURRENT_DIRECTORY, "*.txt");

            foreach (var path in filesPaths)
            {
                await System.IO.File.WriteAllTextAsync(path, string.Empty, cancellationToken);
            }
        }

        private static async Task GenerateTermMarks(
            List<CourseToStudent> coursesToStudents, 
            TimetileDbContext _context, 
            CancellationToken cancellationToken)
        {
            const float FINAL_GRADE_RANGE = 2f;
            var rand = new Random();

            foreach (var courseToStudent in coursesToStudents)
            {
                if (courseToStudent.Course.Term.EndDate > DateTime.UtcNow)
                    continue;

                var submissionGradeIds = courseToStudent.Student.Submissions
                    .Where(s => s.Assignment.Lesson.CourseId == courseToStudent.CourseId)
                    .Where(s => s.GradeId != null)
                    .Select(s => s.GradeId!.Value)
                    .ToList();

                var classworkGradeIds = courseToStudent.Course.Lessons
                    .SelectMany(l => l.LessonsToStudents)
                    .Where(ls => ls.StudentId == courseToStudent.StudentId)
                    .Where(ls => ls.GradeId != null)
                    .Select(ls => ls.GradeId!.Value)
                    .ToList();

                var combinedGradeIds = submissionGradeIds.Union(classworkGradeIds);

                if (combinedGradeIds.Any())
                {
                    var weightedGrades = await _context.Grades
                        .Where(g => combinedGradeIds.Contains(g.Id))
                        .Select(g => new { g.Value, g.Weight })
                        .ToListAsync(cancellationToken);

                    var totalWeight = weightedGrades.Sum(g => g.Weight);

                    var averageValue = weightedGrades.Sum(g => g.Value * g.Weight) / totalWeight;

                    // finalValue can be within a range of 2
                    var finalValue = (float)(rand.NextDouble() * FINAL_GRADE_RANGE * 2 + (averageValue - FINAL_GRADE_RANGE));

                    courseToStudent.Grade = new Grade
                    {
                        Value = finalValue,
                        Weight = totalWeight,
                        Type = GradeType.TermMark
                    };
                }
            }
        }
    }
}
