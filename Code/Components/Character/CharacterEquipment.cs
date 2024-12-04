using Sandbox;

namespace SoulsBox
{
	/// <summary>
	/// Equipment for souls box character
	/// </summary>
	[Title( "Souls Box Character Equipment" )]
	[Category( "Souls Box Character" )]
	[Icon( "shield" )]
	public sealed class CharacterEquipment : Component
	{

		[Property]
		CharacterAgent CharacterAgent { get; set; }

		public readonly Weapon DefaultWeapon = new Weapon { Name = "Empty", ID = "none" };
		public readonly Usable DefaultUsable = new Usable { Name = "Empty", ID = "none" };
		public readonly Arrow DefaultArrow = new Arrow { Name = "Empty", ID = "none" };
		public readonly Armor DefaultArmor = new Armor { Name = "Empty", ID = "none" };
		public readonly Ring DefaultRing = new Ring { Name = "Empty", ID = "none" };

		public List<Weapon> RightHandWeapons { get; set; }
		public List<Weapon> LeftHandWeapons { get; set; }
		public List<Usable> Usables { get; set; }
		public List<Arrow> Arrows { get; set; }
		public List<Arrow> Bolts { get; set; }
		public List<Armor> Armors { get; set; }
		public List<Ring> Rings { get; set; }

		public CharacterEquipment()
		{
			RightHandWeapons = new List<Weapon> { DefaultWeapon, DefaultWeapon };
			LeftHandWeapons = new List<Weapon> { DefaultWeapon, DefaultWeapon };
			Usables = new List<Usable> { DefaultUsable, DefaultUsable, DefaultUsable, DefaultUsable, DefaultUsable };
			Arrows = new List<Arrow> { DefaultArrow, DefaultArrow };
			Bolts = new List<Arrow> { DefaultArrow, DefaultArrow };
			Armors = new List<Armor> { DefaultArmor, DefaultArmor, DefaultArmor, DefaultArmor }; // Head, Chest, Hands, Legs
			Rings = new List<Ring> { DefaultRing, DefaultRing };
		}

		public int SelectedLeftHand { get; set; } = 0;
		public int SelectedRightHand { get; set; } = 0;
		public int SelectedUsable { get; set; } = 0;
		public int SelectedMagic { get; set; } = 0;

		private bool hasEquipmentChanged = false;

		public bool HasEquipmentChanged
		{
			get => hasEquipmentChanged;
			private set => hasEquipmentChanged = value;
		}

		public void ResetEquipmentChangedFlag()
		{
			HasEquipmentChanged = false;
		}

		public void CycleSelectedLeftHand()
		{
			SelectedLeftHand = (SelectedLeftHand + 1) % LeftHandWeapons.Count;
			HasEquipmentChanged = true;
		}

		public void CycleSelectedRightHand()
		{
			SelectedRightHand = (SelectedRightHand + 1) % RightHandWeapons.Count;
			HasEquipmentChanged = true;
		}

		public void CycleSelectedUsable()
		{
			SelectedUsable = (SelectedUsable + 1) % Usables.Count;
			HasEquipmentChanged = true;
		}

		public void CycleSelectedMagic()
		{
			SelectedMagic = (SelectedMagic + 1) % CharacterAgent.CharacterStats.AttunementSlots;
			HasEquipmentChanged = true;
		}

		public Weapon GetSelectedLeftHand()
		{
			return LeftHandWeapons[SelectedLeftHand];
		}

		public Weapon GetSelectedRightHand()
		{
			return RightHandWeapons[SelectedRightHand];
		}

		public Usable GetSelectedUsable()
		{
			return Usables[SelectedUsable];
		}

		public Item GetSelectedMagic()
		{
			return null;
		}

		public Weapon GetLeftHand( int index )
		{
			if ( index >= 0 && index < LeftHandWeapons.Count )
				return LeftHandWeapons[index];
			else
				return new Weapon { Name = "None",ID = "none" };
		}

		public Weapon GetRightHand( int index )
		{
			if ( index >= 0 && index < RightHandWeapons.Count )
				return RightHandWeapons[index];
			else
				return new Weapon { Name = "None",ID = "none" };
		}

		public Arrow GetArrow( int index )
		{
			if ( index >= 0 && index < Arrows.Count )
				return Arrows[index];
			else
				return new Arrow { Name = "None",ID = "none" };
		}

		public Arrow GetBolt( int index )
		{
			if ( index >= 0 && index < Bolts.Count )
				return Bolts[index];
			else
				return new Arrow { Name = "None",ID = "none" };
		}

		public Armor GetArmor( int index )
		{
			if ( index >= 0 && index < Armors.Count )
				return Armors[index];
			else
				return new Armor { Name = "None",ID = "none" };
		}

		public Ring GetRing( int index )
		{
			if ( index >= 0 && index < Rings.Count )
				return Rings[index];
			else
				return new Ring { Name = "None",ID = "none" };
		}

		public Usable GetUsable( int index )
		{
			if ( index >= 0 && index < Usables.Count )
				return Usables[index];
			else
				return new Usable { Name = "None",ID = "none" };
		}

		public Item GetMagic(int index)
		{
			return null;
		}

		public void SetLeftHand(Weapon weapon, int index )
		{
			LeftHandWeapons[index] = weapon;
			HasEquipmentChanged = true;
		}

		public void SetRightHand(Weapon weapon, int index )
		{
			RightHandWeapons[index] = weapon;
			HasEquipmentChanged = true;
		}

		public void SetArrow(Arrow arrow, int index )
		{
			Arrows[index] = arrow;
			HasEquipmentChanged = true;
		}

		public void SetBolt(Arrow bolt, int index )
		{
			Bolts[index] = bolt;
			HasEquipmentChanged = true;
		}

		public void SetArmor(Armor armor, int index )
		{
			Armors[index] = armor;
			HasEquipmentChanged = true;
		}

		public void SetRing(Ring ring, int index )
		{
			Rings[index] = ring;
			HasEquipmentChanged = true;
		}

		public void SetUsable(Usable usable, int index )
		{
			Usables[index] = usable;
			HasEquipmentChanged = true;
		}

		public Item SetMagic(Sorcery sorcery, int index )
		{
			return null;
		}

		public List<Item> GetList( MenuEquipment.ItemCategory category )
		{
			return category switch
			{
				MenuEquipment.ItemCategory.Usable => Usables.Cast<Item>().ToList(),
				MenuEquipment.ItemCategory.LWeapon => LeftHandWeapons.Cast<Item>().ToList(),
				MenuEquipment.ItemCategory.RWeapon => RightHandWeapons.Cast<Item>().ToList(),
				MenuEquipment.ItemCategory.Arrow => Arrows.Cast<Item>().ToList(),

				MenuEquipment.ItemCategory.Head => Armors.Cast<Item>().ToList(),
				MenuEquipment.ItemCategory.Chest => Armors.Cast<Item>().ToList(),
				MenuEquipment.ItemCategory.Legs => Armors.Cast<Item>().ToList(),
				MenuEquipment.ItemCategory.Hands => Armors.Cast<Item>().ToList(),

				MenuEquipment.ItemCategory.Ring => Rings.Cast<Item>().ToList(),
				_ => new List<Item>()
			};
		}

		public void AddWeapon( Weapon weapon, int quantity, bool isRightHand )
		{
			List<Weapon> weapons = isRightHand ? RightHandWeapons : LeftHandWeapons;

			var existingWeapon = weapons.FirstOrDefault( w => w.ID == weapon.ID );
			if ( existingWeapon != null )
			{
				existingWeapon.Quantity += quantity;
			}
			else
			{
				weapon.Quantity = quantity;
				weapons.Add( weapon );
			}

			HasEquipmentChanged = true;
		}

		public void RemoveWeapon( string id, int quantity, bool isRightHand )
		{
			List<Weapon> weapons = isRightHand ? RightHandWeapons : LeftHandWeapons;

			var existingWeapon = weapons.FirstOrDefault( w => w.ID == id );
			if ( existingWeapon != null )
			{
				existingWeapon.Quantity -= quantity;
				if ( existingWeapon.Quantity <= 0 )
				{
					weapons.Remove( existingWeapon );
				}
			}

			HasEquipmentChanged = true;
		}

		public void AddUsable( Usable usable, int quantity )
		{
			var existingUsable = Usables.FirstOrDefault( u => u.ID == usable.ID );
			if ( existingUsable != null )
			{
				existingUsable.Quantity += quantity;
			}
			else
			{
				usable.Quantity = quantity;
				Usables.Add( usable );
			}

			HasEquipmentChanged = true;
		}

		public void RemoveUsable( string id, int quantity )
		{
			var existingUsable = Usables.FirstOrDefault( u => u.ID == id );
			if ( existingUsable != null )
			{
				existingUsable.Quantity -= quantity;
				if ( existingUsable.Quantity <= 0 )
				{
					Usables.Remove( existingUsable );
				}
			}

			HasEquipmentChanged = true;
		}

		public void AddArrow( Arrow arrow, int quantity, bool isBolt )
		{
			List<Arrow> arrows = isBolt ? Bolts : Arrows;

			var existingArrow = arrows.FirstOrDefault( a => a.ID == arrow.ID );
			if ( existingArrow != null )
			{
				existingArrow.Quantity += quantity;
			}
			else
			{
				arrow.Quantity = quantity;
				arrows.Add( arrow );
			}

			HasEquipmentChanged = true;
		}

		public void RemoveArrow( string id, int quantity, bool isBolt )
		{
			List<Arrow> arrows = isBolt ? Bolts : Arrows;

			var existingArrow = arrows.FirstOrDefault( a => a.ID == id );
			if ( existingArrow != null )
			{
				existingArrow.Quantity -= quantity;
				if ( existingArrow.Quantity <= 0 )
				{
					arrows.Remove( existingArrow );
				}
			}

			HasEquipmentChanged = true;
		}

		public void AddArmor( Armor armor, int quantity )
		{
			var existingArmor = Armors.FirstOrDefault( a => a.ID == armor.ID );
			if ( existingArmor != null )
			{
				existingArmor.Quantity += quantity;
			}
			else
			{
				armor.Quantity = quantity;
				Armors.Add( armor );
			}

			HasEquipmentChanged = true;
		}

		public void RemoveArmor( string id, int quantity )
		{
			var existingArmor = Armors.FirstOrDefault( a => a.ID == id );
			if ( existingArmor != null )
			{
				existingArmor.Quantity -= quantity;
				if ( existingArmor.Quantity <= 0 )
				{
					Armors.Remove( existingArmor );
				}
			}

			HasEquipmentChanged = true;
		}

		public void AddRing( Ring ring, int quantity )
		{
			var existingRing = Rings.FirstOrDefault( r => r.ID == ring.ID );
			if ( existingRing != null )
			{
				existingRing.Quantity += quantity;
			}
			else
			{
				ring.Quantity = quantity;
				Rings.Add( ring );
			}

			HasEquipmentChanged = true;
		}

		public void RemoveRing( string id, int quantity )
		{
			var existingRing = Rings.FirstOrDefault( r => r.ID == id );
			if ( existingRing != null )
			{
				existingRing.Quantity -= quantity;
				if ( existingRing.Quantity <= 0 )
				{
					Rings.Remove( existingRing );
				}
			}

			HasEquipmentChanged = true;
		}
	}
}
