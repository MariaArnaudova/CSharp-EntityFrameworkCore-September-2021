using Microsoft.EntityFrameworkCore;
using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        public static void Main(string[] args)
        {

            var context = new SoftUniContext();
            // var result =  GetEmployeesFullInformation(context);
            // var result = GetEmployeesWithSalaryOver50000(context);
            // var result = GetEmployeesFromResearchAndDevelopment(context);
            // var result = AddNewAddressToEmployee(context);
            //var result = GetEmployeesInPeriod(context);
            //var result = GetEmployee147(context);
            //var result = GetDepartmentsWithMoreThan5Employees(context);
            //var result = GetLatestProjects(context);
            //var result = IncreaseSalaries(context);
           var result = GetEmployeesByFirstNameStartingWithSa(context);
             // var result = DeleteProjectById(context);
             // var result = RemoveTown(context);
            Console.WriteLine(result);

        }
        //----- 3.Employees Full Information
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees
                .Select(x => new
                {
                    employeeId = x.EmployeeId,
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    middleName = x.MiddleName,
                    jobTitle = x.JobTitle,
                    salary = x.Salary
                })
                .OrderBy(x => x.employeeId)
                .ToList();

            var sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.firstName} {employee.lastName} {employee.middleName} {employee.jobTitle} { employee.salary:F2}");
            }
            return sb.ToString().TrimEnd();
        }

        // --------04. Employees with Salary Over 50 000 
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {

            var employees = context.Employees.Where(x => x.Salary > 50000)
                .Select(x => new
                {
                    firstName = x.FirstName,
                    salary = x.Salary
                })
                .OrderBy(x => x.firstName);

            var sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.firstName} - {employee.salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        // ------05. Employees from Research and Development 
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {//their first name, last name, department name and salary rounded to 2 symbols
            var employees = context.Employees
                .Where(x => x.Department.Name == "Research and Development")
                .Select(x => new
                {
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    departmentName = x.Department.Name,
                    salary = x.Salary
                })
                .OrderBy(x => x.salary)
                .ThenByDescending(x => x.firstName);

            var sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.firstName} {employee.lastName} from {employee.departmentName} - ${employee.salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        // -----06.adding a new address and updating employee
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            Address newAddress = new Address
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            context.Addresses.Add(newAddress);

            Employee nakov = context.Employees.FirstOrDefault(x => x.LastName == "Nakov");

            nakov.Address = newAddress;
            context.SaveChanges();

            var addresses = context.Employees
                .Select(x => new
                {
                    addressId = x.Address.AddressId,
                    addressText = x.Address.AddressText
                })
                .OrderByDescending(x => x.addressId)
                .Take(10)
                .ToList();

            var sb = new StringBuilder();

            foreach (var addressNakov in addresses)
            {
                sb.AppendLine($"{addressNakov.addressText}");
            }
            return sb.ToString().TrimEnd();
        }

        // -----07. Employees and Projects 

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employees = context.Employees
                .Include(x => x.EmployeesProjects)
                .ThenInclude(x => x.Project)
                .Where(x => x.EmployeesProjects.Any(x => x.Project.StartDate.Year >= 2001 &&
                                                      x.Project.StartDate.Year <= 2003))
                .Select(x => new
                {
                    EmployeeFirstName = x.FirstName,
                    EmployeeLastName = x.LastName,
                    ManagerFirstName = x.Manager.FirstName,
                    ManagerLastName = x.Manager.LastName,
                    Projects = x.EmployeesProjects.Select(p => new
                    {
                        ProjectName = p.Project.Name,
                        StartDate = p.Project.StartDate,
                        EndDate = p.Project.EndDate
                    })
                })
                .Take(10)
                .ToList();

            var sb = new StringBuilder();

            foreach (var em in employees)
            {
                sb.AppendLine($"{em.EmployeeFirstName} {em.EmployeeLastName} - Manager: {em.ManagerFirstName} {em.ManagerLastName}");

                foreach (var project in em.Projects)
                {
                    var endDate = project.EndDate.HasValue
                        ? project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
                        : "not finished";

                    sb.AppendLine($"--{project.ProjectName} - {project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)} - {endDate}");
                }
            }

            return sb.ToString().TrimEnd();


            //foreach (var em in employees)
            //{
            //    sb.AppendLine($"{em.EmployeeFirstName} {em.EmployeeLastName} - Manager: {em.ManagerFirstName} {em.ManagerLastName}");

            //    foreach (var project in em.Projects)
            //    {
            //        string startDate = project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture );
            //        string endDate = project.EndDate.ToString();

            //        if (project.EndDate == null)
            //        {
            //            sb.AppendLine($"--{project.ProjectName} - {startDate} - {endDate} not finished");
            //        }
            //        else
            //        {
            //            sb.AppendLine($"--{project.ProjectName} - {startDate} - {endDate}");
            //        }                 
            //    }
            //}
        }

        // ----8.	Addresses by Town
        public static string GetAddressesByTown(SoftUniContext context)
        {

            var addreses = context.Addresses
                .Where(a => a.Employees.Any(e => e.AddressId == a.AddressId))
                .Select(a => new
                {
                    EmployeesCount = a.Employees.Count,
                    //EmployeesCount = a.Employees.Select(e => e.AddressId == a.AddressId).Count(),
                    AddressText = a.AddressText,
                    TownName = a.Town.Name
                })
                .OrderByDescending(a => a.EmployeesCount)
                .ThenBy(a => a.TownName)
                .ThenBy(a => a.AddressText)
                .Take(10);

            var sb = new StringBuilder();
            foreach (var address in addreses)
            {
                sb.AppendLine($"{address.AddressText}, {address.TownName} - {address.EmployeesCount} employees");
            }

            return sb.ToString().TrimEnd();
        }

        //----9.	Employee 147
        public static string GetEmployee147(SoftUniContext context)
        {

            var employeeId147 = context.Employees//.Where(e => e.EmployeeId == 147)
                .Select(x => new
                {
                    //first name, last name, job title and projects 
                    x.EmployeeId,
                    x.FirstName,
                    x.LastName,
                    x.JobTitle,
                    Projects = x.EmployeesProjects.OrderBy(x => x.Project.Name).Select(p => new
                    {
                        p.Project.Name
                    })
                })
                .FirstOrDefault(x => x.EmployeeId == 147);

            var sb = new StringBuilder();

            sb.AppendLine($"{employeeId147.FirstName} {employeeId147.LastName} - {employeeId147.JobTitle}");

            foreach (var project in employeeId147.Projects)
            {
                sb.AppendLine($"{project.Name}");
            }

            return sb.ToString().TrimEnd();
        }

        //public static string GetEmployee147(SoftUniContext context)
        //{

        //    var employeeId147 = context.Employees
        //        .Select(x => new Employee
        //        {
        //            //first name, last name, job title and projects 
        //            EmployeeId = x.EmployeeId,
        //            FirstName = x.FirstName,
        //            LastName = x.LastName,
        //            JobTitle = x.JobTitle,
        //            EmployeesProjects = x.EmployeesProjects.Select(p => new EmployeeProject
        //            {
        //               Project =  p.Project
        //            })
        //            .OrderBy(x => x.Project.Name)
        //            .ToList()
        //        })
        //        .FirstOrDefault(x => x.EmployeeId == 147);

        //    var sb = new StringBuilder();

        //    sb.AppendLine($"{employeeId147.FirstName} {employeeId147.LastName} - {employeeId147.JobTitle}");

        //    foreach (var project in employeeId147.EmployeesProjects)
        //    {
        //        sb.AppendLine($"{project.Project.Name}");
        //    }

        //    return sb.ToString().TrimEnd();
        //}

        //-----10.	Departments with More Than 5 Employees
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departments = context.Departments
                .Where(x => x.Employees.Count > 5)
                .OrderBy(x => x.Employees.Count)
                .ThenBy(x => x.Name)
                .Select(x => new
                {
                    // employeesCount = x.Employees.Count(), Гърми в джъдж
                    departmenName = x.Name,
                    managerFirsName = x.Manager.FirstName,
                    managerLastName = x.Manager.LastName,
                    employeeData = x.Employees
                    .Select(e => new
                    {
                        e.FirstName,
                        e.LastName,
                        e.JobTitle
                    })
                    .OrderBy(x => x.FirstName)
                    .ThenBy(x => x.LastName)
                    .ToList()
                })
                .ToList();

            var sb = new StringBuilder();

            foreach (var dep in departments)
            {
                sb.AppendLine($"{dep.departmenName} – {dep.managerFirsName} {dep.managerLastName}");

                foreach (var emp in dep.employeeData)
                {
                    sb.AppendLine($"{emp.FirstName} {emp.LastName} - {emp.JobTitle}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //------11. Find Latest 10 Projects 
        public static string GetLatestProjects(SoftUniContext context)
        {
            var projects10 = context.Projects
                .OrderByDescending(x => x.StartDate)
                .Take(10)
                .OrderBy(p => p.Name)
                .ToList();

            var sb = new StringBuilder();

            foreach (var project in projects10)
            {
                sb.AppendLine($"{project.Name}");
                sb.AppendLine($"{project.Description}");
                sb.AppendLine($"{project.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)}");
            }

            return sb.ToString().TrimEnd();
        }

        //------12. Increase Salaries

        public static string IncreaseSalaries(SoftUniContext context)
        {
            var salaries = context.Employees
                .Where(x => x.Department.Name == "Engineering"
                         || x.Department.Name == "Tool Design"
                         || x.Department.Name == "Marketing"
                         || x.Department.Name == "Information Services");

            foreach (var salary in salaries)
            {
                salary.Salary *= 1.12m;
            }

            context.SaveChanges();

            var employeesOutput = salaries
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    x.Salary
                })
           .OrderBy(e => e.FirstName)
           .ThenBy(e => e.LastName)
           .ToList();

            var sb = new StringBuilder();

            foreach (var sal in employeesOutput)
            {
                sb.AppendLine($"{sal.FirstName} {sal.LastName} (${sal.Salary:F2})");
            }

            return sb.ToString().TrimEnd();
        }

        // -----13. Find Employees by First Name Starting With Sa 
       // public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        //{

        //    var employees = context.Employees
        //        .Where(x => x.FirstName.StartsWith("Sa"))
        //        .OrderBy(x => x.FirstName)
        //        .ThenBy(x => x.LastName)
        //        .Select(e => new
        //        {
        //            FirstName = e.FirstName,
        //            LastName = e.LastName,
        //            JobTitle = e.JobTitle,
        //            Salaty = e.Salary
        //        });

        //    var sb = new StringBuilder();

        //    foreach (var emp in employees)
        //    {
        //        sb.AppendLine($"{emp.FirstName} {emp.LastName} - {emp.JobTitle} - (${emp.Salaty:F2})");
        //    }

        //    return sb.ToString().TrimEnd();
        //}

        // =====14.	Delete Project by Id
        public static string DeleteProjectById(SoftUniContext context)
        {
    
            var project = context.Projects.FirstOrDefault(x => x.ProjectId == 2);

            var projectsRefferenceRemove = context.EmployeesProjects
                                  .Where( x => x.ProjectId == 2).ToList();

            foreach (var projToRemove in projectsRefferenceRemove)
            {
                context.EmployeesProjects.Remove(projToRemove);
            }

            
            context.Projects.Remove(project);

           // context.SaveChanges();

            var projects10 = context.Projects.Take(10).ToList();


            var sb = new StringBuilder();

            foreach (var proj in projects10)
            {
                sb.AppendLine(proj.Name);
            }

            return sb.ToString().TrimEnd();
        }

        //---15.	Remove Town

        public static string RemoveTown(SoftUniContext context)
        {
            
            Town townToRemove = context.Towns
               // .Include(x =>x.Addresses)
                .FirstOrDefault(t => t.Name == "Seattle");

            List<Address> addresesInTowns = context
                .Addresses.Where(t => t.Town.Name == "Seattle").ToList();

            List<Employee> employessInTown = context.Employees
               // .Where(a => a.Address.Town.Name == "Seattle" ).ToList();
              .ToList()
              .Where(a => addresesInTowns.Any(x => a.AddressId == x.AddressId)).ToList();

            foreach (Employee employees in employessInTown)
            {
                employees.AddressId = null;
            }

            var countDeletedAddresses = addresesInTowns.Count();
    
            foreach (var addreses in addresesInTowns)
            {
                context.Addresses.Remove(addreses);
                addreses.TownId = null;
            }

            context.Towns.Remove(townToRemove);

             
           return $"{countDeletedAddresses} addresses in Seattle were deleted";
        }



        // dani demo

    }
}


