using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulsBox
{
	public class CategoriesComponent
	{
		public List<Category> Categories = new List<Category>();

		public Category SelectedCategory;

		public CategoriesComponent()
		{
			SelectedCategory = Categories.FirstOrDefault(); // Initialize the first category as selected
		}

		public void SelectCategory( Category category )
		{
			SelectedCategory = category;
		}

		public void CycleLeft()
		{
			var currentIndex = Categories.IndexOf( SelectedCategory );
			var newIndex = (currentIndex - 1 + Categories.Count) % Categories.Count;
			SelectedCategory = Categories[newIndex];
		}

		public void CycleRight()
		{
			var currentIndex = Categories.IndexOf( SelectedCategory );
			var newIndex = (currentIndex + 1) % Categories.Count;
			SelectedCategory = Categories[newIndex];
		}

		public class Category
		{
			public string Name { get; set; }
			public string Icon { get; set; }
		}

		public void Add(string name, string icon)
		{
			Categories.Add( new Category()
			{
				Name = name,
				Icon = icon
			} );
		}
	}
}
