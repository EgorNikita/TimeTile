using Bogus;
using Microsoft.EntityFrameworkCore;
using TimeTile.Core.Common.Interfaces.Services;
using TimeTile.Core.Models;
using TimeTile.Storage.Contexts;
using TimeTile.Storage.Seeders.Fakers;

namespace TimeTile.Storage.Seeders
{
    public class DataSeeder
    {
        private const string CURRENT_ASSEMBLY = "TimeTile.Storage";
        private const string CURRENT_FOLDER = "Seeders";
        public static readonly string CURRENT_DIRECTORY = Path.Combine(
            Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.Parent!.FullName,
            CURRENT_ASSEMBLY,
            CURRENT_FOLDER
        );

        // Influence Generation's volume
        private const int INSTITUTIONS_COUNT = 2;
        private const int CLASSROOM_TYPES_COUNT = 5;
        private const int LESSON_STATUSES_COUNT = 10;
        private const int TIMETABLE_UNITS_COUNT = 16;
        private const int SUBJECTS_COUNT = 20;
        private const int TERMS_COUNT = 4;
        private const int ROLES_COUNT = 20;
        private const int GROUPS_COUNT = 10;
        private const int ADMINS_COUNT = 2;
        private const int CLASSROOMS_COUNT = 6;
        private const int ROLES_TO_PERMISSIONS_COUNT = 100;
        private const int STUDENTS_COUNT = 200;
        private const int INSTITUTION_MEMBERS_COUNT = 20;
        private const int INSTITUTION_MEMBERS_TO_GROUPS_COUNT = 25;
        private const int TEACHERS_TO_SUBJECTS_COUNT = 35;
        private const int COURSES_COUNT = 70;
        private const int COURSES_TO_STUDENTS_COUNT = 1000;
        private const int LESSONS_COUNT = 1000;
        private const int LESSONS_TO_STUDENTS_COUNT = 5000;

        private readonly TimetileDbContext _context;

        public DataSeeder(TimetileDbContext context)
        {
            _context = context;
        }

        public async Task Seed(CancellationToken cancellationToken = default)
        {
            // if database is not empty
            if (await _context.Institutions.AnyAsync(cancellationToken))
                return;

            await ClearAllTxtFiles(cancellationToken);

            var institutions = new InstitutionFaker().Generate(INSTITUTIONS_COUNT);
            await _context.Institutions.AddRangeAsync(institutions, cancellationToken);

            var permissions = new PermissionFaker().Generate();
            await _context.Permissions.AddRangeAsync(permissions, cancellationToken);

            // Essential for Student generation
            Role studentRole = new Role()
            {
                Title = "Student"
            };
            await _context.Roles.AddAsync(studentRole, cancellationToken);

            // Essential for Admin generation
            Role adminRole = new Role()
            {
                Title = "Admin"
            };
            await _context.Roles.AddAsync(adminRole, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            var classroomTypes = new ClassroomTypeFaker(institutions).Generate(CLASSROOM_TYPES_COUNT);
            await _context.ClassroomTypes.AddRangeAsync(classroomTypes, cancellationToken);

            var lessonStatuses = new LessonStatusFaker(institutions).Generate(LESSON_STATUSES_COUNT);
            await _context.LessonStatuses.AddRangeAsync(lessonStatuses, cancellationToken);

            var timetableUnits = new TimetableUnitFaker(institutions).Generate(TIMETABLE_UNITS_COUNT);
            await _context.TimetableUnits.AddRangeAsync(timetableUnits, cancellationToken);

            var subjects = new SubjectFaker(institutions).Generate(SUBJECTS_COUNT);
            await _context.Subjects.AddRangeAsync(subjects, cancellationToken);

            var terms = new TermFaker(institutions).Generate(TERMS_COUNT);
            await _context.Terms.AddRangeAsync(terms, cancellationToken);

            var roles = new RoleFaker(institutions).Generate(ROLES_COUNT);
            await _context.Roles.AddRangeAsync(roles, cancellationToken);

            var groups = new GroupFaker(institutions).Generate(GROUPS_COUNT);
            await _context.Groups.AddRangeAsync(groups, cancellationToken);

            var admins = await new AdminFaker(adminRole).GenerateAsync(ADMINS_COUNT, cancellationToken);
            await _context.Users.AddRangeAsync(admins, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            var classrooms = new ClassroomFaker(institutions, classroomTypes).Generate(CLASSROOMS_COUNT);
            await _context.Classrooms.AddRangeAsync(classrooms, cancellationToken);

            var rolesToPermissions = new RoleToPermissionFaker(roles, permissions).Generate(ROLES_TO_PERMISSIONS_COUNT);
            await _context.RolesPermissions.AddRangeAsync(rolesToPermissions, cancellationToken);

            var students = await new StudentFaker(studentRole, institutions, groups).GenerateAsync(STUDENTS_COUNT, cancellationToken);
            await _context.Students.AddRangeAsync(students, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            var institutionMembers = await new InstitutionMemberFaker(roles, institutions, classrooms).GenerateAsync(INSTITUTION_MEMBERS_COUNT, cancellationToken);
            await _context.InstitutionMembers.AddRangeAsync(institutionMembers, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            var institutionMembersToGroups = new InstitutionMemberToGroupFaker(institutionMembers, groups).Generate(INSTITUTION_MEMBERS_TO_GROUPS_COUNT);
            await _context.InstitutionMembersGroups.AddRangeAsync(institutionMembersToGroups, cancellationToken);

            var teachersToSubjects = new TeacherToSubjectFaker(institutionMembers, subjects).Generate(TEACHERS_TO_SUBJECTS_COUNT);
            await _context.TeachersSubjects.AddRangeAsync(teachersToSubjects, cancellationToken);

            var courses = new CourseFaker(subjects, institutionMembers, institutions, terms).Generate(COURSES_COUNT);
            await _context.Courses.AddRangeAsync(courses, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            var coursesToStudents = new CourseToStudentFaker(courses, students).Generate(COURSES_TO_STUDENTS_COUNT);
            await _context.CoursesStudents.AddRangeAsync(coursesToStudents, cancellationToken);

            var lessons = new LessonFaker(institutions, classrooms, courses, lessonStatuses, timetableUnits).Generate(LESSONS_COUNT);
            await _context.Lessons.AddRangeAsync(lessons, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            var lessonsToStudents = new LessonToStudentFaker(lessons, students).Generate(LESSONS_TO_STUDENTS_COUNT);
            await _context.LessonsStudents.AddRangeAsync(lessonsToStudents, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        private static async Task ClearAllTxtFiles(CancellationToken cancellationToken)
        {
            var filesPaths = Directory.GetFiles(CURRENT_DIRECTORY, "*.txt");

            foreach (var path in filesPaths)
            {
                await System.IO.File.WriteAllTextAsync(path, string.Empty, cancellationToken);
            }
        }
    }
}
