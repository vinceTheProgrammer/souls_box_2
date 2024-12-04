using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulsBox
{
	public sealed class Arrow : Item
	{

		public int PhysicalAttack { get; set;}
		public int MagicAttack { get; set; }
		public int FireAttack { get; set; }
		public int LightningAttack { get; set; }
		public int CriticalAttack { get; set; }
		public int BleedEffect { get; set; }
		public int PoisonEffect { get; set; }
		public int DivineEffect { get; set; }
		public int OccultEffect { get; set; }
	}
}
