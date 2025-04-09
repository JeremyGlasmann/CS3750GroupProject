using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RegisterPage.Data;
using Microsoft.EntityFrameworkCore;
using RegisterPage.model;

namespace RegisterPage.Pages
{
    public class CalendarModel : PageModel
    {
        private readonly RegisterPageContext _context;

        public CalendarModel(RegisterPageContext context)
        {
            _context = context;
        }

        public List<CalendarEvent> CollegeInformationlist { get; set; }
        public string Role { get; set; }

        public async Task OnGetAsync()
        {
            CollegeInformationlist = new List<CalendarEvent>();
            var username = HttpContext.Session.GetString("Username");

            // Fetch the user from the database using the session username
            var user = await _context.register
                .Include(u => u.Classes) // Include classes directly
                .SingleOrDefaultAsync(u => u.username == username);

            if (user != null)
            {
                Role = user.role;

                // Fetch and add events based on user role
                if (Role == "Student")
                {
                    foreach (var classObj in user.Classes)
                    {
                        // Fetch assignments for each class
                        var assignments = await _context.assignments
                            .Where(a => a.classID == classObj.Id)
                            .Select(a => new { a.ID, a.title, a.dueDate, a.classID })
                            .ToListAsync();

                        foreach (var assignment in assignments)
                        {
                            CollegeInformationlist.Add(new CalendarEvent
                            {
                                Title = assignment.title,
                                Start = assignment.dueDate,
                                End = assignment.dueDate,
                                Url = $"/Course?id={assignment.classID}" // Link to the course page
                            });
                        }

                        // Add weekly class events for students
                        CreateWeeklyEvents(classObj, CollegeInformationlist, true);
                    }
                }
                else if (Role == "Teacher")
                {
                    var userClasses = await _context.classes
                        .Where(c => c.professorID == user.Id)
                        .ToListAsync();

                    foreach (var userClass in userClasses)
                    {
                        // Add weekly class events for teachers
                        CreateWeeklyEvents(userClass, CollegeInformationlist, true);
                    }
                }
            }
        }

        // Method to create weekly events for classes
        private void CreateWeeklyEvents(classes userClass, List<CalendarEvent> eventsList, bool showDays)
        {
            // Define the range of dates for the semester
            DateTime rangeStartDate = new DateTime(2024, 8, 28);
            DateTime rangeEndDate = new DateTime(2024, 12, 12);

            // Extract class start and end times
            DateTime actualStartDate = userClass.startDate;
            TimeSpan startTimeOfDay = userClass.startTime.TimeOfDay;
            TimeSpan endTimeOfDay = userClass.endTime.TimeOfDay;

            if (showDays)
            {
                string daysOfWeek = userClass.days ?? "M"; // Default to Monday if days are null
                foreach (char dayChar in daysOfWeek)
                {
                    DayOfWeek day = MapDayCharToDayOfWeek(dayChar);
                    DateTime classStart = FindNextOccurrenceOfDay(rangeStartDate, day).Add(startTimeOfDay);
                    DateTime classEnd = classStart.Add(endTimeOfDay - startTimeOfDay);

                    while (classStart <= rangeEndDate)
                    {
                        if (classStart >= rangeStartDate && classStart <= rangeEndDate)
                        {
                            // Add class event to the calendar list with a link to the course page
                            eventsList.Add(new CalendarEvent
                            {
                                Title = userClass.courseName,
                                Start = classStart,
                                End = classEnd,
                                Url = $"/Course?id={userClass.Id}" // Link to the course page
                            });
                        }

                        // Move to next week's class time
                        classStart = classStart.AddDays(7);
                        classEnd = classEnd.AddDays(7);
                    }
                }
            }
            else
            {
                DateTime classStart = actualStartDate.Add(startTimeOfDay);
                DateTime classEnd = classStart.Add(endTimeOfDay - startTimeOfDay);

                while (classStart <= rangeEndDate)
                {
                    if (classStart >= rangeStartDate && classStart <= rangeEndDate)
                    {
                        // Add single class event to the calendar list with a link to the course page
                        eventsList.Add(new CalendarEvent
                        {
                            Title = userClass.courseName,
                            Start = classStart,
                            End = classEnd,
                            Url = $"/Course?id={userClass.Id}" // Link to the course page
                        });
                    }

                    // Move to next week's class time
                    classStart = classStart.AddDays(7);
                    classEnd = classEnd.AddDays(7);
                }
            }
        }

        // Convert day character to DayOfWeek enum
        private DayOfWeek MapDayCharToDayOfWeek(char dayChar)
        {
            return dayChar.ToString().ToUpper() switch
            {
                "M" => DayOfWeek.Monday,
                "T" => DayOfWeek.Tuesday,
                "W" => DayOfWeek.Wednesday,
                "R" => DayOfWeek.Thursday,
                "F" => DayOfWeek.Friday,
                "S" => DayOfWeek.Saturday,
                "U" => DayOfWeek.Sunday,
                _ => DayOfWeek.Monday,
            };
        }

        // Find the next occurrence of a specific day starting from a given date
        private DateTime FindNextOccurrenceOfDay(DateTime startDate, DayOfWeek dayOfWeek)
        {
            int daysToAdd = ((int)dayOfWeek - (int)startDate.DayOfWeek + 7) % 7;
            return startDate.AddDays(daysToAdd == 0 ? 7 : daysToAdd);
        }

        // Model to represent each calendar event
        public class CalendarEvent
        {
            public string Title { get; set; }
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
            public string Url { get; set; }
        }
    }
}
