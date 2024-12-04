using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulsBox
{
	public sealed class Usable : Item
    {
		public Action UseAction { get; set; }

		public void Use()
		{
			UseAction?.Invoke();
		}

    }
}
