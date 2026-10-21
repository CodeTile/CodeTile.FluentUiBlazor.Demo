
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeTile.FluentUiBlazor.Demo.Models;

public class Person
{
	public int PersonId { get; set; }
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
	public string Name => $"{FirstName} {LastName}";
	public DateTime BirthDate { get; set; }
	public string JobTitle { get; set; } = string.Empty;
	public string Department { get; set; } = string.Empty;
	public string Location { get; set; } = string.Empty;

	// Hierarchy properties
	public int? ManagerId { get; set; }
	public List<Person> DirectReports { get; set; } = new();
}


public static class CompanyOrgGenerator
{
	private static readonly string[] FirstNames = {
		"Oliver", "George", "Arthur", "Noah", "Muhammad", "Leo", "Oscar", "Harry", "Archie", "Jack",
		"Olivia", "Amelia", "Isla", "Ava", "Freya", "Mia", "Grace", "Sophia", "Charlotte", "Lily"
	};

	private static readonly string[] LastNames = {
		"Smith", "Jones", "Taylor", "Brown", "Williams", "Wilson", "Johnson", "Davies", "Patel", "Wright",
		"Thompson", "Evans", "Walker", "White", "Roberts", "Green", "Hall", "Wood", "Clarke", "Hughes"
	};

	private static readonly string[] Locations = {
		"London (HQ)", "Manchester", "Birmingham", "Edinburgh", "Bristol", "Leeds", "Remote (UK)"
	};

	// British Corporate Title Hierarchy
	private static readonly Dictionary<string, string[]> DepartmentRoles = new()
	{
		{
			"Executive Board",
			new[] { "Chief Executive Officer & Managing Director" }
		},
		{
			"Engineering & Tech",
			new[] { "Engineering Director", "Head of Engineering", "Engineering Team Lead", "Senior Software Engineer", "Software Engineer" }
		},
		{
			"Product & Design",
			new[] { "Product Director", "Head of Product", "Lead Product Manager", "Product Manager", "UX Designer" }
		},
		{
			"Commercial & Sales",
			new[] { "Commercial Director", "Head of Sales", "Sales Team Lead", "Senior Account Manager", "Business Development Executive" }
		},
		{
			"People & Operations",
			new[] { "People Director", "Head of HR", "HR Operations Manager", "HR Advisor", "Operations Administrator" }
		}
	};

	public static List<Person> GenerateFlatOrg(int totalEmployees)
	{
		var people = new List<Person>();
		var random = Random.Shared;
		int nextId = 1000;

		// 1. Create Chief Executive Officer / Managing Director
		var ceo = new Person
		{
			PersonId = nextId++,
			FirstName = FirstNames[random.Next(FirstNames.Length)],
			LastName = LastNames[random.Next(LastNames.Length)],
			JobTitle = "Chief Executive Officer & Managing Director",
			Department = "Executive Board",
			Location = "London (HQ)",
			BirthDate = DateTime.Today.AddYears(-random.Next(45, 62)),
			ManagerId = null
		};
		people.Add(ceo);

		if (totalEmployees <= 1) return people;

		// 2. Create Board-Level Directors reporting directly to CEO
		var directors = new List<Person>();
		string[] depts = { "Engineering & Tech", "Product & Design", "Commercial & Sales", "People & Operations" };

		foreach (var dept in depts)
		{
			if (people.Count >= totalEmployees) break;

			var director = new Person
			{
				PersonId = nextId++,
				FirstName = FirstNames[random.Next(FirstNames.Length)],
				LastName = LastNames[random.Next(LastNames.Length)],
				JobTitle = DepartmentRoles[dept][0], // e.g., "Engineering Director"
				Department = dept,
				Location = "London (HQ)",
				BirthDate = DateTime.Today.AddYears(-random.Next(38, 55)),
				ManagerId = ceo.PersonId
			};
			people.Add(director);
			directors.Add(director);
		}

		// 3. Populate Heads of Department, Team Leads, and Staff downwards
		var leadershipPool = new List<Person>(directors);

		while (people.Count < totalEmployees)
		{
			// Pick a manager from leadership/management pool
			var currentManager = leadershipPool[random.Next(leadershipPool.Count)];
			var dept = currentManager.Department;

			// Pick a child role relative to current manager's level
			bool isDirector = currentManager.JobTitle.Contains("Director");
			string title;

			if (isDirector)
			{
				// Directors hire "Head of..." or "Lead..."
				title = DepartmentRoles[dept][1];
			}
			else
			{
				// Heads / Leads hire middle tier or IC roles
				title = DepartmentRoles[dept][random.Next(2, DepartmentRoles[dept].Length)];
			}

			var employee = new Person
			{
				PersonId = nextId++,
				FirstName = FirstNames[random.Next(FirstNames.Length)],
				LastName = LastNames[random.Next(LastNames.Length)],
				JobTitle = title,
				Department = dept,
				Location = Locations[random.Next(Locations.Length)],
				BirthDate = DateTime.Today.AddYears(-random.Next(22, 48)),
				ManagerId = currentManager.PersonId
			};

			people.Add(employee);

			// If the new role is a management/leadership role, add to pool so they can take direct reports
			if (title.StartsWith("Head of") || title.Contains("Lead") || title.Contains("Manager"))
			{
				leadershipPool.Add(employee);
			}
		}

		// 4. Link DirectReports collections
		foreach (var person in people)
		{
			person.DirectReports = people.Where(p => p.ManagerId == person.PersonId).ToList();
		}

		return people;
	}
}